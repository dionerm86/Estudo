using Glass.Data.Model;
using Glass.Pool;

namespace Glass.Data.Helper.Calculos.Estrategia.ValorUnitario
{
    abstract class BaseStrategy<T> : PoolableObject<T>, IValorUnitarioStrategy
        where T : BaseStrategy<T>
    {
        public decimal? Calcular(IProdutoCalculo produto, IContainerCalculo container, decimal total,
            bool arredondarAluminio, bool calcMult5, bool nf, int numeroBenef,
            bool calcularAreaMinima, int alturaBenef, int larguraBenef)
        {
            var qtdeAmbiente = produto.QtdeAmbiente > 0
                ? produto.QtdeAmbiente
                : 1;

            return Calcular(
                produto,
                container,
                qtdeAmbiente,
                total,
                arredondarAluminio,
                calcMult5,
                nf,
                numeroBenef,
                calcularAreaMinima,
                alturaBenef,
                larguraBenef
            );
        }

        protected abstract decimal Calcular(IProdutoCalculo produto, IContainerCalculo container, int qtdeAmbiente,
            decimal total, bool arredondarAluminio, bool calcMult5, bool nf, int numeroBenef,
            bool calcularAreaMinima, int alturaBenef, int larguraBenef);
    }
}
