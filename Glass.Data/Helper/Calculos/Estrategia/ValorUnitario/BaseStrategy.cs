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

        public decimal? Calcular(GDASession sessao, IProdutoCalculo produto, IContainerCalculo container,
            decimal total, ArredondarAluminio arredondarAluminio, bool calcMult5, bool nf, int numeroBenef,
            int alturaBenef, int larguraBenef)
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
                container,
                qtdeAmbiente,
                total,
                arredondarAluminio,
                calcMult5,
                nf,
                numeroBenef,
                alturaBenef,
                larguraBenef
            );
        }

        protected abstract decimal Calcular(GDASession sessao, IProdutoCalculo produto, IContainerCalculo container,
            int qtdeAmbiente, decimal total, ArredondarAluminio arredondarAluminio, bool calcMult5, bool nf, int numeroBenef,
            int alturaBenef, int larguraBenef);
    }
}
