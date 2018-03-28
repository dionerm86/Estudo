using Glass.Data.Model;

namespace Glass.Data.Helper.DescontoAcrescimo.Estrategia.Acrescimo
{
    class RemoverAcrescimoAmbienteStrategy : BaseStrategy
    {
        protected override void AtualizaValorBeneficiamento(GenericBenef beneficiamento, decimal valor)
        {
            valor = beneficiamento.ValorAcrescimoProd;
            beneficiamento.ValorAcrescimoProd -= valor;
            beneficiamento.Valor -= valor;
        }

        protected override void AtualizaValorProduto(IProdutoDescontoAcrescimo produto, decimal valor)
        {
            valor = produto.ValorAcrescimoProd;
            produto.ValorAcrescimoProd -= valor;
            produto.Total -= valor;
        }

        protected override void PrepararProdutoParaAlteracao(IProdutoDescontoAcrescimo produto)
        {
            produto.RemoverDescontoQtde = true;
        }
    }
}
