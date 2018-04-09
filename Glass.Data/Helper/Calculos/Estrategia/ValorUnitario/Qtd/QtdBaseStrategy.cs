using GDA;
using Glass.Data.Helper.Calculos.Estrategia.ValorTotal.Enum;
using Glass.Data.Model;

namespace Glass.Data.Helper.Calculos.Estrategia.ValorUnitario
{
    abstract class QtdBaseStrategy<T> : BaseStrategy<T>
        where T : QtdBaseStrategy<T>
    {
        protected override decimal Calcular(GDASession sessao, IProdutoCalculo produto, int qtdeAmbiente, decimal total,
            ArredondarAluminio arredondarAluminio, bool calcularMultiploDe5, bool nf, int numeroBeneficiamentos,
            int alturaBeneficiamento, int larguraBeneficiamento)
        {
            decimal divisor = Divisor(produto, qtdeAmbiente);
            return total / divisor;
        }

        private decimal Divisor(IProdutoCalculo produto, int qtdeAmbiente)
        {
            decimal qtde = produto.Qtde > 0 ? (decimal)produto.Qtde : 1;
            return qtde * qtdeAmbiente;
        }
    }
}
