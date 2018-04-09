using GDA;
using Glass.Data.Model;

namespace Glass.Data.Helper.Calculos
{
    sealed class ValorBruto : BaseCalculo<ValorBruto>
    {
        private ValorBruto() { }

        public void Calcular(GDASession sessao, IContainerCalculo container, IProdutoCalculo produto)
        {
            AtualizaDadosProdutosCalculo(produto, sessao, container);

            if (!DeveExecutar(produto))
                return;

            produto.TotalBruto = produto.Total
                - produto.ValorAcrescimo
                - produto.ValorAcrescimoCliente
                - produto.ValorComissao
                - produto.ValorAcrescimoProd
                + produto.ValorDesconto
                + produto.ValorDescontoCliente
                + produto.ValorDescontoQtde
                + produto.ValorDescontoProd;

            CalcularValorUnitario(sessao, produto);

            AtualizarDadosCache(produto);
        }

        private void CalcularValorUnitario(GDASession sessao, IProdutoCalculo produto)
        {
            var valorUnitario = ValorUnitario.Instance.RecalcularValor(sessao, produto.Container, produto, true);

            if (valorUnitario.HasValue)
                produto.ValorUnitarioBruto = valorUnitario.Value;
        }
    }
}
