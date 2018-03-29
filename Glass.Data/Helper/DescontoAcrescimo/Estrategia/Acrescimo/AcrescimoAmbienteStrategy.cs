using System;
using Glass.Data.Model;

namespace Glass.Data.Helper.DescontoAcrescimo.Estrategia.Acrescimo
{
    class AcrescimoAmbienteStrategy : BaseStrategy<AcrescimoAmbienteStrategy>
    {
        private AcrescimoAmbienteStrategy() { }

        protected override bool PermitirExecucao()
        {
            return true;
        }

        protected override void PrepararProdutoParaAlteracao(IProdutoDescontoAcrescimo produto)
        {
            produto.RemoverDescontoQtde = true;
        }

        protected override void AplicaValorBeneficiamento(GenericBenef beneficiamento, decimal valor)
        {
            beneficiamento.ValorAcrescimoProd = valor;
            beneficiamento.Valor += valor;
        }

        protected override void RemoveValorBeneficiamento(GenericBenef beneficiamento)
        {
            beneficiamento.Valor -= beneficiamento.ValorAcrescimoProd;
            beneficiamento.ValorAcrescimoProd = 0;
        }

        protected override void AplicaValorProduto(IProdutoDescontoAcrescimo produto, decimal valor)
        {
            produto.ValorAcrescimoProd += valor;
            produto.Total += valor;
        }

        protected override void RemoveValorProduto(IProdutoDescontoAcrescimo produto)
        {
            produto.Total -= produto.ValorAcrescimoProd;
            produto.ValorAcrescimoProd = 0;
        }
    }
}
