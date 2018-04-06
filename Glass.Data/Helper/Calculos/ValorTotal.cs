using GDA;
using Glass.Data.Helper.Calculos.Estrategia;
using Glass.Data.Helper.Calculos.Estrategia.ValorTotal.Enum;
using Glass.Data.Model;

namespace Glass.Data.Helper.Calculos
{
    sealed class ValorTotal : BaseCalculo<ValorTotal>
    {
        private ValorTotal() { }

        /// <summary>
        /// Método utilizado para calcular o valor total e o total de m² de um produto.
        /// </summary>
        public void Calcular(GDASession sessao, IProdutoCalculo produto, IContainerCalculo container,
            ArredondarAluminio arredondarAluminio, bool calcMult5, int numeroBenef, bool usarChapaVidro)
        {
            var alturaBenef = NormalizarAlturaLarguraBeneficiamento(produto.AlturaBenef, produto);
            var larguraBenef = NormalizarAlturaLarguraBeneficiamento(produto.LarguraBenef, produto);

            var compra = produto is ProdutosCompra;
            var nf = produto is ProdutosNf;

            var estrategia = ValorTotalStrategyFactory.Instance.RecuperaEstrategia(produto, nf, compra);

            estrategia.Calcular(
                sessao,
                produto,
                container,
                arredondarAluminio,
                calcMult5,
                compra,
                nf,
                numeroBenef,
                alturaBenef,
                larguraBenef,
                usarChapaVidro
            );
        }

        private int NormalizarAlturaLarguraBeneficiamento(int? valor, IProdutoCalculo produto)
        {
            if (valor.HasValue && produto.AlturaBenef > 0 && produto.LarguraBenef > 0)
            {
                return valor.Value;
            }

            return 2;
        }
    }
}
