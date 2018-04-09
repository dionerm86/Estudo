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
            int alturaBeneficiamento, int larguraBeneficiamento, bool compra, decimal custoCompra, bool usarChapaVidro)
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

            var baseCalculo = qtdeAmbiente
                * (decimal)alturaArredondada
                * (decimal)produto.Qtde;

            if (alturaArredondada < TAMANHO_BARRA_ALUMINIO_EM_M)
            {
                var fatorMultiplicacao = (decimal)alturaArredondada
                    % TAMANHO_BARRA_ALUMINIO_EM_M
                    / TAMANHO_BARRA_ALUMINIO_EM_M;

                produto.Total = produto.ValorUnit
                    * fatorMultiplicacao
                    * qtdeAmbiente
                    * (decimal)produto.Qtde;
            }
            else
            {
                produto.Total = produto.ValorUnit / TAMANHO_BARRA_ALUMINIO_EM_M * baseCalculo;
            }

            produto.CustoProd = custoCompra / TAMANHO_BARRA_ALUMINIO_EM_M * baseCalculo;
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
    }
}
