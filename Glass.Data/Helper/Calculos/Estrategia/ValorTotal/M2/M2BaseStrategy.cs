using Glass.Data.Model;
using GDA;
using Glass.Data.Helper.Calculos.Estrategia.ValorTotal.Enum;
using Glass.Configuracoes;

namespace Glass.Data.Helper.Calculos.Estrategia.ValorTotal.M2
{
    abstract class M2BaseStrategy<T> : BaseStrategy<T>
        where T : M2BaseStrategy<T>
    {
        protected abstract bool CalcularMultiploDe5 { get; }

        protected override void Calcular(GDASession sessao, IProdutoCalculo produto, IContainerCalculo container,
            int qtdeAmbiente, ArredondarAluminio arredondarAluminio, bool calcMult5, bool nf, int numeroBenef,
            int alturaBenef, int larguraBenef, bool compra, decimal custoCompra, bool usarChapaVidro)
        {
            float totM2Temp = produto.TotM;

            CalcularTotalM2(sessao, produto, container, calcMult5, compra);
            CalcularTotalM2Calculo(sessao, produto, container, qtdeAmbiente, calcMult5, numeroBenef, usarChapaVidro);

            if (PedidoConfig.NaoRecalcularValorProdutoComposicaoAoAlterarAlturaLargura
                && container.DadosProduto.AlturaProduto(sessao, produto) > 0
                && container.DadosProduto.LarguraProduto(sessao, produto) > 0
                && container.DadosProduto.TipoSubgrupo(sessao, produto) == TipoSubgrupoProd.VidroDuplo)
            {
                produto.Total = (decimal)produto.Qtde * produto.ValorUnit;
                return;
            }

            float totM2Preco = RecuperarTotalM2Preco(produto, nf, compra, totM2Temp);
            CalcularTotalM2CalculoAreaMinima(sessao, produto, container, qtdeAmbiente, nf, numeroBenef, compra, totM2Preco);

            produto.CustoProd = (decimal)produto.TotM * custoCompra;
            produto.Total = produto.ValorUnit * (decimal)produto.TotM2Calc;
        }

        private void CalcularTotalM2(GDASession sessao, IProdutoCalculo produto, IContainerCalculo container,
            bool calcMult5, bool compra)
        {
            if (!compra)
            {
                produto.TotM = CalculoM2.Instance.Calcular(
                    sessao,
                    produto,
                    container,
                    calcMult5 && CalcularMultiploDe5
                );
            }
        }

        private void CalcularTotalM2Calculo(GDASession sessao, IProdutoCalculo produto, IContainerCalculo container,
            int qtdeAmbiente, bool calcMult5, int numeroBenef, bool usarChapaVidro)
        {
            produto.TotM2Calc = CalculoM2.Instance.CalcularM2Calculo(
                sessao,
                produto,
                container,
                usarChapaVidro,
                calcMult5 && CalcularMultiploDe5,
                numeroBenef,
                qtdeAmbiente
            );
        }

        private float RecuperarTotalM2Preco(IProdutoCalculo produto, bool nf, bool compra, float totM2Temp)
        {
            float totM2Preco = !compra
                ? produto.TotM2Calc
                : produto.TotM;

            if (nf)
            {
                produto.TotM = totM2Temp;
                totM2Preco = totM2Temp;
            }

            return totM2Preco;
        }

        private void CalcularTotalM2CalculoAreaMinima(GDASession sessao, IProdutoCalculo produto, IContainerCalculo container,
            int qtdeAmbiente, bool nf, int numeroBenef, bool compra, float totM2Preco)
        {
            float m2Minimo = AreaMinima(sessao, produto, container, numeroBenef, nf, compra) * produto.Qtde * qtdeAmbiente;
            produto.TotM2Calc = totM2Preco < m2Minimo ? m2Minimo : totM2Preco;
        }

        private float AreaMinima(GDASession sessao, IProdutoCalculo produto, IContainerCalculo container, int numeroBenef,
            bool nf, bool compra)
        {
            return !nf && !compra && container.DadosProduto.CalcularAreaMinima(sessao, produto, numeroBenef)
                ? container.DadosProduto.AreaMinima(sessao, produto)
                : 0;
        }
    }
}
