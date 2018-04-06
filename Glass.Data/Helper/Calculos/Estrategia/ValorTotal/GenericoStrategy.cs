using GDA;
using Glass.Data.Helper.Calculos.Estrategia.ValorTotal.Enum;
using Glass.Data.Model;

namespace Glass.Data.Helper.Calculos.Estrategia.ValorTotal
{
    class GenericoStrategy : QtdBaseStrategy<GenericoStrategy>
    {
        private GenericoStrategy() { }

        protected override void Calcular(GDASession sessao, IProdutoCalculo produto, IContainerCalculo container,
            int qtdeAmbiente, ArredondarAluminio arredondarAluminio, bool calcMult5, bool nf, int numeroBenef,
            int alturaBenef, int larguraBenef, bool compra, decimal custoCompra, bool usarChapaVidro)
        {
            CalculaTotalM2(sessao, produto, container, calcMult5, numeroBenef);

            base.Calcular(
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

        private void CalculaTotalM2(GDASession sessao, IProdutoCalculo produto, IContainerCalculo container,
            bool calcMult5, int numeroBenef)
        {
            if (produto.Altura > 0 && produto.Largura > 0 && 
                (produto.TotM == 0 || container.DadosProduto.ProdutoEVidro(sessao, produto)))
            {
                var quantidadeOriginalProduto = produto.Qtde;
                var quantidadeCalcularM2 = CalcularQuantidadeProdutoModulado(sessao, produto, container);

                // Caso o produto seja chapa (altura ou largura > 2500), seja vendido por qtd e seja produto de produção, 
                // não calcula múltiplo de 5
                calcMult5 = !(produto.Altura > 2500 || produto.Largura > 2500) && calcMult5;

                try
                {
                    produto.Qtde = quantidadeCalcularM2;
                    produto.TotM = CalculoM2.Instance.Calcular(sessao, produto, container, calcMult5);
                    produto.TotM2Calc = CalculoM2.Instance.CalcularM2Calculo(sessao, produto, container, true, calcMult5, numeroBenef);
                }
                finally
                {
                    produto.Qtde = quantidadeOriginalProduto;
                }
            }
        }

        private float CalcularQuantidadeProdutoModulado(GDASession sessao, IProdutoCalculo produto,
            IContainerCalculo container)
        {
            var quantidade = produto.Qtde;
            var tipoSubgrupo = container.DadosProduto.TipoSubgrupo(sessao, produto);
            
            if (tipoSubgrupo == TipoSubgrupoProd.Modulado)
                foreach (var quantidadeBaixaEstoque in container.DadosProduto.QuantidadesProdutosBaixaEstoque(sessao, produto))
                    quantidade *= quantidadeBaixaEstoque;

            return quantidade;
        }
    }
}
