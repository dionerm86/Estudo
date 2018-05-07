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
        public void Calcular(GDASession sessao, IContainerCalculo container, IProdutoCalculo produto,
            ArredondarAluminio arredondarAluminio, bool calcularMultiploDe5, int numeroBeneficiamentos,
            bool usarChapaVidro = true, bool valorBruto = false)
        {
            AtualizaDadosProdutosCalculo(produto, sessao, container);

            if (!DeveExecutar(produto))
                return;

            var alturaBeneficiamento = NormalizarAlturaLarguraBeneficiamento(produto.AlturaBenef, produto);
            var larguraBeneficiamento = NormalizarAlturaLarguraBeneficiamento(produto.LarguraBenef, produto);

            var compra = produto is ProdutosCompra;
            var nf = produto is ProdutosNf;

            var estrategia = ValorTotalStrategyFactory.Instance.RecuperaEstrategia(produto, nf, compra);

            estrategia.Calcular(
                sessao,
                produto,
                arredondarAluminio,
                calcularMultiploDe5,
                compra,
                nf,
                numeroBeneficiamentos,
                alturaBeneficiamento,
                larguraBeneficiamento,
                usarChapaVidro,
                valorBruto
            );

            AtualizarDadosCache(produto);
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
