using System;
using Glass.Data.Model;
using Glass.Data.Helper.Calculos.Estrategia.DescontoAcrescimo.Enum;

namespace Glass.Data.Helper.Calculos.Estrategia.DescontoAcrescimo.Comissao
{
    class ComissaoGeralStrategy : BaseStrategy<ComissaoGeralStrategy>
    {
        private ComissaoGeralStrategy() { }

        protected override decimal CalcularTotalDesejado(TipoValor tipo, decimal valorAplicar, decimal totalAtual)
        {
            var percentual = CalcularPercentualTotalAplicar(0, valorAplicar);
            var valorTotalAplicar = CalculaValorComissao(totalAtual, percentual);
            return totalAtual + Math.Round(valorTotalAplicar, 2);
        }

        protected override decimal CalcularPercentualTotalAplicar(decimal totalAtual, decimal valorAplicar)
        {
            return (100 - valorAplicar) / 100;
        }

        protected override decimal AplicarBeneficiamentos(decimal percentual, IProdutoDescontoAcrescimo produto)
        {
            decimal valorAplicado = 0;

            foreach (var beneficiamento in produto.Beneficiamentos)
            {
                decimal valorCalculado = CalculaValorComissao(beneficiamento.TotalBruto, percentual);

                AplicarValorBeneficiamento(beneficiamento, valorCalculado);
                valorAplicado += valorCalculado;
            }

            return valorAplicado;
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

        protected override decimal AplicarProduto(decimal percentual, IProdutoDescontoAcrescimo produto)
        {
            decimal valorCalculado = CalculaValorComissao(
                CalcularTotalBrutoDependenteCliente(produto) + produto.ValorAcrescimo,
                percentual
            );
            
            AplicarValorProduto(produto, valorCalculado);
            return valorCalculado;
        }

        protected override void AplicarValorProduto(IProdutoDescontoAcrescimo produto, decimal valor)
        {
            produto.ValorComissao += valor;
            produto.Total += valor;
        }

        protected override void RemoverValorProduto(IProdutoDescontoAcrescimo produto)
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
