using GDA;
using Glass.Data.Helper.Calculos.Estrategia.ValorTotal.Enum;
using Glass.Data.Model;

namespace Glass.Data.Helper.Calculos.Estrategia.ValorUnitario
{
    public interface IValorUnitarioStrategy
    {
        decimal? Calcular(GDASession sessao, IProdutoCalculo produto, decimal total, ArredondarAluminio arredondarAluminio,
            bool calcularMultiploDe5, bool nf, int numeroBeneficiamentos, int alturaBeneficiamento,
            int larguraBeneficiamento);
    }
}
