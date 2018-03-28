using Glass.Data.Model;

namespace Glass.Data.Helper.DescontoAcrescimo.Estrategia.Acrescimo
{
    class RemoverAcrescimoGeralStrategy : BaseStrategy
    {
        protected override void AtualizaValorBeneficiamento(GenericBenef beneficiamento, decimal valor)
        {
            valor = beneficiamento.ValorAcrescimo;
            beneficiamento.ValorAcrescimo -= valor;
            beneficiamento.Valor -= valor;
        }

        protected override void AtualizaValorProduto(IProdutoDescontoAcrescimo produto, decimal valor)
        {
            valor = produto.ValorAcrescimo;
            produto.ValorAcrescimo -= valor;
            produto.Total -= valor;
        }

        protected override void PrepararProdutoParaAlteracao(IProdutoDescontoAcrescimo produto)
        {
            produto.RemoverDescontoQtde = true;
        }
    }
}
