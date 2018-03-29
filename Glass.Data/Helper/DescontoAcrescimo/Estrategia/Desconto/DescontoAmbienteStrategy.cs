using System;
using Glass.Data.Model;

namespace Glass.Data.Helper.DescontoAcrescimo.Estrategia.Desconto
{
    class DescontoAmbienteStrategy : BaseStrategy<DescontoAmbienteStrategy>
    {
        private DescontoAmbienteStrategy() { }

        protected override void PrepararProdutoParaAlteracao(IProdutoDescontoAcrescimo produto)
        {
            produto.RemoverDescontoQtde = true;
        }

        protected override void AplicaValorBeneficiamento(GenericBenef beneficiamento, decimal valor)
        {
            beneficiamento.ValorDescontoProd += valor;
            beneficiamento.Valor -= valor;
        }

        protected override void RemoveValorBeneficiamento(GenericBenef beneficiamento)
        {
            beneficiamento.Valor += beneficiamento.ValorDescontoProd;
            beneficiamento.ValorDescontoProd = 0;
        }

        protected override void AplicaValorProduto(IProdutoDescontoAcrescimo produto, decimal valor)
        {
            produto.ValorDescontoProd += valor;
            produto.Total -= valor;
        }

        protected override void RemoveValorProduto(IProdutoDescontoAcrescimo produto)
        {
            produto.Total += produto.ValorDescontoProd;
            produto.ValorDescontoProd = 0;
        }
    }
}
