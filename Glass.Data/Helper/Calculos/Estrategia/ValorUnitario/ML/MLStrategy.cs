using GDA;
using Glass.Data.Helper.Calculos.Estrategia.ValorTotal.Enum;
using Glass.Data.Model;

namespace Glass.Data.Helper.Calculos.Estrategia.ValorUnitario.ML
{
    class MLStrategy : BaseStrategy<MLStrategy>
    {
        private MLStrategy() { }

        protected override decimal Calcular(GDASession sessao, IProdutoCalculo produto, int qtdeAmbiente, decimal total,
            ArredondarAluminio arredondarAluminio, bool calcularMultiploDe5, bool nf, int numeroBeneficiamentos,
            int alturaBeneficiamento, int larguraBeneficiamento)
        {
            decimal divisor = Divisor(produto);
            return total / divisor;
        }

        private decimal Divisor(IProdutoCalculo produto)
        {
            float divisor = produto.Altura * produto.Qtde;
            return divisor > 0 ? (decimal)divisor : 1;
        }
    }
}
