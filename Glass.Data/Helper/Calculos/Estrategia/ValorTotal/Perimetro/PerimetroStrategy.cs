using GDA;
using Glass.Data.Helper.Calculos.Estrategia.ValorTotal.Enum;
using Glass.Data.Model;

namespace Glass.Data.Helper.Calculos.Estrategia.ValorTotal.Perimetro
{
    class PerimetroStrategy : BaseStrategy<PerimetroStrategy>
    {
        private PerimetroStrategy() { }

        protected override void Calcular(GDASession sessao, IProdutoCalculo produto, int qtdeAmbiente,
            ArredondarAluminio arredondarAluminio, bool calcularMultiploDe5, bool nf, int numeroBeneficiamentos,
            int alturaBeneficiamento, int larguraBeneficiamento, bool compra, decimal custoCompra, bool usarChapaVidro)
        {
            var baseCalculo = ObterBaseCalculo(produto, alturaBeneficiamento, larguraBeneficiamento, qtdeAmbiente);

            produto.CustoProd = custoCompra * baseCalculo;
            produto.Total = produto.ValorUnit * baseCalculo;
        }

        private decimal ObterBaseCalculo(IProdutoCalculo produto, int alturaBeneficiamento, int larguraBeneficiamento,
            int qtdeAmbiente)
        {
            var metroLinear = (produto.Altura * alturaBeneficiamento)
                + (produto.Largura * larguraBeneficiamento);

            return qtdeAmbiente
                * (decimal)produto.Qtde
                * (decimal)(metroLinear / 1000);
        }
    }
}
