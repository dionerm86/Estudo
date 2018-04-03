using System;
using Glass.Data.Model;

namespace Glass.Data.Helper.Calculos.Estrategia.DescontoAcrescimo.Acrescimo
{
    class AcrescimoAmbienteStrategy : BaseStrategy<AcrescimoAmbienteStrategy>
    {
        private AcrescimoAmbienteStrategy() { }

        protected override void AplicarValorBeneficiamento(GenericBenef beneficiamento, decimal valor)
        {
            beneficiamento.ValorAcrescimoProd += valor;
            beneficiamento.Valor += valor;
        }

        protected override void RemoverValorBeneficiamento(GenericBenef beneficiamento)
        {
            beneficiamento.Valor -= beneficiamento.ValorAcrescimoProd;
            beneficiamento.ValorAcrescimoProd = 0;
        }

        protected override void AplicarValorProduto(IProdutoCalculo produto, decimal valor)
        {
            produto.ValorAcrescimoProd += valor;
            produto.Total += valor;
        }

        protected override void RemoverValorProduto(IProdutoCalculo produto)
        {
            produto.Total -= produto.ValorAcrescimoProd;
            produto.ValorAcrescimoProd = 0;
        }
    }
}
