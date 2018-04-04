using Glass.Data.Model;

namespace Glass.Data.Helper.Calculos
{
    sealed class ValorBruto : BaseCalculo<ValorBruto>
    {
        private ValorBruto()
            : base("valorBruto")
        {
        }

        public void Calcular(IProdutoCalculo produto, IContainerCalculo container,
            bool calcularAreaMinima = false)
        {
            if (!DeveExecutarParaOsItens(produto, container))
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

            CalcularValorUnitario(produto, container, calcularAreaMinima);

            AtualizarDadosCache(produto, container);
        }

        private void CalcularValorUnitario(IProdutoCalculo produto, IContainerCalculo container,
            bool calcularAreaMinima)
        {
            var valorUnitario = ValorUnitario.Instance.RecalcularValor(produto, container, calcularAreaMinima, true);

            if (valorUnitario.HasValue)
                produto.ValorUnitarioBruto = valorUnitario.Value;
        }
    }
}
