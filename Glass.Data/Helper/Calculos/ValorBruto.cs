using GDA;
using Glass.Data.Model;

namespace Glass.Data.Helper.Calculos
{
    sealed class ValorBruto : BaseCalculo<ValorBruto>
    {
        private ValorBruto() { }

        public void Calcular(GDASession sessao, IProdutoCalculo produto, IContainerCalculo container,
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

            CalcularValorUnitario(sessao, produto, container, calcularAreaMinima);

            AtualizarDadosCache(produto, container);
        }

        private void CalcularValorUnitario(GDASession sessao, IProdutoCalculo produto, IContainerCalculo container,
            bool calcularAreaMinima)
        {
            var valorUnitario = ValorUnitario.Instance.RecalcularValor(sessao, produto, container, calcularAreaMinima, true);

            if (valorUnitario.HasValue)
                produto.ValorUnitarioBruto = valorUnitario.Value;
        }
    }
}
