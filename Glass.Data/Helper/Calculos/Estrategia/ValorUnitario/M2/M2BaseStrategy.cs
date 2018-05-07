using Glass.Data.Model;
using GDA;
using Glass.Data.Helper.Calculos.Estrategia.ValorTotal.Enum;

namespace Glass.Data.Helper.Calculos.Estrategia.ValorUnitario.M2
{
    abstract class M2BaseStrategy<T> : BaseStrategy<T>
        where T : M2BaseStrategy<T>
    {
        protected abstract bool DeveCalcularMultiploDe5 { get; }

        protected override decimal Calcular(GDASession sessao, IProdutoCalculo produto, int qtdeAmbiente, decimal total,
            ArredondarAluminio arredondarAluminio, bool calcularMultiploDe5, bool nf, int numeroBeneficiamentos,
            int alturaBeneficiamento, int larguraBeneficiamento)
        {
            float totM2Temp = produto.TotM;

            CalcularTotalM2(sessao, produto, calcularMultiploDe5, nf, totM2Temp);

            float totM2Preco = CalcularTotalM2ParaCalculoPreco(
                sessao,
                produto,
                qtdeAmbiente,
                calcularMultiploDe5,
                nf,
                numeroBeneficiamentos,
                totM2Temp
            );

            float areaMinima = AreaMinima(produto, numeroBeneficiamentos);

            float totM2Calc = totM2Preco < (areaMinima * produto.Qtde * qtdeAmbiente)
                ? (areaMinima * produto.Qtde * qtdeAmbiente)
                : totM2Preco;

            return total / (totM2Calc > 0 ? (decimal)totM2Calc : 1);
        }

        private void CalcularTotalM2(GDASession sessao, IProdutoCalculo produto, bool calcMult5, bool nf, float totM2Temp)
        {
            produto.TotM = !nf
                ? CalculoM2.Instance.Calcular(
                    sessao,
                    produto.Container,
                    produto,
                    calcMult5
                )
                : totM2Temp;
        }

        private float CalcularTotalM2ParaCalculoPreco(GDASession sessao, IProdutoCalculo produto, int qtdeAmbiente,
            bool calcularMultiploDe5, bool nf, int numeroBenef, float totM2Temp)
        {
            return !nf
                ? CalculoM2.Instance.CalcularM2Calculo(
                    sessao,
                    produto.Container,
                    produto,
                    true,
                    calcularMultiploDe5 && DeveCalcularMultiploDe5,
                    NumeroBeneficiamentosAreaMinima(produto, numeroBenef),
                    qtdeAmbiente,
                    LarguraProduto(produto)
                )
                : totM2Temp;
        }

        private int LarguraProduto(IProdutoCalculo produto)
        {
            if (produto.Container.MaoDeObra && produto.Redondo && produto.Largura == 0)
            {
                return (int)produto.Altura;
            }

            return produto.Largura;
        }

        private int NumeroBeneficiamentosAreaMinima(IProdutoCalculo produto, int numeroBenef)
        {
            var numeroBeneficiamentosAreaMinima = numeroBenef > 0
                ? numeroBenef
                : 0;

            if (numeroBeneficiamentosAreaMinima == 0
                && produto.DadosProduto.CalcularAreaMinima(numeroBenef))
            {
                numeroBeneficiamentosAreaMinima = 1;
            }

            return numeroBeneficiamentosAreaMinima;
        }

        private float AreaMinima(IProdutoCalculo produto, int numeroBenef)
        {
            return produto.DadosProduto.CalcularAreaMinima(numeroBenef)
                ? produto.DadosProduto.AreaMinima()
                : 0;
        }
    }
}
