using System;
using Glass.Data.Model;

namespace Glass.Data.Helper.Calculos.Estrategia.DescontoAcrescimo.Desconto
{
    class DescontoGeralStrategy : BaseStrategy<DescontoGeralStrategy>
    {
        private DescontoGeralStrategy() { }

        protected override void AplicarValorBeneficiamento(GenericBenef beneficiamento, decimal valor)
        {
            beneficiamento.ValorDesconto += valor;
            beneficiamento.Valor -= valor;
        }

        protected override void RemoverValorBeneficiamento(GenericBenef beneficiamento)
        {
            beneficiamento.Valor += beneficiamento.ValorDesconto;
            beneficiamento.ValorDesconto = 0;
        }

        protected override void AplicarValorProduto(IProdutoDescontoAcrescimo produto, decimal valor)
        {
            produto.ValorDesconto += valor;
            produto.Total -= valor;
        }

        protected override void RemoverValorProduto(IProdutoDescontoAcrescimo produto)
        {
            produto.Total += produto.ValorDesconto;
            produto.ValorDesconto = 0;
        }
    }
}
