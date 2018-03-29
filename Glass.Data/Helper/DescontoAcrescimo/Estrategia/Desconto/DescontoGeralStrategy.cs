using System;
using Glass.Data.Model;

namespace Glass.Data.Helper.DescontoAcrescimo.Estrategia.Desconto
{
    class DescontoGeralStrategy : BaseStrategy<DescontoGeralStrategy>
    {
        private DescontoGeralStrategy() { }

        protected override void PrepararProdutoParaAlteracao(IProdutoDescontoAcrescimo produto)
        {
            produto.RemoverDescontoQtde = true;
        }

        protected override void AplicaValorBeneficiamento(GenericBenef beneficiamento, decimal valor)
        {
            beneficiamento.ValorDesconto += valor;
            beneficiamento.Valor -= valor;
        }

        protected override void RemoveValorBeneficiamento(GenericBenef beneficiamento)
        {
            beneficiamento.Valor += beneficiamento.ValorDesconto;
            beneficiamento.ValorDesconto = 0;
        }

        protected override void AplicaValorProduto(IProdutoDescontoAcrescimo produto, decimal valor)
        {
            produto.ValorDesconto += valor;
            produto.Total -= valor;
        }

        protected override void RemoveValorProduto(IProdutoDescontoAcrescimo produto)
        {
            produto.Total += produto.ValorDesconto;
            produto.ValorDesconto = 0;
        }
    }
}
