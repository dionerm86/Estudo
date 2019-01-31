using GDA;
using Glass.Data.Helper.Calculos.Estrategia.DescontoAcrescimo.Enum;
using Glass.Data.Model;
using Glass.Pool;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Glass.Data.Helper.Calculos.Estrategia.DescontoAcrescimo
{
    abstract class BaseStrategy<T> : Singleton<T>, IDescontoAcrescimoStrategy
        where T : BaseStrategy<T>
    {
        public bool Aplicar(GDASession sessao, IContainerCalculo container, IEnumerable<IProdutoCalculo> produtos,
            TipoValor tipo, decimal valorAplicar)
        {
            if (valorAplicar == 0 || !produtos.Any() || !PermiteAplicar())
                return false;

            produtos.InicializarParaCalculo(sessao, container);

            Remover(
                sessao,
                produtos.Where(produto => PermitirRemocaoCalculoProduto(produto)),
                produto => { }
            );

            decimal totalAtual = CalcularTotalAtual(sessao, produtos);
            decimal valor = CalcularValorAplicar(tipo, valorAplicar, totalAtual);
            decimal percentualAplicar = CalcularPercentualTotalAplicar(totalAtual, valor);

            decimal valorAplicado = Aplicar(sessao, produtos, percentualAplicar);

            var produtoValorResidual = this.ObterProdutoValorResidual(produtos);
            var valorResidual = this.ObterValorResidual(sessao, produtos, tipo, valorAplicar, valorAplicado);

            this.AplicarValorResidual(sessao, produtoValorResidual, valorResidual);

            return true;
        }

        public bool Remover(GDASession sessao, IContainerCalculo container, IEnumerable<IProdutoCalculo> produtos)
        {
            var produtosRemover = produtos
                .Where(produto => PermitirRemocaoCalculoProduto(produto));

            if (!produtosRemover.Any())
                return false;

            produtosRemover.InicializarParaCalculo(sessao, container);

            Remover(
                sessao,
                produtosRemover,
                produto => RecalcularValorUnitario(sessao, produto)
            );

            return true;
        }

        protected abstract bool PermitirRemocaoCalculoProduto(IProdutoCalculo produto);

        protected abstract void AplicarValorBeneficiamento(GenericBenef beneficiamento, decimal valor);

        protected abstract void RemoverValorBeneficiamento(GenericBenef beneficiamento);

        protected abstract void AplicarValorProduto(IProdutoCalculo produto, decimal valor);

        protected abstract void RemoverValorProduto(IProdutoCalculo produto);

        protected virtual decimal CalcularValorAplicar(TipoValor tipo, decimal valorAplicar, decimal totalAtual)
        {
            if (tipo == TipoValor.Percentual)
            {
                return Math.Round(totalAtual * valorAplicar / 100, 2);
            }

            return valorAplicar;
        }

        protected virtual bool PermiteAplicar()
        {
            return true;
        }

        protected virtual decimal BaseCalculoTotalProduto(IProdutoCalculo produto)
        {
            return PrecoTabelaCliente(produto);
        }

        protected virtual decimal BaseCalculoTotalResidualProduto(IProdutoCalculo produto)
        {
            return this.BaseCalculoTotalProduto(produto);
        }

        protected virtual decimal AplicarProduto(decimal percentual, IProdutoCalculo produto)
        {
            decimal valorCalculado = Math.Round(percentual / 100 * PrecoTabelaCliente(produto), 2);
            AplicarValorProduto(produto, valorCalculado);

            return valorCalculado;
        }

        protected virtual decimal PrecoTabelaCliente(IProdutoCalculo produto)
        {
            return produto.TotalBruto - produto.ValorDescontoCliente + produto.ValorAcrescimoCliente;
        }

        protected virtual decimal AplicarBeneficiamentos(decimal percentual, IProdutoCalculo produto)
        {
            decimal valorAplicado = 0;
            var beneficiamentos = produto.Beneficiamentos ?? GenericBenefCollection.Empty;

            foreach (var beneficiamento in beneficiamentos)
            {
                decimal valorCalculado = Math.Round(percentual / 100 * beneficiamento.TotalBruto, 2);
                valorAplicado += valorCalculado;

                AplicarValorBeneficiamento(beneficiamento, valorCalculado);
            }

            produto.Beneficiamentos = beneficiamentos;
            return valorAplicado;
        }

        private decimal CalcularPercentualTotalAplicar(decimal totalAtual, decimal valorAplicar)
        {
            return totalAtual > 0
                ? valorAplicar / totalAtual * 100
                : 100;
        }

        private void Remover(GDASession sessao, IEnumerable<IProdutoCalculo> produtos,
            Action<IProdutoCalculo> acoesAdicionais)
        {
            foreach (var produto in produtos)
            {
                CalcularTotalBrutoProduto(sessao, produto);
                RemoverBeneficiamentos(produto);
                RemoverProduto(produto);
                CalcularTotalBrutoProduto(sessao, produto);
                acoesAdicionais(produto);
            }
        }

        private decimal CalcularTotalAtual(GDASession sessao, IEnumerable<IProdutoCalculo> produtos)
        {
            decimal totalAtual = 0;

            foreach (var produto in produtos)
            {
                CalcularTotalBrutoProduto(sessao, produto);
                totalAtual += BaseCalculoTotalProduto(produto);
                totalAtual += CalcularTotalBeneficiamentosProduto(produto);
            }

            return totalAtual;
        }

        protected virtual decimal CalcularTotalBeneficiamentosProduto(IProdutoCalculo produto)
        {
            decimal totalAtual = 0;

            foreach (var beneficiamento in (produto.Beneficiamentos ?? GenericBenefCollection.Empty))
            {
                totalAtual += beneficiamento.TotalBruto;
            }

            return totalAtual;
        }

        private decimal Aplicar(GDASession sessao, IEnumerable<IProdutoCalculo> produtos, decimal percentualAplicar)
        {
            decimal valorAplicado = 0;

            foreach (var produto in produtos)
            {
                CalcularTotalBrutoProduto(sessao, produto);

                valorAplicado += AplicarBeneficiamentos(percentualAplicar, produto);
                valorAplicado += AplicarProduto(percentualAplicar, produto);

                RecalcularValorUnitario(sessao, produto);
            }

            return Math.Round(valorAplicado, 2);
        }

        private decimal ObterValorResidual(GDASession sessao, IEnumerable<IProdutoCalculo> produtos, TipoValor tipo, decimal valorAplicar, decimal valorAplicado)
        {
            decimal totalAtual = 0;
            decimal valor = 0;

            foreach (var produto in produtos)
            {
                totalAtual += BaseCalculoTotalResidualProduto(produto);
                totalAtual += CalcularTotalBeneficiamentosProduto(produto);
            }

            valor = this.CalcularValorAplicar(tipo, valorAplicar, totalAtual);

            return valor - valorAplicado;
        }

        private IProdutoCalculo ObterProdutoValorResidual(IEnumerable<IProdutoCalculo> produtos)
        {
            var produtosQtde = produtos.Where(p => p.DadosProduto.DadosGrupoSubgrupo.TipoCalculo() == TipoCalculoGrupoProd.Qtd);

            if (produtosQtde.Any())
            {
                return produtosQtde
                    .OrderBy(f => f.Qtde)
                    .First();
            }
            else
            {
                return produtos
                    .OrderBy(f => f.ValorUnit)
                    .ThenBy(f => f.TotM2Calc)
                    .ThenByDescending(f => f.Qtde)
                    .First();
            }
        }

        protected virtual void AplicarValorResidual(GDASession sessao, IProdutoCalculo produto, decimal valorResidual)
        {
            if (produto != null && valorResidual != 0)
            {
                AplicarValorProduto(produto, valorResidual);
                RecalcularValorUnitario(sessao, produto);
            }
        }

        protected virtual void RemoverBeneficiamentos(IProdutoCalculo produto)
        {
            var beneficiamentos = produto.Beneficiamentos ?? GenericBenefCollection.Empty;

            foreach (var beneficiamento in beneficiamentos)
            {
                RemoverValorBeneficiamento(beneficiamento);
            }

            produto.Beneficiamentos = beneficiamentos;
        }

        private void RemoverProduto(IProdutoCalculo produto)
        {
            RemoverValorProduto(produto);
        }

        private void CalcularTotalBrutoProduto(GDASession sessao, IProdutoCalculo produto)
        {
            if (produto.TotalBruto == 0 && (produto.IdProduto == 0 || produto.Total > 0))
                ValorBruto.Instance.Calcular(sessao, produto.Container, produto);
        }

        private void RecalcularValorUnitario(GDASession sessao, IProdutoCalculo produto)
        {
            var valorUnitario = Calculos.ValorUnitario.Instance.CalcularValor(
                sessao,
                produto.Container,
                produto,
                produto.Total
            );

            if (valorUnitario.HasValue)
                produto.ValorUnit = valorUnitario.Value;
        }
    }
}
