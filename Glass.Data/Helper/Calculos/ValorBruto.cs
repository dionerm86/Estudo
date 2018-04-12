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

            CalcularValorUnitarioBruto(sessao, produto);

            produto.TotalBruto = produto.Total
                - produto.ValorAcrescimo
                - produto.ValorAcrescimoCliente
                - produto.ValorComissao
                - produto.ValorAcrescimoProd
                + produto.ValorDesconto
                + produto.ValorDescontoCliente
                + produto.ValorDescontoQtde
                + produto.ValorDescontoProd;

            AtualizarDadosCache(produto);
        }

        private void CalcularValorUnitarioBruto(GDASession sessao, IProdutoCalculo produto)
        {
            var valorUnitario = ValorUnitario.Instance.RecalcularValor(sessao, produto.Container, produto, true);

            if (valorUnitario.HasValue)
                produto.ValorUnitarioBruto = valorUnitario.Value;
        }
    }
}
