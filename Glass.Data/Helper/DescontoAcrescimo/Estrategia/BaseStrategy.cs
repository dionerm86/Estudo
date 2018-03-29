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

            Remover(produtos, produto => { });

            decimal valor = CalculaValorTotalAplicar(tipo, valorAplicar, container);
            decimal percentualAplicar = CalculaPercentualTotalAplicar(container.TotalDesejado, valor);

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
                produto => RecalcularValorUnitario(container, produto)
            );

            return true;
        }

        protected abstract void PrepararProdutoParaAlteracao(IProdutoDescontoAcrescimo produto);

        protected abstract void AplicaValorBeneficiamento(GenericBenef beneficiamento, decimal valor);

        protected abstract void RemoveValorBeneficiamento(GenericBenef beneficiamento);

        protected abstract void AplicaValorProduto(IProdutoDescontoAcrescimo produto, decimal valor);

        protected abstract void RemoveValorProduto(IProdutoDescontoAcrescimo produto);

        protected decimal CalcularTotalBrutoIndependenteCliente(IProdutoDescontoAcrescimo produto)
        {
            return produto.TotalBruto - produto.ValorDescontoCliente + produto.ValorAcrescimoCliente;
        }

        protected virtual decimal CalculaValorTotalAplicar(TipoValor tipo, decimal valorAplicar,
            IContainerDescontoAcrescimo container)
        {
            decimal valor = valorAplicar;

            if (tipo == TipoValor.Percentual)
            {
                valor = container.TotalDesejado > 0
                    ? container.TotalDesejado * (valor / 100)
                    : container.TotalAtual;
            }

            return Math.Round(valor, 2);
        }

        protected virtual decimal CalculaPercentualTotalAplicar(decimal totalDesejado, decimal valor)
        {
            return totalDesejado > 0
                ? valor / totalDesejado * 100
                : 100;
        }

        protected virtual decimal AplicarBeneficiamentos(decimal percentual, IProdutoDescontoAcrescimo produto)
        {
            decimal valorAplicado = 0;

            foreach (GenericBenef beneficiamento in produto.Beneficiamentos)
            {
                decimal valorCalculado = Math.Round(percentual / 100 * beneficiamento.TotalBruto, 2);
                valorAplicado += valorCalculado;

                AplicaValorBeneficiamento(beneficiamento, valorCalculado);
            }

            return valorAplicado;
        }

        protected virtual decimal AplicarProduto(decimal percentual, IProdutoDescontoAcrescimo produto)
        {
            decimal valorCalculado = Math.Round(percentual / 100 * CalcularTotalBrutoIndependenteCliente(produto), 2);
            AplicaValorProduto(produto, valorCalculado);

            return valorCalculado;
        }

        private decimal Aplicar(IEnumerable<IProdutoDescontoAcrescimo> produtos, IContainerDescontoAcrescimo container, decimal percentualAplicar)
        {
            decimal valorAplicado = 0;

            foreach (IProdutoDescontoAcrescimo produto in produtos)
            {
                PrepararProdutoParaAlteracao(produto);
                CalcularTotalBrutoProduto(produto);

                valorAplicado += AplicarBeneficiamentos(percentualAplicar, produto);
                valorAplicado += AplicarProduto(percentualAplicar, produto);

                RecalcularValorUnitario(container, produto);
            }

            return Math.Round(valorAplicado, 2);
        }

        private void AplicarValorResidual(IContainerDescontoAcrescimo container, IProdutoDescontoAcrescimo produto, decimal valorResidual)
        {
            if (produto != null && valorResidual != 0)
            {
                AplicaValorProduto(produto, valorResidual);
                RecalcularValorUnitario(container, produto);
            }
        }

        private void Remover(IEnumerable<IProdutoDescontoAcrescimo> produtos, Action<IProdutoDescontoAcrescimo> acao)
        {
            foreach (IProdutoDescontoAcrescimo produto in produtos)
            {
                PrepararProdutoParaAlteracao(produto);
                CalcularTotalBrutoProduto(produto);
                RemoverBeneficiamentos(produto);
                RemoverProduto(produto);
                acao(produto);
            }
        }

        private void RemoverBeneficiamentos(IProdutoDescontoAcrescimo produto)
        {
            foreach (GenericBenef beneficiamento in produto.Beneficiamentos)
            {
                RemoveValorBeneficiamento(beneficiamento);
            }
        }

        private void RemoverProduto(IProdutoDescontoAcrescimo produto)
        {
            RemoveValorProduto(produto);
        }

        private void CalcularTotalBrutoProduto(IProdutoDescontoAcrescimo prod)
        {
            if (prod.TotalBruto == 0 && (prod.IdProduto == 0 || prod.Total > 0))
                CalculaValorBruto(sessao, prod);
        }

        private void RecalcularValorUnitario(IContainerDescontoAcrescimo container, IProdutoDescontoAcrescimo produto)
        {
            if (produto.IdProduto > 0)
                RecalcularValorUnit(produto, container);
            else
                produto.ValorUnit = produto.Total / (decimal)(produto.Qtde > 0 ? produto.Qtde : 1);
        }
    }
}
