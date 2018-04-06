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

        public void Calcular(GDASession sessao, IProdutoCalculo produto, IContainerCalculo container,
            ArredondarAluminio arredondarAluminio, bool calcMult5, bool compra, bool nf, int numeroBenef,
            int alturaBenef, int larguraBenef, bool usarChapaVidro)
        {
            /* Chamado 41410. */
            if (ValidarQuantidadeDecimal && produto.Qtde % 1 > 0)
                throw new Exception("Somente produtos calculados por Qtd. decimal podem possuir números decimais no campo Quantidade.");

            var qtdeAmbiente = produto.QtdeAmbiente > 0
                ? produto.QtdeAmbiente
                : 1;

            var custoCompra = container.DadosProduto.CustoCompra(sessao, produto);

            Calcular(
                sessao,
                produto,
                container,
                qtdeAmbiente,
                arredondarAluminio,
                calcMult5,
                nf,
                numeroBenef,
                alturaBenef,
                larguraBenef,
                compra,
                custoCompra,
                usarChapaVidro
            );
        }

        protected abstract void Calcular(GDASession sessao, IProdutoCalculo produto, IContainerCalculo container,
            int qtdeAmbiente, ArredondarAluminio arredondarAluminio, bool calcMult5, bool nf, int numeroBenef,
            int alturaBenef, int larguraBenef, bool compra, decimal custoCompra, bool usarChapaVidro);
    }
}
