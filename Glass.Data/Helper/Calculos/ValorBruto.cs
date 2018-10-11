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

            produto.TotalBruto = produto.Total - DescontoValorBruto(produto);
            CalcularValorUnitarioBruto(sessao, produto);
        }

        public decimal DescontoValorBruto(IProdutoCalculo produto)
        {
            return produto.ValorAcrescimo
                + produto.ValorAcrescimoCliente
                + produto.ValorComissao
                + produto.ValorAcrescimoProd
                - produto.ValorDesconto
                - produto.ValorDescontoCliente
                - produto.ValorDescontoQtde
                - produto.ValorDescontoProd;
        }

        private void CalcularValorUnitarioBruto(GDASession sessao, IProdutoCalculo produto)
        {
            var valorUnitario = ValorUnitario.Instance.CalcularValor(sessao, produto.Container, produto, produto.TotalBruto);

            if (valorUnitario.HasValue)
                produto.ValorUnitarioBruto = valorUnitario.Value;
        }
    }
}
