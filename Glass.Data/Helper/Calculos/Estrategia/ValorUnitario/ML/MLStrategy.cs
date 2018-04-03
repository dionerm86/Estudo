using Glass.Data.Model;

namespace Glass.Data.Helper.Calculos.Estrategia.ValorUnitario.ML
{
    class MLStrategy : BaseStrategy<MLStrategy>
    {
        private MLStrategy() { }

        protected override decimal Calcular(IProdutoCalculo produto, IContainerCalculo container, int qtdeAmbiente,
            decimal total, bool arredondarAluminio, bool calcMult5, bool nf, int numeroBenef,
            bool calcularAreaMinima, int alturaBenef, int larguraBenef)
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
