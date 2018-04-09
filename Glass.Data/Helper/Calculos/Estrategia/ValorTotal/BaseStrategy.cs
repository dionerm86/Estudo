using GDA;
using Glass.Data.Helper.Calculos.Estrategia.ValorTotal.Enum;
using Glass.Data.Model;
using Glass.Pool;
using System;

namespace Glass.Data.Helper.Calculos.Estrategia.ValorTotal
{
    abstract class BaseStrategy<T> : Singleton<T>, IValorTotalStrategy
        where T : BaseStrategy<T>
    {
        protected virtual bool ValidarQuantidadeDecimal
        {
            get { return true; }
        }

        public void Calcular(GDASession sessao, IProdutoCalculo produto, ArredondarAluminio arredondarAluminio,
            bool calcularMultiploDe5, bool compra, bool nf, int numeroBeneficiamentos, int alturaBeneficiamento,
            int larguraBeneficiamento, bool usarChapaVidro)
        {
            /* Chamado 41410. */
            if (ValidarQuantidadeDecimal && produto.Qtde % 1 > 0)
                throw new Exception("Somente produtos calculados por Qtd. decimal podem possuir números decimais no campo Quantidade.");

            var qtdeAmbiente = produto.QtdeAmbiente > 0
                ? produto.QtdeAmbiente
                : 1;

            var custoCompra = produto.DadosProduto.CustoCompra();

            Calcular(
                sessao,
                produto,
                qtdeAmbiente,
                arredondarAluminio,
                calcularMultiploDe5,
                nf,
                numeroBeneficiamentos,
                alturaBeneficiamento,
                larguraBeneficiamento,
                compra,
                custoCompra,
                usarChapaVidro
            );
        }

        protected abstract void Calcular(GDASession sessao, IProdutoCalculo produto, int qtdeAmbiente,
            ArredondarAluminio arredondarAluminio, bool calcularMultiploDe5, bool nf, int numeroBeneficiamentos,
            int alturaBeneficiamento, int larguraBeneficiamento, bool compra, decimal custoCompra, bool usarChapaVidro);
    }
}
