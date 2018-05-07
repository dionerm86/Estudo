using Glass.Data.Model;
using GDA;
using Glass.Data.Helper.Calculos.Estrategia.ValorTotal.Enum;

namespace Glass.Data.Helper.Calculos.Estrategia.ValorTotal.MLAL
{
    abstract class MLALBaseStrategy<T> : BaseStrategy<T>
        where T : MLALBaseStrategy<T>
    {
        private const int TAMANHO_BARRA_ALUMINIO_EM_M = 6;

        protected abstract float ValorArredondar { get; }

        protected override void Calcular(GDASession sessao, IProdutoCalculo produto, int qtdeAmbiente,
            ArredondarAluminio arredondarAluminio, bool calcularMultiploDe5, bool nf, int numeroBeneficiamentos,
            int alturaBeneficiamento, int larguraBeneficiamento, bool compra, decimal custoCompra, bool usarChapaVidro,
            bool valorBruto)
        {
            float decimosAltura = produto.Altura - (int)produto.Altura;
            float alturaArredondada = produto.Altura;

            if (!produto.DadosProduto.DadosGrupoSubgrupo.ProdutoEAluminio())
            {
                arredondarAluminio = ArredondarAluminio.NaoArredondar;
            }

            if (arredondarAluminio != ArredondarAluminio.NaoArredondar && ValorArredondar > 0)
            {
                alturaArredondada = Arredondar(produto, decimosAltura) ?? alturaArredondada;

                if (arredondarAluminio == ArredondarAluminio.ArredondarEAtualizarProduto)
                {
                    produto.Altura = alturaArredondada;
                }
            }

            produto.CustoProd = custoCompra * ObterBaseCalculo(produto, (decimal)alturaArredondada, qtdeAmbiente, false);
            var baseCalculo = ObterBaseCalculo(produto, (decimal)alturaArredondada, qtdeAmbiente, true);

            if (!valorBruto)
                produto.Total = produto.ValorUnit * baseCalculo;
            else
                produto.TotalBruto = produto.ValorUnitarioBruto * baseCalculo;
        }

        protected virtual float? Arredondar(IProdutoCalculo produto, float decimosAltura)
        {
            float? alturaArredondada = null;
            int alturaInteira = (int)produto.Altura;

            if (decimosAltura > 0 && decimosAltura < ValorArredondar)
            {
                alturaArredondada = alturaInteira + ValorArredondar;
            }
            else if (decimosAltura > ValorArredondar)
            {
                alturaArredondada = alturaInteira + (ValorArredondar * 2);
            }

            return alturaArredondada;
        }

        private decimal ObterBaseCalculo(IProdutoCalculo produto, decimal alturaArredondada, int qtdeAmbiente, bool venda)
        {
            var baseCalculo = alturaArredondada
                / TAMANHO_BARRA_ALUMINIO_EM_M
                * qtdeAmbiente
                * (decimal)produto.Qtde;

            if (venda && alturaArredondada < TAMANHO_BARRA_ALUMINIO_EM_M)
            {
                var fatorMultiplicacao = alturaArredondada
                    % TAMANHO_BARRA_ALUMINIO_EM_M
                    / TAMANHO_BARRA_ALUMINIO_EM_M;

                baseCalculo = fatorMultiplicacao
                    * qtdeAmbiente
                    * (decimal)produto.Qtde;
            }

            return baseCalculo;
        }
    }
}
