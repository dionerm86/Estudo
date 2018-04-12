using System;
using Glass.Data.Model;

namespace Glass.Data.Helper.Calculos.Estrategia.DescontoAcrescimo.Comissao
{
    class ComissaoGeralStrategy : BaseComissaoStrategy<ComissaoGeralStrategy>
    {
        private ComissaoGeralStrategy() { }

        protected override Func<IProdutoCalculo, bool> FiltrarParaRemocao()
        {
            return produto => produto.ValorComissao > 0;
        }

        protected override void AplicarValorBeneficiamento(GenericBenef beneficiamento, decimal valor)
        {
            beneficiamento.ValorComissao += valor;
            beneficiamento.Valor += valor;
        }

        protected override void RemoverValorBeneficiamento(GenericBenef beneficiamento)
        {
            beneficiamento.Valor -= beneficiamento.ValorComissao;
            beneficiamento.ValorComissao = 0;
        }

        protected override void AplicarValorProduto(IProdutoCalculo produto, decimal valor)
        {
            produto.ValorComissao += valor;
            produto.Total += valor;
        }

        protected override void RemoverValorProduto(IProdutoCalculo produto)
        {
            produto.Total -= produto.ValorComissao;
            produto.ValorComissao = 0;
        }
    }
}
