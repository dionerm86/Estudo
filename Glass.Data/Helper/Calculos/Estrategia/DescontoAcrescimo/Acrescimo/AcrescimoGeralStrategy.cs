﻿using System;
using Glass.Data.Model;

namespace Glass.Data.Helper.Calculos.Estrategia.DescontoAcrescimo.Acrescimo
{
    class AcrescimoGeralStrategy : BaseAcrescimoStrategy<AcrescimoGeralStrategy>
    {
        private AcrescimoGeralStrategy() { }

        protected override Func<IProdutoCalculo, bool> FiltrarParaRemocao()
        {
            return produto => produto.ValorAcrescimo > 0;
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

        protected override void AplicarValorProduto(IProdutoCalculo produto, decimal valor)
        {
            produto.ValorAcrescimo += valor;
            produto.Total += valor;
        }

        protected override void RemoverValorProduto(IProdutoCalculo produto)
        {
            produto.Total -= produto.ValorAcrescimo;
            produto.ValorAcrescimo = 0;
        }
    }
}
