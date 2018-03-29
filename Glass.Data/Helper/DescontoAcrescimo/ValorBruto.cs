using Glass.Data.Model;
using Glass.Global;
using Glass.Pool;
using System;

namespace Glass.Data.Helper.DescontoAcrescimo
{
    class ValorBruto : PoolableObject<ValorBruto>
    {
        private ValorBruto() { }

        public void Calcular(IProdutoDescontoAcrescimo produto, IContainerDescontoAcrescimo container)
        {
            produto.TotalBruto = produto.Total
                - produto.ValorAcrescimo
                - produto.ValorAcrescimoCliente
                - produto.ValorComissao
                - produto.ValorAcrescimoProd
                + produto.ValorDesconto
                + produto.ValorDescontoCliente
                + produto.ValorDescontoQtde
                + produto.ValorDescontoProd;

            ValorUnitario.Instance.CalcularBruto(produto, container);
        }
    }
}
