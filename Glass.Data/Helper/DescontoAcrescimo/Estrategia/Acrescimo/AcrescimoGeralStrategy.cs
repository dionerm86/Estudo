using System;
using Glass.Data.Model;

namespace Glass.Data.Helper.DescontoAcrescimo.Estrategia.Acrescimo
{
    class AcrescimoGeralStrategy : BaseStrategy<AcrescimoGeralStrategy>
    {
        private AcrescimoGeralStrategy() { }

        protected override void PrepararProdutoParaAlteracao(IProdutoDescontoAcrescimo produto)
        {
            produto.RemoverDescontoQtde = true;
        }

        protected override void AplicarValorBeneficiamento(GenericBenef beneficiamento, decimal valor)
        {
            beneficiamento.ValorAcrescimo += valor;
            beneficiamento.Valor += valor;
        }

        protected override void RemoverValorBeneficiamento(GenericBenef beneficiamento)
        {
            beneficiamento.Valor -= beneficiamento.ValorAcrescimo;
            beneficiamento.ValorAcrescimo = 0;
        }

        protected override void AplicarValorProduto(IProdutoDescontoAcrescimo produto, decimal valor)
        {
            produto.ValorAcrescimo += valor;
            produto.Total += valor;
        }

        protected override void RemoverValorProduto(IProdutoDescontoAcrescimo produto)
        {
            produto.Total -= produto.ValorAcrescimo;
            produto.ValorAcrescimo = 0;
        }
    }
}
