using GDA;
using Glass.Data.Helper.Calculos.Estrategia;
using Glass.Data.Helper.Calculos.Estrategia.ValorTotal.Enum;
using Glass.Data.Model;
using Glass.Pool;

namespace Glass.Data.Helper.Calculos
{
    sealed class ValorTotal : Singleton<ValorTotal>
    {
        private ValorTotal() { }

        public void Calcular(GDASession sessao, IContainerCalculo container, IProdutoCalculo produto,
            ArredondarAluminio arredondarAluminio, bool calcularMultiploDe5, int numeroBeneficiamentos,
            bool usarChapaVidro = true, bool valorBruto = false)
        {
            Calcular(
                sessao,
                container,
                produto,
                arredondarAluminio,
                calcularMultiploDe5,
                false,
                false,
                numeroBeneficiamentos,
                usarChapaVidro,
                valorBruto);
        }

        /// <summary>
        /// Método utilizado para calcular o valor total e o total de m² de um produto.
        /// </summary>
        public void Calcular(GDASession sessao, IContainerCalculo container, IProdutoCalculo produto,
            ArredondarAluminio arredondarAluminio, bool calcularMultiploDe5, bool nf, bool compra, int numeroBeneficiamentos,
            bool usarChapaVidro = true, bool valorBruto = false)
        {
            produto.InicializarParaCalculo(sessao, container);

            var alturaBeneficiamento = NormalizarAlturaLarguraBeneficiamento(produto.AlturaBenef, container);
            var larguraBeneficiamento = NormalizarAlturaLarguraBeneficiamento(produto.LarguraBenef, container);

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

            this.IncluirDescontoPorQuantidade(produto);
        }

        private int NormalizarAlturaLarguraBeneficiamento(int? valor, IContainerCalculo container)
        {
            if (container?.MaoDeObra ?? false)
                return valor.Value;

            return 2;
        }

        private void IncluirDescontoPorQuantidade(IProdutoCalculo produto)
        {
            if (produto.PercDescontoQtde > 0)
            {
                var fatorMultiplicacao = produto.PercDescontoQtde < 100
                    ? 1 - (produto.PercDescontoQtde / 100)
                    : 0;

                produto.Total *= (decimal)fatorMultiplicacao;
            }
        }
    }
}
