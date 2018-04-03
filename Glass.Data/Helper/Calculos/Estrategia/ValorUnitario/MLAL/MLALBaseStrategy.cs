using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Glass.Data.Model;
using Glass.Data.DAL;

namespace Glass.Data.Helper.Calculos.Estrategia.ValorUnitario.MLAL
{
    abstract class MLALBaseStrategy<T> : BaseStrategy<T>
        where T : MLALBaseStrategy<T>
    {
        private const int TAMANHO_BARRA_ALUMINIO_EM_M = 6;

        protected abstract float ValorArredondar { get; }

        protected override decimal Calcular(IProdutoCalculo produto, IContainerCalculo container, int qtdeAmbiente,
            decimal total, bool arredondarAluminio, bool calcMult5, bool nf, int numeroBenef,
            bool calcularAreaMinima, int alturaBenef, int larguraBenef)
        {
            float decimosAltura = produto.Altura - (int)produto.Altura;
            float alturaArredondada = produto.Altura;

            var idGrupoProd = ProdutoDAO.Instance.ObtemIdGrupoProd(null, (int)produto.IdProduto);
            if (!GrupoProdDAO.Instance.IsAluminio((int)idGrupoProd))
            {
                arredondarAluminio = false;
            }

            if (arredondarAluminio && ValorArredondar > 0)
            {
                alturaArredondada = Arredondar(produto, decimosAltura) ?? alturaArredondada;
            }

            if (alturaArredondada == TAMANHO_BARRA_ALUMINIO_EM_M)
            {
                return CalculoValorUnitarioBarraInteira(produto, qtdeAmbiente, total);
            }
            else if (alturaArredondada < TAMANHO_BARRA_ALUMINIO_EM_M)
            {
                var divisor = alturaArredondada > 0 ? (decimal)alturaArredondada : 1;
                return CalculoValorUnitarioBarraInteira(produto, qtdeAmbiente, total) / divisor * TAMANHO_BARRA_ALUMINIO_EM_M;
            }
            else
            {
                float divisorFloat = alturaArredondada * produto.Qtde * qtdeAmbiente;
                decimal divisor = divisorFloat > 0 ? (decimal)divisorFloat : 1;
                return total / divisor * TAMANHO_BARRA_ALUMINIO_EM_M;
            }
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

        private decimal CalculoValorUnitarioBarraInteira(IProdutoCalculo produto, int qtdeAmbiente, decimal total)
        {
            var qtde = produto.Qtde > 0 ? produto.Qtde : 1;
            return total / (decimal)(qtde * qtdeAmbiente);
        }
    }
}
