using GDA;
using Glass.Data.Helper.Calculos.Estrategia.ValorTotal.Enum;
using Glass.Data.Model;

namespace Glass.Data.Helper.Calculos.Estrategia.ValorTotal
{
    public interface IValorTotalStrategy
    {
        void Calcular(GDASession sessao, IProdutoCalculo produto, ArredondarAluminio arredondarAluminio,
            bool calcularMultiploDe5, bool compra, bool nf, int numeroBeneficiamentos, int alturaBeneficiamento,
            int larguraBeneficiamento, bool usarChapaVidro, bool valorBruto = false);
    }
}
