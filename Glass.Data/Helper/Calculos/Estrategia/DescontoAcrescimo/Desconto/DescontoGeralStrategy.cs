using Glass.Configuracoes;
using Glass.Data.Model;
using System;

namespace Glass.Data.Helper.Calculos.Estrategia.DescontoAcrescimo.Desconto
{
    class DescontoGeralStrategy : BaseStrategy<DescontoGeralStrategy>
    {
        private DescontoGeralStrategy() { }

        protected override bool PermiteAplicar()
        {
            return PedidoConfig.RatearDescontoProdutos;
        }

        protected override decimal AplicarProduto(decimal percentual, IProdutoCalculo produto)
        {
            decimal valorCalculado = Math.Round(percentual / 100 * (this.PrecoTabelaCliente(produto) + produto.ValorAcrescimo), 2);
            this.AplicarValorProduto(produto, valorCalculado);

            return valorCalculado;
        }

        protected override decimal PrecoTabelaCliente(IProdutoCalculo produto)
        {
            return produto.TotalBruto - produto.ValorDescontoCliente + produto.ValorAcrescimoCliente;
        }

        protected override bool PermitirRemocaoCalculoProduto(IProdutoCalculo produto)
        {
            return produto.ValorDesconto != 0;
        }

        protected override decimal AplicarBeneficiamentos(decimal percentual, IProdutoCalculo produto)
        {
            decimal valorAplicado = 0;
            var beneficiamentos = produto.Beneficiamentos ?? GenericBenefCollection.Empty;

            foreach (var beneficiamento in beneficiamentos)
            {
                decimal valorCalculado = Math.Round(percentual / 100 * (beneficiamento.TotalBruto + beneficiamento.ValorAcrescimo), 2);
                valorAplicado += valorCalculado;

                AplicarValorBeneficiamento(beneficiamento, valorCalculado);
            }

            produto.Beneficiamentos = beneficiamentos;
            return valorAplicado;
        }

        protected override void AplicarValorBeneficiamento(GenericBenef beneficiamento, decimal valor)
        {
            beneficiamento.ValorDesconto += valor;
            beneficiamento.Valor -= valor;
        }

        protected override void RemoverValorBeneficiamento(GenericBenef beneficiamento)
        {
            beneficiamento.Valor += beneficiamento.ValorDesconto;
            beneficiamento.ValorDesconto = 0;
        }

        protected override void AplicarValorProduto(IProdutoCalculo produto, decimal valor)
        {
            produto.ValorDesconto += valor;
            produto.Total -= valor;
        }

        protected override void RemoverValorProduto(IProdutoCalculo produto)
        {
            produto.Total += produto.ValorDesconto;
            produto.ValorDesconto = 0;
        }

        protected override decimal CalcularTotalBeneficiamentosProduto(IProdutoCalculo produto)
        {
            decimal totalAtual = 0;

            foreach (var beneficiamento in (produto.Beneficiamentos ?? GenericBenefCollection.Empty))
            {
                totalAtual += beneficiamento.TotalBruto + beneficiamento.ValorAcrescimo;
            }

            return totalAtual;
        }

        protected override decimal BaseCalculoTotalProduto(IProdutoCalculo produto)
        {
            return base.BaseCalculoTotalProduto(produto) + produto.ValorAcrescimo;
        }
    }
}
