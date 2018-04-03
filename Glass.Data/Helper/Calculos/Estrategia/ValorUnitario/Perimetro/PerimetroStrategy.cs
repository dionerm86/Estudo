using Glass.Data.Model;

namespace Glass.Data.Helper.Calculos.Estrategia.ValorUnitario.Perimetro
{
    class PerimetroStrategy : BaseStrategy<PerimetroStrategy>
    {
        private PerimetroStrategy() { }

        protected override decimal Calcular(IProdutoCalculo produto, IContainerCalculo container, int qtdeAmbiente,
            decimal total, bool arredondarAluminio, bool calcMult5, bool nf, int numeroBenef,
            bool calcularAreaMinima, int alturaBenef, int larguraBenef)
        {
            var altura = (int)produto.Altura * alturaBenef;
            var largura = produto.Largura * larguraBenef;

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
