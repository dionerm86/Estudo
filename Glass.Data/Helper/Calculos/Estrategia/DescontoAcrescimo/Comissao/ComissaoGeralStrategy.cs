using GDA;
using Glass.Configuracoes;
using Glass.Data.Helper.Calculos.Estrategia.DescontoAcrescimo.Enum;
using Glass.Data.Model;
using System;
using System.Collections.Generic;

namespace Glass.Data.Helper.Calculos.Estrategia.DescontoAcrescimo.Comissao
{
    class ComissaoGeralStrategy : BaseStrategy<ComissaoGeralStrategy>
    {
        private ComissaoGeralStrategy() { }

        protected override bool PermiteAplicar()
        {
            return PedidoConfig.Comissao.ComissaoPedido;
        }

        protected override decimal AplicarProduto(decimal percentual, IProdutoCalculo produto)
        {
            decimal valorCalculado = Math.Round(percentual / 100 * (PrecoTabelaCliente(produto) + produto.ValorAcrescimo - produto.ValorDesconto), 2);
            AplicarValorProduto(produto, valorCalculado);

            return valorCalculado;
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

        protected override decimal AplicarBeneficiamentos(decimal percentual, IProdutoCalculo produto)
        {
            decimal valorAplicado = 0;
            var beneficiamentos = produto.Beneficiamentos ?? GenericBenefCollection.Empty;

            foreach (var beneficiamento in beneficiamentos)
            {
                decimal valorCalculado = Math.Round(percentual / 100 * (beneficiamento.TotalBruto + beneficiamento.ValorAcrescimo - beneficiamento.ValorDesconto), 2);
                valorAplicado += valorCalculado;

                AplicarValorBeneficiamento(beneficiamento, valorCalculado);
            }

            produto.Beneficiamentos = beneficiamentos;
            return valorAplicado;
        }

        protected override void RemoverBeneficiamentos(IProdutoCalculo produto)
        {
            var beneficiamentos = produto.Beneficiamentos ?? GenericBenefCollection.Empty;

            foreach (var beneficiamento in beneficiamentos)
            {
                beneficiamento.PercentualComissao = produto.PercentualComissao;
                RemoverValorBeneficiamento(beneficiamento);
            }

            produto.Beneficiamentos = beneficiamentos;
        }

        protected override void AplicarValorBeneficiamento(GenericBenef beneficiamento, decimal valor)
        {
            beneficiamento.ValorComissao += valor;
            beneficiamento.Valor += valor;
        }

        protected override void RemoverValorBeneficiamento(GenericBenef beneficiamento)
        {
            beneficiamento.Valor -= Math.Round(beneficiamento.Valor * (beneficiamento.PercentualComissao / 100), 2);
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

        protected override decimal CalcularTotalBeneficiamentosProduto(IProdutoCalculo produto)
        {
            decimal totalAtual = 0;

            foreach (var beneficiamento in (produto.Beneficiamentos ?? GenericBenefCollection.Empty))
            {
                totalAtual += beneficiamento.TotalBruto + beneficiamento.ValorAcrescimo - beneficiamento.ValorDesconto;
            }

            return totalAtual;
        }

        protected override decimal BaseCalculoTotalProduto(IProdutoCalculo produto)
        {
            return base.BaseCalculoTotalProduto(produto) + produto.ValorAcrescimo - produto.ValorDesconto;
        }

        protected override decimal BaseCalculoTotalResidualProduto(IProdutoCalculo produto)
        {
            return base.BaseCalculoTotalProduto(produto) + produto.ValorAcrescimo - produto.ValorDesconto;
        }
    }
}
