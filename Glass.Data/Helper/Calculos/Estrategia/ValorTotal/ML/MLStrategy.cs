using GDA;
using Glass.Data.Helper.Calculos.Estrategia.ValorTotal.Enum;
using Glass.Data.Model;

namespace Glass.Data.Helper.Calculos.Estrategia.ValorTotal.ML
{
    class MLStrategy : BaseStrategy<MLStrategy>
    {
        private MLStrategy() { }

        protected override void Calcular(GDASession sessao, IProdutoCalculo produto, int qtdeAmbiente,
            ArredondarAluminio arredondarAluminio, bool calcularMultiploDe5, bool nf, int numeroBeneficiamentos,
            int alturaBeneficiamento, int larguraBeneficiamento, bool compra, decimal custoCompra, bool usarChapaVidro)
        {
            var baseCalculo = ObterBaseCalculo(produto, qtdeAmbiente);

            produto.Total = produto.ValorUnit * baseCalculo;
            produto.CustoProd = custoCompra * baseCalculo;
        }

        private static decimal ObterBaseCalculo(IProdutoCalculo produto, int qtdeAmbiente)
        {
            return qtdeAmbiente
                * (decimal)produto.Altura
                * (decimal)produto.Qtde;
        }
    }
}
