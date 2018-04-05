using GDA;
using Glass.Data.Model;

namespace Glass.Data.Helper.Calculos.Estrategia.ValorUnitario
{
    public interface IValorUnitarioStrategy
    {
        decimal? Calcular(GDASession sessao, IProdutoCalculo produto, IContainerCalculo container, decimal total,
            bool arredondarAluminio, bool calcMult5, bool nf, int numeroBenef, bool calcularAreaMinima,
            int alturaBenef, int larguraBenef);
    }
}
