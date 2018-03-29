using Glass.Data.Model;
using Glass.Global;

namespace Glass.Data.Helper.DescontoAcrescimo
{
    class ValorBruto
    {
        public void Calcular(GDASession sessao, IProdutoDescontoAcrescimo produto, IContainerDescontoAcrescimo container)
        {
            produto.TotalBruto = produto.Total - produto.ValorAcrescimo - produto.ValorAcrescimoCliente + produto.ValorDesconto + produto.ValorDescontoCliente + produto.ValorDescontoQtde -
                produto.ValorComissao - produto.ValorAcrescimoProd + produto.ValorDescontoProd;

            decimal valorUnitario = 0;
            var alturaBenef = produto.AlturaBenef == null || (produto.AlturaBenef == 0 && produto.LarguraBenef == 0) ? 2 : produto.AlturaBenef.Value;
            var larguraBenef = produto.LarguraBenef == null || (produto.AlturaBenef == 0 && produto.LarguraBenef == 0) ? 2 : produto.LarguraBenef.Value;
            
            CalculosFluxo.CalcValorUnitItemProd(
                sessao,
                container.IdCliente.GetValueOrDefault(),
                (int)produto.IdProduto,
                produto.Largura,
                produto.Qtde,
                produto.QtdeAmbiente,
                produto.TotalBruto,
                produto.Espessura,
                produto.Redondo,
                1,
                false,
                !container.IsPedidoProducaoCorte,
                produto.Altura,
                produto.TotM,
                ref valorUnitario,
                produto.Beneficiamentos.CountAreaMinimaSession(sessao),
                alturaBenef,
                larguraBenef
            );

            produto.ValorUnitarioBruto = valorUnitario;
        }
    }
}
