using System;
using Glass.Data.Model;

namespace Glass.Data.Helper.DescontoAcrescimo.Estrategia.Desconto
{
    class DescontoQuantidadeStrategy : BaseStrategy<DescontoQuantidadeStrategy>
    {
        private DescontoQuantidadeStrategy() { }

        protected override void PrepararProdutoParaAlteracao(IProdutoDescontoAcrescimo produto)
        {
            // não faz nada
        }

        protected override void AplicarValorBeneficiamento(GenericBenef beneficiamento, decimal valor)
        {
            // não aplica
        }

        protected override void RemoverValorBeneficiamento(GenericBenef beneficiamento)
        {
            // não remove
        }

        protected override decimal AplicarProduto(decimal percentual, IProdutoDescontoAcrescimo produto)
        {
            var baseCalculo = produto.RemoverDescontoQtde
                ? CalcularTotalBrutoIndependenteCliente(produto)
                : produto.Total;

            percentual = (decimal)produto.PercDescontoQtde / 100;
            decimal valorCalculado = Math.Round(baseCalculo * percentual, 2);

            AplicarValorProduto(produto, valorCalculado);
            return valorCalculado;
        }

        protected override void AplicarValorProduto(IProdutoDescontoAcrescimo produto, decimal valor)
        {
            produto.ValorDescontoQtde += valor;
            produto.Total -= valor;
        }

        protected override void RemoverValorProduto(IProdutoDescontoAcrescimo produto)
        {
            produto.Total += produto.ValorDescontoQtde;
            produto.ValorDescontoQtde = 0;
        }
    }
}
