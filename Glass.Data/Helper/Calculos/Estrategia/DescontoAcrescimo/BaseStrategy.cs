using System;
using System.Collections.Generic;
using System.Linq;
using Glass.Data.Model;
using Glass.Data.Helper.Calculos.Estrategia.DescontoAcrescimo.Enum;
using Glass.Pool;

namespace Glass.Data.Helper.Calculos.Estrategia.DescontoAcrescimo
{
    abstract class BaseStrategy<T> : Singleton<T>, IDescontoAcrescimoStrategy
        where T : BaseStrategy<T>
    {
        public bool Aplicar(TipoValor tipo, decimal valorAplicar, IEnumerable<IProdutoCalculo> produtos,
            IContainerCalculo container)
        {
            if (valorAplicar == 0 || !produtos.Any() || !PermiteAplicarOuRemover())
                return false;

            Remover(produtos, container);

            decimal totalAtual = CalcularTotalAtual(produtos, container);
            decimal totalDesejado = CalcularTotalDesejado(tipo, valorAplicar, totalAtual);
            decimal valor = Math.Abs(totalDesejado - totalAtual);
            decimal percentualAplicar = CalcularPercentualTotalAplicar(totalAtual, valor);

            decimal valorAplicado = Aplicar(produtos, container, percentualAplicar);

            IProdutoCalculo produtoValorResidual = produtos.Last();
            AplicarValorResidual(container, produtoValorResidual, valor - valorAplicado);

            return true;
        }

        public bool Remover(IEnumerable<IProdutoCalculo> produtos, IContainerCalculo container)
        {
            if (produtos == null || !produtos.Any() || !PermiteAplicarOuRemover())
                return false;

            foreach (var produto in produtos)
            {
                CalcularTotalBrutoProduto(produto, container);
                RemoverBeneficiamentos(produto);
                RemoverProduto(produto);
            }

            return true;
        }

        protected abstract decimal CalcularTotalDesejado(TipoValor tipo, decimal valorAplicar, decimal totalAtual);

        protected abstract void AplicarValorBeneficiamento(GenericBenef beneficiamento, decimal valor);

        protected abstract void RemoverValorBeneficiamento(GenericBenef beneficiamento);

        protected abstract void AplicarValorProduto(IProdutoCalculo produto, decimal valor);

        protected abstract void RemoverValorProduto(IProdutoCalculo produto);

        protected virtual bool PermiteAplicarOuRemover()
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
            }

            return Math.Round(valorAplicado, 2);
        }

        private void AplicarValorResidual(IContainerCalculo container, IProdutoCalculo produto,
            decimal valorResidual)
        {
            if (produto != null && valorResidual != 0)
            {
                AplicarValorProduto(produto, valorResidual);
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
    }
}
