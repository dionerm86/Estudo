using System;
using Glass.Data.Model;

namespace Glass.Data.Helper.DescontoAcrescimo.Estrategia.Acrescimo
{
    class AcrescimoGeralStrategy : BaseStrategy<AcrescimoGeralStrategy>
    {
        private AcrescimoGeralStrategy() { }

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
            beneficiamento.ValorAcrescimo = valor;
            beneficiamento.Valor += valor;
        }

        protected override void RemoveValorBeneficiamento(GenericBenef beneficiamento)
        {
            beneficiamento.Valor -= beneficiamento.ValorAcrescimo;
            beneficiamento.ValorAcrescimo = 0;
        }

        protected override void AplicaValorProduto(IProdutoDescontoAcrescimo produto, decimal valor)
        {
            produto.ValorAcrescimo = valor;
            produto.Total += valor;
        }

        protected override void RemoveValorProduto(IProdutoDescontoAcrescimo produto)
        {
            produto.Total -= produto.ValorAcrescimo;
            produto.ValorAcrescimo = 0;
        }
    }
}
