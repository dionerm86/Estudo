using Glass.Configuracoes;
using Glass.Data.Model;

namespace Glass.Data.Helper.Calculos.Estrategia.DescontoAcrescimo.Desconto
{
    class DescontoAmbienteStrategy : BaseStrategy<DescontoAmbienteStrategy>
    {
        private DescontoAmbienteStrategy() { }

        protected override bool PermiteAplicar()
        {
            return PedidoConfig.RatearDescontoProdutos;
        }

        protected override bool PermitirRemocaoCalculoProduto(IProdutoCalculo produto)
        {
            return produto.ValorDescontoProd != 0;
        }

        protected override void AplicarValorBeneficiamento(GenericBenef beneficiamento, decimal valor)
        {
            beneficiamento.ValorDescontoProd += valor;
            beneficiamento.Valor -= valor;
        }

        protected override void RemoverValorBeneficiamento(GenericBenef beneficiamento)
        {
            beneficiamento.Valor += beneficiamento.ValorDescontoProd;
            beneficiamento.ValorDescontoProd = 0;
        }

        protected override void AplicarValorProduto(IProdutoCalculo produto, decimal valor)
        {
            produto.ValorDescontoProd += valor;
            produto.Total -= valor;
        }

        protected override void RemoverValorProduto(IProdutoCalculo produto)
        {
            produto.Total += produto.ValorDescontoProd;
            produto.ValorDescontoProd = 0;
        }
    }
}
