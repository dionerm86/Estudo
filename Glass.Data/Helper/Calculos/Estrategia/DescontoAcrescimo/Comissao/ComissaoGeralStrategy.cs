using System;
using Glass.Data.Model;
using Glass.Data.Helper.Calculos.Estrategia.DescontoAcrescimo.Enum;

namespace Glass.Data.Helper.Calculos.Estrategia.DescontoAcrescimo.Comissao
{
    class ComissaoGeralStrategy : BaseComissaoStrategy<ComissaoGeralStrategy>
    {
        private ComissaoGeralStrategy() { }

        protected override decimal AplicarBeneficiamentos(decimal percentual, IProdutoCalculo produto)
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

        protected override decimal AplicarProduto(decimal percentual, IProdutoCalculo produto)
        {
            decimal valorCalculado = CalculaValorComissao(
                CalcularTotalBrutoDependenteCliente(produto) + produto.ValorAcrescimo,
                percentual
            );
            
            AplicarValorProduto(produto, valorCalculado);
            return valorCalculado;
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
