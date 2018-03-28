using System;
using System.Collections.Generic;
using System.Linq;
using Glass.Data.Model;
using Glass.Data.Helper.DescontoAcrescimo.Estrategia.Enum;

namespace Glass.Data.Helper.DescontoAcrescimo.Estrategia
{
    abstract class BaseStrategy : ICalculoStrategy
    {
        public bool Calcular(TipoValor tipo, decimal valorAplicar, decimal totalAtual, decimal totalDesejado,
            IEnumerable<IProdutoDescontoAcrescimo> produtos, IContainerDescontoAcrescimo container)
        {
            if (valorAplicar == 0 || produtos == null || !produtos.Any())
                return false;

            decimal valorAplicado = 0;
            IProdutoDescontoAcrescimo ultimoProduto = produtos.Last();

            decimal valor = CalculaValorTotalAplicar(tipo, valorAplicar, totalAtual, totalDesejado);
            decimal percentualAplicar = CalculaPercentualTotalAplicar(totalDesejado, valor);
            valor = Math.Round(valor, 2);

            foreach (IProdutoDescontoAcrescimo produto in produtos)
            {
                PrepararProdutoParaAlteracao(produto);
                CalcularTotalBrutoProduto(produto);
                AplicarBeneficiamentos(ref valorAplicado, percentualAplicar, produto);
                AplicarProduto(ref valorAplicado, percentualAplicar, produto);
                RecalcularValorUnitario(container, produto);
            }

            if (ultimoProduto != null)
            {
                AtualizaValorProduto(ultimoProduto, valor - valorAplicado);
                RecalcularValorUnitario(container, ultimoProduto);
            }

            return true;
        }

        protected abstract void PrepararProdutoParaAlteracao(IProdutoDescontoAcrescimo produto);

        protected abstract void AtualizaValorBeneficiamento(GenericBenef beneficiamento, decimal valor);

        protected abstract void AtualizaValorProduto(IProdutoDescontoAcrescimo produto, decimal valor);

        private decimal CalculaPercentualTotalAplicar(decimal totalDesejado, decimal valor)
        {
            return totalDesejado > 0
                ? valor / totalDesejado * 100
                : 100;
        }

        private decimal CalculaValorTotalAplicar(TipoValor tipo, decimal valorAplicar, decimal totalAtual, decimal totalDesejado)
        {
            decimal valor = valorAplicar;

            if (tipo == TipoValor.Percentual)
            {
                valor = totalDesejado > 0
                    ? totalDesejado * (valor / 100)
                    : totalAtual;
            }

            return valor;
        }

        private void CalcularTotalBrutoProduto(IProdutoDescontoAcrescimo prod)
        {
            if (prod.TotalBruto == 0 && (prod.IdProduto == 0 || prod.Total > 0))
                CalculaValorBruto(sessao, prod);
        }

        private void AplicarBeneficiamentos(ref decimal valorAplicado, decimal percentual, IProdutoDescontoAcrescimo produto)
        {
            decimal valorCalculado;
            GenericBenefCollection listaBeneficiamentos = produto.Beneficiamentos;

            foreach (GenericBenef beneficiamento in listaBeneficiamentos)
            {
                valorCalculado = Math.Round(percentual / 100 * beneficiamento.TotalBruto, 2);
                valorAplicado += valorCalculado;

                AtualizaValorBeneficiamento(beneficiamento, valorCalculado);
            }

            produto.Beneficiamentos = listaBeneficiamentos;
        }

        private void AplicarProduto(ref decimal valorAplicado, decimal percentual, IProdutoDescontoAcrescimo produto)
        {
            decimal valorCalculado = Math.Round(percentual / 100 * CalcularTotalBrutoIndependenteCliente(produto), 2);
            valorAplicado += valorCalculado;

            AtualizaValorProduto(produto, valorCalculado);
        }

        private decimal CalcularTotalBrutoIndependenteCliente(IProdutoDescontoAcrescimo produto)
        {
            return produto.TotalBruto - produto.ValorDescontoCliente + produto.ValorAcrescimoCliente;
        }

        private void RecalcularValorUnitario(IContainerDescontoAcrescimo container, IProdutoDescontoAcrescimo prod)
        {
            if (prod.IdProduto > 0)
                RecalcularValorUnit(prod, container);
            else
                prod.ValorUnit = prod.Total / (decimal)(prod.Qtde > 0 ? prod.Qtde : 1);
        }
    }
}
