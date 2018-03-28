using Glass.Data.Model;

namespace Glass.Data.Helper.DescontoAcrescimo.Estrategia.Desconto
{
    class RemoverDescontoAmbienteStrategy : BaseStrategy
    {
        protected override void AtualizaValorBeneficiamento(GenericBenef beneficiamento, decimal valor)
        {
            valor = beneficiamento.ValorDescontoProd;
            beneficiamento.ValorDescontoProd += valor;
            beneficiamento.Valor += valor;
        }

        protected override void AtualizaValorProduto(IProdutoDescontoAcrescimo produto, decimal valor)
        {
            valor = produto.ValorDescontoProd;
            produto.ValorDescontoProd += valor;
            produto.Total += valor;
        }

        protected override void PrepararProdutoParaAlteracao(IProdutoDescontoAcrescimo produto)
        {
            produto.RemoverDescontoQtde = true;
        }
    }
}
