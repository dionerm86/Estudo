using GDA;
using Glass.Data.Model;

namespace Glass.Data.Helper.Calculos.Estrategia.ValorUnitario
{
    class GenericoStrategy : BaseStrategy<GenericoStrategy>
    {
        private GenericoStrategy() { }

        protected override decimal Calcular(GDASession sessao, IProdutoCalculo produto, IContainerCalculo container, int qtdeAmbiente,
            decimal total, bool arredondarAluminio, bool calcMult5, bool nf, int numeroBenef,
            bool calcularAreaMinima, int alturaBenef, int larguraBenef)
        {
            CalculaTotalM2(sessao, produto, container);

            decimal divisor = Divisor(produto, qtdeAmbiente);
            return total / divisor;
        }

        private decimal Divisor(IProdutoCalculo produto, int qtdeAmbiente)
        {
            decimal qtde = produto.Qtde > 0 ? (decimal)produto.Qtde : 1;
            return qtde * qtdeAmbiente;
        }

        private void CalculaTotalM2(GDASession sessao, IProdutoCalculo produto, IContainerCalculo container)
        {
            if (produto.Altura > 0 && produto.Largura > 0)
            {
                produto.TotM = CalculoM2.Instance.Calcular(sessao, produto, container, true);
            }
        }
    }
}
