using System;
using System.Collections.Generic;
using System.Linq;
using Glass.Data.Model;
using Glass.Data.Helper.Calculos.Estrategia.DescontoAcrescimo.Enum;
using Glass.Pool;
using GDA;

namespace Glass.Data.Helper.Calculos.Estrategia.DescontoAcrescimo
{
    abstract class BaseStrategy<T> : Singleton<T>, IDescontoAcrescimoStrategy
        where T : BaseStrategy<T>
    {
        public bool Aplicar(GDASession sessao, TipoValor tipo, decimal valorAplicar, IEnumerable<IProdutoCalculo> produtos)
        {
            if (valorAplicar == 0 || !produtos.Any() || !PermiteAplicarOuRemover(produtos))
                return false;

            Remover(sessao, produtos);

            decimal totalAtual = CalcularTotalAtual(sessao, produtos);
            decimal totalDesejado = CalcularTotalDesejado(tipo, valorAplicar, totalAtual);
            decimal valor = Math.Abs(totalDesejado - totalAtual);
            decimal percentualAplicar = CalcularPercentualTotalAplicar(totalAtual, valor);

            decimal valorAplicado = Aplicar(sessao, produtos, percentualAplicar);

            IProdutoCalculo produtoValorResidual = produtos.Last();
            AplicarValorResidual(sessao, produtoValorResidual, valor - valorAplicado);

            return true;
        }

        public bool Remover(GDASession sessao, IEnumerable<IProdutoCalculo> produtos)
        {
            if (produtos == null || !produtos.Any() || !PermiteAplicarOuRemover(produtos))
                return false;

            foreach (var produto in produtos)
            {
                CalcularTotalBrutoProduto(sessao, produto);
                RemoverBeneficiamentos(produto);
                RemoverProduto(produto);
                RecalcularValorUnitario(sessao, produto);
            }

            return true;
        }

        protected abstract decimal CalcularTotalDesejado(TipoValor tipo, decimal valorAplicar, decimal totalAtual);

        protected abstract void AplicarValorBeneficiamento(GenericBenef beneficiamento, decimal valor);

        protected abstract void RemoverValorBeneficiamento(GenericBenef beneficiamento);

        protected abstract void AplicarValorProduto(IProdutoCalculo produto, decimal valor);

        protected abstract void RemoverValorProduto(IProdutoCalculo produto);

        protected virtual bool PermiteAplicarOuRemover(IEnumerable<IProdutoCalculo> produtos)
        {
            return true;
        }

        protected decimal CalcularTotalBrutoDependenteCliente(IProdutoCalculo produto)
        {
            return produto.TotalBruto - produto.ValorDescontoCliente + produto.ValorAcrescimoCliente;
        }

        protected virtual decimal CalcularPercentualTotalAplicar(decimal totalAtual, decimal valorAplicar)
        {
            return totalAtual > 0
                ? valorAplicar / totalAtual * 100
                : 100;
        }

        protected virtual decimal AplicarBeneficiamentos(decimal percentual, IProdutoCalculo produto)
        {
            decimal valorAplicado = 0;

            foreach (var beneficiamento in (produto.Beneficiamentos ?? GenericBenefCollection.Empty))
            {
                decimal valorCalculado = Math.Round(percentual / 100 * beneficiamento.TotalBruto, 2);
                valorAplicado += valorCalculado;

                AplicarValorBeneficiamento(beneficiamento, valorCalculado);
            }

            return valorAplicado;
        }

        protected virtual decimal AplicarProduto(decimal percentual, IProdutoCalculo produto)
        {
            decimal valorCalculado = Math.Round(percentual / 100 * CalcularTotalBrutoDependenteCliente(produto), 2);
            AplicarValorProduto(produto, valorCalculado);

            return valorCalculado;
        }

        private decimal CalcularTotalAtual(GDASession sessao, IEnumerable<IProdutoCalculo> produtos)
        {
            decimal totalAtual = 0;

            foreach (var produto in produtos)
            {
                CalcularTotalBrutoProduto(sessao, produto);
                totalAtual += CalcularTotalBrutoDependenteCliente(produto);
                totalAtual += CalcularTotalBeneficiamentosProduto(produto);
            }

            return totalAtual;
        }

        private decimal CalcularTotalBeneficiamentosProduto(IProdutoCalculo produto)
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

        private void AplicarValorResidual(GDASession sessao, IProdutoCalculo produto, decimal valorResidual)
        {
            if (produto != null && valorResidual != 0)
            {
                AplicarValorProduto(produto, valorResidual);
                RecalcularValorUnitario(sessao, produto);
            }
        }

        private void RemoverBeneficiamentos(IProdutoCalculo produto)
        {
            foreach (var beneficiamento in (produto.Beneficiamentos ?? GenericBenefCollection.Empty))
            {
                RemoverValorBeneficiamento(beneficiamento);
            }
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
