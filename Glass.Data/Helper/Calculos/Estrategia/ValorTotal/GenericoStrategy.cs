using GDA;
using Glass.Data.Helper.Calculos.Estrategia.ValorTotal.Enum;
using Glass.Data.Model;

namespace Glass.Data.Helper.Calculos.Estrategia.ValorTotal
{
    class GenericoStrategy : QtdBaseStrategy<GenericoStrategy>
    {
        private GenericoStrategy() { }

        protected override void Calcular(GDASession sessao, IProdutoCalculo produto, int qtdeAmbiente,
            ArredondarAluminio arredondarAluminio, bool calcularMultiploDe5, bool nf, int numeroBeneficiamentos,
            int alturaBeneficiamento, int larguraBeneficiamento, bool compra, decimal custoCompra, bool usarChapaVidro)
        {
            CalculaTotalM2(sessao, produto, calcularMultiploDe5, numeroBeneficiamentos);

            base.Calcular(
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

        private void CalculaTotalM2(GDASession sessao, IProdutoCalculo produto, bool calcMult5, int numeroBenef)
        {
            if (produto.Altura > 0 && produto.Largura > 0 && 
                (produto.TotM == 0 || produto.DadosProduto.DadosGrupoSubgrupo.ProdutoEVidro()))
            {
                var quantidadeOriginalProduto = produto.Qtde;
                var quantidadeCalcularM2 = CalcularQuantidadeProdutoModulado(produto);

                // Caso o produto seja chapa (altura ou largura > 2500), seja vendido por qtd e seja produto de produção, 
                // não calcula múltiplo de 5
                calcMult5 = !(produto.Altura > 2500 || produto.Largura > 2500) && calcMult5;

                try
                {
                    produto.Qtde = quantidadeCalcularM2;
                    produto.TotM = CalculoM2.Instance.Calcular(sessao, produto.Container, produto, calcMult5);
                    produto.TotM2Calc = CalculoM2.Instance.CalcularM2Calculo(sessao, produto.Container, produto, true,
                        calcMult5, numeroBenef);
                }
                finally
                {
                    produto.Qtde = quantidadeOriginalProduto;
                }
            }
        }

        private float CalcularQuantidadeProdutoModulado(IProdutoCalculo produto)
        {
            var quantidade = produto.Qtde;
            var tipoSubgrupo = produto.DadosProduto.DadosGrupoSubgrupo.TipoSubgrupo();
            
            if (tipoSubgrupo == TipoSubgrupoProd.Modulado)
                foreach (var quantidadeBaixaEstoque in produto.DadosProduto.DadosBaixaEstoque.QuantidadesBaixaEstoque())
                    quantidade *= quantidadeBaixaEstoque;

            return quantidade;
        }
    }
}
