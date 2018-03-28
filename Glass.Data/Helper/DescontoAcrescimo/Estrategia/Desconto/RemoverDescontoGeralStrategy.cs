using Glass.Data.Model;

namespace Glass.Data.Helper.DescontoAcrescimo.Estrategia.Desconto
{
    class RemoverDescontoGeralStrategy : BaseStrategy
    {
        protected override void AtualizaValorBeneficiamento(GenericBenef beneficiamento, decimal valor)
        {
            valor = beneficiamento.ValorDesconto;
            beneficiamento.ValorDesconto += valor;
            beneficiamento.Valor += valor;
        }

        protected override void AtualizaValorProduto(IProdutoDescontoAcrescimo produto, decimal valor)
        {
            valor = produto.ValorDesconto;
            produto.ValorDesconto += valor;
            produto.Total += valor;
        }

        protected override void PrepararProdutoParaAlteracao(IProdutoDescontoAcrescimo produto)
        {
            produto.RemoverDescontoQtde = true;
        }
    }
}
