using GDA;
using Glass.Data.Helper.Calculos.Estrategia.ValorTotal.Enum;
using Glass.Data.Model;

namespace Glass.Data.Helper.Calculos.Estrategia.ValorTotal.Perimetro
{
    class PerimetroStrategy : BaseStrategy<PerimetroStrategy>
    {
        private PerimetroStrategy() { }

        protected override void Calcular(GDASession sessao, IProdutoCalculo produto, IContainerCalculo container,
            int qtdeAmbiente, ArredondarAluminio arredondarAluminio, bool calcMult5, bool nf, int numeroBenef,
            int alturaBenef, int larguraBenef, bool compra, decimal custoCompra, bool usarChapaVidro)
        {
            var baseCalculo = ObterBaseCalculo(produto, alturaBenef, larguraBenef, qtdeAmbiente);

            produto.CustoProd = custoCompra * baseCalculo;
            produto.Total = produto.ValorUnit * baseCalculo;
        }

        private decimal ObterBaseCalculo(IProdutoCalculo produto, int alturaBenef, int larguraBenef, int qtdeAmbiente)
        {
            var metroLinear = (produto.Altura * alturaBenef)
                + (produto.Largura * larguraBenef);

            return qtdeAmbiente
                * (decimal)produto.Qtde
                * (decimal)(metroLinear / 1000);
        }
    }
}
