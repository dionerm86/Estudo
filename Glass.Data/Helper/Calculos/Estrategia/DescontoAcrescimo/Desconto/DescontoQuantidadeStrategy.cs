using System;
using System.Collections.Generic;
using System.Linq;
using Glass.Data.Model;

namespace Glass.Data.Helper.Calculos.Estrategia.DescontoAcrescimo.Desconto
{
    class DescontoQuantidadeStrategy : BaseDescontoStrategy<DescontoQuantidadeStrategy>
    {
        private DescontoQuantidadeStrategy() { }

        protected override bool PermitirRemocaoCalculoProduto(IProdutoCalculo produto)
        {
            return produto.ValorDescontoQtde > 0;
        }

        protected override void AplicarValorBeneficiamento(GenericBenef beneficiamento, decimal valor)
        {
            // não aplica
        }

        protected override void RemoverValorBeneficiamento(GenericBenef beneficiamento)
        {
            // não remove
        }

        protected override decimal AplicarProduto(decimal percentual, IProdutoCalculo produto)
        {
            var baseCalculo = BaseCalculoTotalProduto(produto);
            percentual = (decimal)produto.PercDescontoQtde;

            decimal valorCalculado = Math.Round(baseCalculo * percentual, 2);

            AplicarValorProduto(produto, valorCalculado);
            return valorCalculado;
        }

        protected override void AplicarValorProduto(IProdutoCalculo produto, decimal valor)
        {
            produto.ValorDescontoQtde += valor;
            produto.Total -= valor;
        }

        protected override void RemoverValorProduto(IProdutoCalculo produto)
        {
            produto.Total += produto.ValorDescontoQtde;
            produto.ValorDescontoQtde = 0;
        }
    }
}
