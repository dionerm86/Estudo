using System;
using Glass.Data.Model;
using Glass.Configuracoes;
using Glass.Data.Helper.DescontoAcrescimo.Estrategia.Enum;

namespace Glass.Data.Helper.DescontoAcrescimo.Estrategia.Comissao
{
    class ComissaoGeralStrategy : BaseStrategy<ComissaoGeralStrategy>
    {
        private ComissaoGeralStrategy() { }

        protected override bool PermitirExecucao()
        {
            return PedidoConfig.Comissao.ComissaoPedido;
        }

        protected override decimal CalculaValorTotalAplicar(TipoValor tipo, decimal valorAplicar, IContainerDescontoAcrescimo container)
        {
            return valorAplicar;
        }

        protected override decimal CalculaPercentualTotalAplicar(decimal totalDesejado, decimal valor)
        {
            return (100 - valor) / 100;
        }

        protected override void PrepararProdutoParaAlteracao(IProdutoDescontoAcrescimo produto)
        {
            produto.RemoverDescontoQtde = true;
        }

        protected override decimal AplicarBeneficiamentos(decimal percentual, IProdutoDescontoAcrescimo produto)
        {
            decimal valorAplicado = 0;

            foreach (var beneficiamento in produto.Beneficiamentos)
            {
                decimal valorCalculado = CalculaValorComissao(beneficiamento.TotalBruto, percentual);

                AplicaValorBeneficiamento(beneficiamento, valorCalculado);
                valorAplicado += valorCalculado;
            }

            return valorAplicado;
        }

        protected override void AplicaValorBeneficiamento(GenericBenef beneficiamento, decimal valor)
        {
            beneficiamento.ValorComissao = valor;
            beneficiamento.Valor += valor;
        }

        protected override void RemoveValorBeneficiamento(GenericBenef beneficiamento)
        {
            beneficiamento.Valor -= beneficiamento.ValorComissao;
            beneficiamento.ValorComissao = 0;
        }

        protected override decimal AplicarProduto(decimal percentual, IProdutoDescontoAcrescimo produto)
        {
            decimal valorCalculado = CalculaValorComissao(
                CalcularTotalBrutoIndependenteCliente(produto) + produto.ValorAcrescimo,
                percentual
            );
            
            AplicaValorProduto(produto, valorCalculado);
            return valorCalculado;
        }

        protected override void AplicaValorProduto(IProdutoDescontoAcrescimo produto, decimal valor)
        {
            produto.ValorComissao = valor;
            produto.Total += valor;
        }

        protected override void RemoveValorProduto(IProdutoDescontoAcrescimo produto)
        {
            produto.Total -= produto.ValorComissao;
            produto.ValorComissao = 0;
        }

        private decimal CalculaValorComissao(decimal baseCalculo, decimal percentual)
        {
            return Math.Round(baseCalculo / percentual - baseCalculo, 2);
        }
    }
}
