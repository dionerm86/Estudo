using Glass.Data.Model;
using GDA;
using Glass.Data.Helper.Calculos.Estrategia.ValorTotal.Enum;
using Glass.Configuracoes;

namespace Glass.Data.Helper.Calculos.Estrategia.ValorTotal.M2
{
    abstract class M2BaseStrategy<T> : BaseStrategy<T>
        where T : M2BaseStrategy<T>
    {
        protected abstract bool DeveCalcularMultiploDe5 { get; }

        protected override void Calcular(GDASession sessao, IProdutoCalculo produto, int qtdeAmbiente,
            ArredondarAluminio arredondarAluminio, bool calcularMultiploDe5, bool nf, int numeroBeneficiamentos,
            int alturaBeneficiamento, int larguraBeneficiamento, bool compra, decimal custoCompra, bool usarChapaVidro,
            bool valorBruto)
        {
            float totM2Temp = produto.TotM;

            CalcularTotalM2(sessao, produto, calcularMultiploDe5, compra);
            CalcularTotalM2Calculo(sessao, produto, qtdeAmbiente, calcularMultiploDe5, numeroBeneficiamentos, usarChapaVidro);

            if (PedidoConfig.NaoRecalcularValorProdutoComposicaoAoAlterarAlturaLargura
                && produto.DadosProduto.AlturaProduto() > 0
                && produto.DadosProduto.LarguraProduto() > 0
                && produto.DadosProduto.DadosGrupoSubgrupo.TipoSubgrupo() == TipoSubgrupoProd.VidroDuplo)
            {
                produto.Total = (decimal)produto.Qtde * produto.ValorUnit;
                return;
            }

            if (compra && produto.DadosProduto.DadosGrupoSubgrupo.TipoCalculo(false, compra) == TipoCalculoGrupoProd.Qtd)
            {
                produto.Total = custoCompra * (decimal)produto.Qtde;
                return;
            }

            float totM2Preco = RecuperarTotalM2Preco(produto, nf, compra, totM2Temp);
            CalcularTotalM2CalculoAreaMinima(produto, qtdeAmbiente, nf, numeroBeneficiamentos, compra, totM2Preco);

            produto.CustoProd = (decimal)produto.TotM * custoCompra;

            if (!valorBruto)
            {
                produto.Total = produto.ValorUnit * (decimal)produto.TotM2Calc;
            }
            else
            {
                produto.TotalBruto = produto.ValorUnitarioBruto * (decimal)produto.TotM2Calc;
            }
        }

        private void CalcularTotalM2(GDASession sessao, IProdutoCalculo produto, bool calcMult5, bool compra)
        {
            if (!compra)
            {
                produto.TotM = CalculoM2.Instance.Calcular(sessao, produto.Container, produto,
                    calcMult5 && DeveCalcularMultiploDe5);
            }
        }

        private void CalcularTotalM2Calculo(GDASession sessao, IProdutoCalculo produto, int qtdeAmbiente,
            bool calcularMultiploDe5, int numeroBeneficiamentos, bool usarChapaVidro)
        {
            produto.TotM2Calc = CalculoM2.Instance.CalcularM2Calculo(
                sessao,
                produto.Container,
                produto,
                usarChapaVidro,
                calcularMultiploDe5 && DeveCalcularMultiploDe5,
                numeroBeneficiamentos,
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

        private void CalcularTotalM2CalculoAreaMinima(IProdutoCalculo produto, int qtdeAmbiente, bool nf,
            int numeroBeneficiamentos, bool compra, float totM2Preco)
        {
            float m2Minimo = AreaMinima(produto, numeroBeneficiamentos, nf, compra) * produto.Qtde * qtdeAmbiente;
            produto.TotM2Calc = totM2Preco < m2Minimo ? m2Minimo : totM2Preco;
        }

        private float AreaMinima(IProdutoCalculo produto, int numeroBeneficiamentos, bool nf, bool compra)
        {
            return !nf && !compra && produto.DadosProduto.CalcularAreaMinima(numeroBeneficiamentos)
                ? produto.DadosProduto.AreaMinima()
                : 0;
        }
    }
}
