using Glass.Data.Model;
using GDA;
using Glass.Data.Helper.Calculos.Estrategia.ValorTotal.Enum;

namespace Glass.Data.Helper.Calculos.Estrategia.ValorUnitario.M2
{
    abstract class M2BaseStrategy<T> : BaseStrategy<T>
        where T : M2BaseStrategy<T>
    {
        protected abstract bool CalcularMultiploDe5 { get; }

        protected override decimal Calcular(GDASession sessao, IProdutoCalculo produto, IContainerCalculo container,
            int qtdeAmbiente, decimal total, ArredondarAluminio arredondarAluminio, bool calcMult5, bool nf, int numeroBenef,
            int alturaBenef, int larguraBenef)
        {
            float totM2Temp = produto.TotM;

            CalcularTotalM2(sessao, produto, container, calcMult5, nf, totM2Temp);

            float totM2Preco = CalcularTotalM2ParaCalculoPreco(
                sessao,
                produto,
                container,
                qtdeAmbiente,
                calcMult5,
                nf,
                numeroBenef,
                totM2Temp
            );

            float areaMinima = AreaMinima(sessao, produto, container, numeroBenef);

            float totM2Calc = totM2Preco < (areaMinima * produto.Qtde * qtdeAmbiente)
                ? (areaMinima * produto.Qtde * qtdeAmbiente)
                : totM2Preco;

            return total / (totM2Calc > 0 ? (decimal)totM2Calc : 1);
        }

        private void CalcularTotalM2(GDASession sessao, IProdutoCalculo produto, IContainerCalculo container,
            bool calcMult5, bool nf, float totM2Temp)
        {
            produto.TotM = !nf
                ? CalculoM2.Instance.Calcular(
                    sessao,
                    produto,
                    container,
                    calcMult5
                )
                : totM2Temp;
        }

        private float CalcularTotalM2ParaCalculoPreco(GDASession sessao, IProdutoCalculo produto, IContainerCalculo container,
            int qtdeAmbiente, bool calcMult5, bool nf, int numeroBenef, float totM2Temp)
        {
            return !nf
                ? CalculoM2.Instance.CalcularM2Calculo(
                    sessao,
                    produto,
                    container,
                    true,
                    calcMult5 && CalcularMultiploDe5,
                    NumeroBeneficiamentosAreaMinima(sessao, produto, container, numeroBenef),
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

        private int NumeroBeneficiamentosAreaMinima(GDASession sessao, IProdutoCalculo produto, IContainerCalculo container,
            int numeroBenef)
        {
            var numeroBeneficiamentosAreaMinima = numeroBenef > 0
                ? numeroBenef
                : 0;

            if (numeroBeneficiamentosAreaMinima == 0
                && container.DadosProduto.CalcularAreaMinima(sessao, produto, numeroBenef))
            {
                numeroBeneficiamentosAreaMinima = 1;
            }

            return numeroBeneficiamentosAreaMinima;
        }

        private float AreaMinima(GDASession sessao, IProdutoCalculo produto, IContainerCalculo container, int numeroBenef)
        {
            return container.DadosProduto.CalcularAreaMinima(sessao, produto, numeroBenef)
                ? container.DadosProduto.AreaMinima(sessao, produto)
                : 0;
        }
    }
}
