using Glass.Configuracoes;
using Glass.Data.Helper.Calculos.Estrategia.DescontoAcrescimo.Enum;
using Glass.Data.Model;
using System;

namespace Glass.Data.Helper.Calculos.Estrategia.DescontoAcrescimo.Comissao
{
    class ComissaoGeralStrategy : BaseStrategy<ComissaoGeralStrategy>
    {
        private ComissaoGeralStrategy() { }

        protected override bool PermiteAplicar()
        {
            return PedidoConfig.Comissao.ComissaoPedido;
        }

        protected override decimal CalcularValorAplicar(TipoValor tipo, decimal valorAplicar, decimal totalAtual)
        {
            var percentualCalculo = (100 - valorAplicar) / 100;
            return Math.Round(totalAtual / percentualCalculo - totalAtual, 2);
        }

        protected override bool PermitirRemocaoCalculoProduto(IProdutoCalculo produto)
        {
            return produto.ValorComissao != 0;
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

        protected override decimal BaseCalculoTotalProduto(IProdutoCalculo produto)
        {
            decimal valorDescontoAcrescimoProduto = 0;
            if (PedidoConfig.Comissao.ComissaoAlteraValor)
            {
                valorDescontoAcrescimoProduto = produto.ValorAcrescimo - produto.ValorDesconto;
            }
            return base.BaseCalculoTotalProduto(produto) + valorDescontoAcrescimoProduto;
        }
    }
}
