using System;
using System.Collections.Generic;
using System.Linq;
using Glass.Data.Model;
using Glass.Data.Helper.Calculos.Estrategia.DescontoAcrescimo.Enum;
using Glass.Pool;

namespace Glass.Data.Helper.Calculos.Estrategia.DescontoAcrescimo
{
    abstract class BaseStrategy<T> : PoolableObject<T>, IDescontoAcrescimoStrategy
        where T : BaseStrategy<T>
    {
        public bool Aplicar(TipoValor tipo, decimal valorAplicar, IEnumerable<IProdutoCalculo> produtos,
            IContainerCalculo container)
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

            IProdutoCalculo produtoValorResidual = produtos.Last();
            AplicarValorResidual(container, produtoValorResidual, valor - valorAplicado);

            return true;
        }

        public bool Remover(IEnumerable<IProdutoCalculo> produtos, IContainerCalculo container)
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

        protected abstract void AplicarValorBeneficiamento(GenericBenef beneficiamento, decimal valor);

        protected abstract void RemoverValorBeneficiamento(GenericBenef beneficiamento);

        protected abstract void AplicarValorProduto(IProdutoCalculo produto, decimal valor);

        protected abstract void RemoverValorProduto(IProdutoCalculo produto);

        protected decimal CalcularTotalBrutoDependenteCliente(IProdutoCalculo produto)
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

        protected virtual decimal AplicarBeneficiamentos(decimal percentual, IProdutoCalculo produto)
        {
            decimal valorAplicado = 0;

            foreach (var beneficiamento in (produto.Beneficiamentos ?? GenericBenefCollection.EMPTY))
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

        private decimal CalcularTotalAtual(IEnumerable<IProdutoCalculo> produtos, IContainerCalculo container)
        {
            decimal totalAtual = 0;

            foreach (var produto in produtos)
            {
                CalcularTotalBrutoProduto(produto, container);
                totalAtual += CalcularTotalBrutoDependenteCliente(produto);
                totalAtual += CalcularTotalBeneficiamentosProduto(produto);
            }

            return totalAtual;
        }

        private decimal CalcularTotalBeneficiamentosProduto(IProdutoCalculo produto)
        {
            decimal totalAtual = 0;

            foreach (var beneficiamento in (produto.Beneficiamentos ?? GenericBenefCollection.EMPTY))
            {
                totalAtual += beneficiamento.TotalBruto;
            }

            return totalAtual;
        }

        private decimal Aplicar(IEnumerable<IProdutoCalculo> produtos, IContainerCalculo container,
            decimal percentualAplicar)
        {
            decimal valorAplicado = 0;

            foreach (var produto in produtos)
            {
                CalcularTotalBrutoProduto(produto, container);

                valorAplicado += AplicarBeneficiamentos(percentualAplicar, produto);
                valorAplicado += AplicarProduto(percentualAplicar, produto);

                RecalcularValorUnitario(container, produto);
            }

            return Math.Round(valorAplicado, 2);
        }

        private void AplicarValorResidual(IContainerCalculo container, IProdutoCalculo produto,
            decimal valorResidual)
        {
            if (produto != null && valorResidual != 0)
            {
                AplicarValorProduto(produto, valorResidual);
                RecalcularValorUnitario(container, produto);
            }
        }

        private void Remover(IEnumerable<IProdutoCalculo> produtos, IContainerCalculo container,
            Action<IProdutoCalculo> acao)
        {
            foreach (var produto in produtos)
            {
                CalcularTotalBrutoProduto(produto, container);
                RemoverBeneficiamentos(produto);
                RemoverProduto(produto);
                acao(produto);
            }
        }

        private void RemoverBeneficiamentos(IProdutoCalculo produto)
        {
            foreach (var beneficiamento in (produto.Beneficiamentos ?? GenericBenefCollection.EMPTY))
            {
                RemoverValorBeneficiamento(beneficiamento);
            }
        }

        private void RemoverProduto(IProdutoCalculo produto)
        {
            RemoverValorProduto(produto);
        }

        private void CalcularTotalBrutoProduto(IProdutoCalculo produto, IContainerCalculo container)
        {
            if (produto.TotalBruto == 0 && (produto.IdProduto == 0 || produto.Total > 0))
                ValorBruto.Instance.Calcular(produto, container);
        }

        private void RecalcularValorUnitario(IContainerCalculo container, IProdutoCalculo produto)
        {
            if (produto.IdProduto > 0)
                Calculos.ValorUnitario.Instance.Calcular(produto, container, false);
            else
                produto.ValorUnit = produto.Total / (decimal)(produto.Qtde > 0 ? produto.Qtde : 1);
        }
    }
}
