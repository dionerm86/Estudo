using Glass.Data.Model;

namespace Glass.Data.Helper.DescontoAcrescimo.Estrategia.Desconto
{
    class AplicarDescontoAmbienteStrategy : BaseStrategy
    {
        protected override void AtualizaValorBeneficiamento(GenericBenef beneficiamento, decimal valor)
        {
            beneficiamento.ValorDescontoProd -= valor;
            beneficiamento.Valor -= valor;
        }

        protected override void AtualizaValorProduto(IProdutoDescontoAcrescimo produto, decimal valor)
        {
            produto.ValorDescontoProd -= valor;
            produto.Total -= valor;
        }

        protected override void PrepararProdutoParaAlteracao(IProdutoDescontoAcrescimo produto)
        {
            produto.RemoverDescontoQtde = true;
        }
    }
}
