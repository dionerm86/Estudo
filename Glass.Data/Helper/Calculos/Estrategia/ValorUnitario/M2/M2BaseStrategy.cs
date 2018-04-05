using Glass.Data.Model;
using Glass.Data.DAL;
using Glass.Global;

namespace Glass.Data.Helper.Calculos.Estrategia.ValorUnitario.M2
{
    abstract class M2BaseStrategy<T> : BaseStrategy<T>
        where T : M2BaseStrategy<T>
    {
        protected abstract bool CalcularMultiploDe5 { get; }

        protected override decimal Calcular(IProdutoCalculo produto, IContainerCalculo container,
            int qtdeAmbiente, decimal total, bool arredondarAluminio, bool calcMult5, bool nf,
            int numeroBenef, bool calcularAreaMinima, int alturaBenef, int larguraBenef)
        {
            float totM2Temp = produto.TotM;

            CalcularTotalM2(produto, container, calcMult5, nf, totM2Temp);

            float totM2Preco = CalcularTotalM2ParaCalculoPreco(
                produto,
                container,
                qtdeAmbiente,
                calcMult5,
                nf,
                numeroBenef,
                calcularAreaMinima,
                totM2Temp
            );

            float areaMinima = AreaMinima(produto, container, numeroBenef);

            float totM2Calc = totM2Preco < (areaMinima * produto.Qtde * qtdeAmbiente)
                ? (areaMinima * produto.Qtde * qtdeAmbiente)
                : totM2Preco;

            return total / (totM2Calc > 0 ? (decimal)totM2Calc : 1);
        }

        private void CalcularTotalM2(IProdutoCalculo produto, IContainerCalculo container, bool calcMult5,
            bool nf, float totM2Temp)
        {
            produto.TotM = !nf
                ? CalculoM2.Instance.Calcular(
                    produto,
                    container,
                    calcMult5
                )
                : totM2Temp;
        }

        private float CalcularTotalM2ParaCalculoPreco(IProdutoCalculo produto, IContainerCalculo container, int qtdeAmbiente,
            bool calcMult5, bool nf, int numeroBenef, bool calcularAreaMinima, float totM2Temp)
        {
            return !nf
                ? CalculoM2.Instance.CalcularM2Calculo(
                    produto,
                    container,
                    true,
                    calcMult5 && CalcularMultiploDe5,
                    NumeroBeneficiamentosAreaMinima(numeroBenef, calcularAreaMinima),
                    qtdeAmbiente,
                    LarguraProduto(produto, container)
                )
                : totM2Temp;
        }

        private int LarguraProduto(IProdutoCalculo produto, IContainerCalculo container)
        {
            if (container.MaoDeObra && produto.Redondo && produto.Largura == 0)
            {
                return (int)produto.Altura;
            }

            return produto.Largura;
        }

        private int NumeroBeneficiamentosAreaMinima(int numeroBenef, bool calcularAreaMinima)
        {
            var numeroBeneficiamentosAreaMinima = numeroBenef > 0
                ? numeroBenef
                : 0;

            if (numeroBeneficiamentosAreaMinima == 0 && calcularAreaMinima)
            {
                numeroBeneficiamentosAreaMinima = 1;
            }

            return numeroBeneficiamentosAreaMinima;
        }

        private float AreaMinima(IProdutoCalculo produto, IContainerCalculo container, int numeroBenef)
        {
            return container.DadosProduto.CalcularAreaMinima(produto, numeroBenef)
                ? container.DadosProduto.AreaMinima(produto)
                : 0;
        }
    }
}
