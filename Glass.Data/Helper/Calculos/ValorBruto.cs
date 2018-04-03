using Glass.Data.Model;
using Glass.Pool;

namespace Glass.Data.Helper.Calculos
{
    sealed class ValorBruto : PoolableObject<ValorBruto>
    {
        private ValorBruto() { }

        public void Calcular(IProdutoCalculo produto, IContainerCalculo container,
            bool calcularAreaMinima = false)
        {
            if (produto == null || container == null)
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
