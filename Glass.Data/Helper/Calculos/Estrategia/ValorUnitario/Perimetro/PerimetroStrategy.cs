using GDA;
using Glass.Data.Helper.Calculos.Estrategia.ValorTotal.Enum;
using Glass.Data.Model;

namespace Glass.Data.Helper.Calculos.Estrategia.ValorUnitario.Perimetro
{
    class PerimetroStrategy : BaseStrategy<PerimetroStrategy>
    {
        private PerimetroStrategy() { }

        protected override decimal Calcular(GDASession sessao, IProdutoCalculo produto, int qtdeAmbiente, decimal total,
            ArredondarAluminio arredondarAluminio, bool calcularMultiploDe5, bool nf, int numeroBeneficiamentos,
            int alturaBeneficiamento, int larguraBeneficiamento)
        {
            var altura = (int)produto.Altura * alturaBeneficiamento;
            var largura = produto.Largura * larguraBeneficiamento;

            int metroLinear = altura + largura;
            decimal divisor = Divisor(produto, qtdeAmbiente, metroLinear);

            return total / divisor;
        }

        private decimal Divisor(IProdutoCalculo produto, int qtdeAmbiente, int metroLinear)
        {
            float divisor = (metroLinear / 1000F) * (produto.Qtde * qtdeAmbiente);
            divisor = divisor > 0 ? divisor : 1;

            return (decimal)divisor;
        }
    }
}
