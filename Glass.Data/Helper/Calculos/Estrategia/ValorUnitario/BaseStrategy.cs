using GDA;
using Glass.Data.Helper.Calculos.Estrategia.ValorTotal.Enum;
using Glass.Data.Model;
using Glass.Pool;
using System;

namespace Glass.Data.Helper.Calculos.Estrategia.ValorUnitario
{
    abstract class BaseStrategy<T> : Singleton<T>, IValorUnitarioStrategy
        where T : BaseStrategy<T>
    {
        protected virtual bool ValidarQuantidadeDecimal
        {
            get { return true; }
        }

        public decimal? Calcular(GDASession sessao, IProdutoCalculo produto, decimal total, ArredondarAluminio arredondarAluminio,
            bool calcularMultiploDe5, bool nf, int numeroBeneficiamentos, int alturaBeneficiamento,
            int larguraBeneficiamento)
        {
            /* Chamado 41410. */
            if (ValidarQuantidadeDecimal && produto.Qtde % 1 > 0)
                throw new Exception("Somente produtos calculados por Qtd. decimal podem possuir números decimais no campo Quantidade.");

            var qtdeAmbiente = produto.QtdeAmbiente > 0
                ? produto.QtdeAmbiente
                : 1;

            return Calcular(
                sessao,
                produto,
                qtdeAmbiente,
                total,
                arredondarAluminio,
                calcularMultiploDe5,
                nf,
                numeroBeneficiamentos,
                alturaBeneficiamento,
                larguraBeneficiamento
            );
        }

        protected abstract decimal Calcular(GDASession sessao, IProdutoCalculo produto, int qtdeAmbiente, decimal total,
            ArredondarAluminio arredondarAluminio, bool calcularMultiploDe5, bool nf, int numeroBeneficiamentos,
            int alturaBeneficiamento, int larguraBeneficiamento);
    }
}
