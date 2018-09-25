using GDA;
using Glass.Data.Model;
using Glass.Pool;

namespace Glass.Data.Helper.Calculos
{
    sealed class ValorBruto : Singleton<ValorBruto>
    {
        private ValorBruto() { }

        public void Calcular(GDASession sessao, IContainerCalculo container, IProdutoCalculo produto)
        {
            produto.InicializarParaCalculo(sessao, container);

            produto.TotalBruto = produto.Total
                - produto.ValorAcrescimo
                - produto.ValorAcrescimoCliente
                - produto.ValorComissao
                - produto.ValorAcrescimoProd
                + produto.ValorDesconto
                + produto.ValorDescontoCliente
                + produto.ValorDescontoQtde
                + produto.ValorDescontoProd;

            CalcularValorUnitarioBruto(sessao, produto);
        }

        private void CalcularValorUnitarioBruto(GDASession sessao, IProdutoCalculo produto)
        {
            var valorUnitario = ValorUnitario.Instance.RecalcularValor(sessao, produto.Container, produto, true, true);

            if (valorUnitario.HasValue)
                produto.ValorUnitarioBruto = valorUnitario.Value;
        }
    }
}
