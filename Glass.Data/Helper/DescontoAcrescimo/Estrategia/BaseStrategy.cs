using System;
using System.Collections.Generic;
using System.Linq;
using Glass.Data.Model;
using Glass.Data.Helper.DescontoAcrescimo.Estrategia.Enum;
using Glass.Pool;

namespace Glass.Data.Helper.DescontoAcrescimo.Estrategia
{
    abstract class BaseStrategy<T> : PoolableObject<T>, ICalculoStrategy
        where T : BaseStrategy<T>
    {
        public bool Aplicar(TipoValor tipo, decimal valorAplicar, IEnumerable<IProdutoDescontoAcrescimo> produtos,
            IContainerDescontoAcrescimo container)
        {
            if (valorAplicar == 0 || produtos == null || !produtos.Any() || container == null)
                return false;

            Remover(
                produtos,
                container,
                produto => { }
            );

            decimal totalAtual = CalcularTotalAtual(produtos, container);
            decimal totalDesejado = CalcularTotalDesejado(tipo, valorAplicar, totalAtual);
            decimal valor = totalDesejado - totalAtual;
            decimal percentualAplicar = CalcularPercentualTotalAplicar(totalAtual, valor);

            decimal valorAplicado = Aplicar(produtos, container, percentualAplicar);

            IProdutoDescontoAcrescimo produtoValorResidual = produtos.Last();
            AplicarValorResidual(container, produtoValorResidual, valor - valorAplicado);

            return true;
        }

        public bool Remover(IEnumerable<IProdutoDescontoAcrescimo> produtos, IContainerDescontoAcrescimo container)
        {
            if (produtos == null || !produtos.Any() || container == null)
                return false;

            Remover(
                produtos,
                container,
                produto => RecalcularValorUnitario(container, produto)
            );

            return true;
        }

        protected abstract void PrepararProdutoParaAlteracao(IProdutoDescontoAcrescimo produto);

        protected abstract void AplicarValorBeneficiamento(GenericBenef beneficiamento, decimal valor);

        protected abstract void RemoverValorBeneficiamento(GenericBenef beneficiamento);

        protected abstract void AplicarValorProduto(IProdutoDescontoAcrescimo produto, decimal valor);

        protected abstract void RemoverValorProduto(IProdutoDescontoAcrescimo produto);

        protected decimal CalcularTotalBrutoIndependenteCliente(IProdutoDescontoAcrescimo produto)
        {
            return produto.TotalBruto - produto.ValorDescontoCliente + produto.ValorAcrescimoCliente;
        }

        protected virtual decimal CalcularTotalDesejado(TipoValor tipo, decimal valorAplicar, decimal totalAtual)
        {
            decimal totalAplicar = valorAplicar;

            if (tipo == TipoValor.Percentual)
            {
                totalAplicar = totalAtual * valorAplicar / 100;
            }

            return totalAtual + Math.Round(totalAplicar, 2);
        }

        protected virtual decimal CalcularPercentualTotalAplicar(decimal totalAtual, decimal valorAplicar)
        {
            return totalAtual > 0
                ? valorAplicar / totalAtual * 100
                : 100;
        }

        protected virtual decimal AplicarBeneficiamentos(decimal percentual, IProdutoDescontoAcrescimo produto)
        {
            decimal valorAplicado = 0;

            foreach (var beneficiamento in produto.Beneficiamentos)
            {
                decimal valorCalculado = Math.Round(percentual / 100 * beneficiamento.TotalBruto, 2);
                valorAplicado += valorCalculado;

                AplicarValorBeneficiamento(beneficiamento, valorCalculado);
            }

            return valorAplicado;
        }

        protected virtual decimal AplicarProduto(decimal percentual, IProdutoDescontoAcrescimo produto)
        {
            decimal valorCalculado = Math.Round(percentual / 100 * CalcularTotalBrutoIndependenteCliente(produto), 2);
            AplicarValorProduto(produto, valorCalculado);

            return valorCalculado;
        }

        private decimal CalcularTotalAtual(IEnumerable<IProdutoDescontoAcrescimo> produtos, IContainerDescontoAcrescimo container)
        {
            decimal totalAtual = 0;

            foreach (var produto in produtos)
            {
                CalcularTotalBrutoProduto(produto, container);
                totalAtual += CalcularTotalBrutoIndependenteCliente(produto);
                totalAtual += CalcularTotalBeneficiamentosProduto(produto);
            }

            return totalAtual;
        }

        private decimal CalcularTotalBeneficiamentosProduto(IProdutoDescontoAcrescimo produto)
        {
            decimal totalAtual = 0;

            foreach (var beneficiamento in produto.Beneficiamentos)
            {
                totalAtual += beneficiamento.TotalBruto;
            }

            return totalAtual;
        }

        private decimal Aplicar(IEnumerable<IProdutoDescontoAcrescimo> produtos, IContainerDescontoAcrescimo container,
            decimal percentualAplicar)
        {
            decimal valorAplicado = 0;

            foreach (var produto in produtos)
            {
                PrepararProdutoParaAlteracao(produto);
                CalcularTotalBrutoProduto(produto, container);

                valorAplicado += AplicarBeneficiamentos(percentualAplicar, produto);
                valorAplicado += AplicarProduto(percentualAplicar, produto);

                RecalcularValorUnitario(container, produto);
            }

            return Math.Round(valorAplicado, 2);
        }

        private void AplicarValorResidual(IContainerDescontoAcrescimo container, IProdutoDescontoAcrescimo produto,
            decimal valorResidual)
        {
            if (produto != null && valorResidual != 0)
            {
                AplicarValorProduto(produto, valorResidual);
                RecalcularValorUnitario(container, produto);
            }
        }

        private void Remover(IEnumerable<IProdutoDescontoAcrescimo> produtos, IContainerDescontoAcrescimo container,
            Action<IProdutoDescontoAcrescimo> acao)
        {
            foreach (var produto in produtos)
            {
                PrepararProdutoParaAlteracao(produto);
                CalcularTotalBrutoProduto(produto, container);
                RemoverBeneficiamentos(produto);
                RemoverProduto(produto);
                acao(produto);
            }
        }

        private void RemoverBeneficiamentos(IProdutoDescontoAcrescimo produto)
        {
            foreach (var beneficiamento in produto.Beneficiamentos)
            {
                RemoverValorBeneficiamento(beneficiamento);
            }
        }

        private void RemoverProduto(IProdutoDescontoAcrescimo produto)
        {
            RemoverValorProduto(produto);
        }

        private void CalcularTotalBrutoProduto(IProdutoDescontoAcrescimo produto, IContainerDescontoAcrescimo container)
        {
            if (produto.TotalBruto == 0 && (produto.IdProduto == 0 || produto.Total > 0))
                ValorBruto.Instance.Calcular(produto, container);
        }

        private void RecalcularValorUnitario(IContainerDescontoAcrescimo container, IProdutoDescontoAcrescimo produto)
        {
            if (produto.IdProduto > 0)
                ValorUnitario.Instance.Calcular(produto, container, false);
            else
                produto.ValorUnit = produto.Total / (decimal)(produto.Qtde > 0 ? produto.Qtde : 1);
        }
    }
}
