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
            float areaMinimaProd = ProdutoDAO.Instance.ObtemAreaMinima(null, (int)produto.IdProduto);
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
                areaMinimaProd,
                totM2Temp);

            float areaMinima = AreaMinima(produto, container, numeroBenef, areaMinimaProd);
            float totM2Calc = totM2Preco < (areaMinima * produto.Qtde * qtdeAmbiente)
                ? (areaMinima * produto.Qtde * qtdeAmbiente)
                : totM2Preco;

            return total / (totM2Calc > 0 ? (decimal)totM2Calc : 1);
        }

        private void CalcularTotalM2(IProdutoCalculo produto, IContainerCalculo container, bool calcMult5,
            bool nf, float totM2Temp)
        {
            produto.TotM = !nf
                ? CalculosFluxo.ArredondaM2(
                    null,
                    LarguraProduto(produto, container),
                    (int)produto.Altura,
                    produto.Qtde,
                    (int)produto.IdProduto,
                    produto.Redondo,
                    produto.Espessura,
                    calcMult5 && CalcularMultiploDe5
                )
                : totM2Temp;
        }

        private float CalcularTotalM2ParaCalculoPreco(IProdutoCalculo produto, IContainerCalculo container, int qtdeAmbiente,
            bool calcMult5, bool nf, int numeroBenef, bool calcularAreaMinima, float areaMinimaProd, float totM2Temp)
        {
            return !nf
                ? CalculosFluxo.CalcM2Calculo(
                    null,
                    container.IdCliente.GetValueOrDefault(),
                    (int)produto.Altura,
                    LarguraProduto(produto, container),
                    produto.Qtde * qtdeAmbiente,
                    (int)produto.IdProduto,
                    produto.Redondo,
                    NumeroBeneficiamentosAreaMinima(numeroBenef, calcularAreaMinima),
                    areaMinimaProd,
                    true,
                    produto.Espessura,
                    calcMult5 && CalcularMultiploDe5
                )
                : totM2Temp;
        }

        private int LarguraProduto(IProdutoCalculo produto, IContainerCalculo container)
        {
            int largura = produto.Largura;

            if (container.MaoDeObra && produto.Redondo && produto.Largura == 0)
            {
                largura = (int)produto.Altura;
            }

            return largura;
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

        private float AreaMinima(IProdutoCalculo produto, IContainerCalculo container, int numeroBenef, float areaMinimaProd)
        {
            return ProdutoDAO.Instance.CalcularAreaMinima(
                    null,
                    container.IdCliente.GetValueOrDefault(),
                    (int)produto.IdProduto,
                    produto.Redondo,
                    numeroBenef
                )
                ? areaMinimaProd
                : 0;
        }
    }
}
