using GDA;
using Glass.Data.Helper.Calculos.Estrategia.ValorTotal.Enum;
using Glass.Data.Model;

namespace Glass.Data.Helper.Calculos.Estrategia.ValorUnitario
{
    public interface IValorUnitarioStrategy
    {
        decimal? Calcular(GDASession sessao, IProdutoCalculo produto, IContainerCalculo container, decimal total,
            ArredondarAluminio arredondarAluminio, bool calcMult5, bool nf, int numeroBenef, int alturaBenef,
            int larguraBenef);
    }
}
