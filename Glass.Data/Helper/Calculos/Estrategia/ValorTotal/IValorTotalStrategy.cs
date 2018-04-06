using GDA;
using Glass.Data.Helper.Calculos.Estrategia.ValorTotal.Enum;
using Glass.Data.Model;

namespace Glass.Data.Helper.Calculos.Estrategia.ValorTotal
{
    public interface IValorTotalStrategy
    {
        void Calcular(GDASession sessao, IProdutoCalculo produto, IContainerCalculo container,
            ArredondarAluminio arredondarAluminio, bool calcMult5, bool compra, bool nf, int numeroBenef,
            int alturaBenef, int larguraBenef, bool usarChapaVidro);
    }
}
