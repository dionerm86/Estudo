using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Colosoft;

namespace Glass.Rentabilidade
{
    /// <summary>
    /// Representa o resultado da rentabilidade.
    /// </summary>
    public class ResultadoRentabilidade
    {
        #region Propriedades

        /// <summary>
        /// Indicadores financeiros usados.
        /// </summary>
        public IDictionary<string, decimal> IndicadoresFinanceiros { get; }

        /// <summary>
        /// Variáveis do item.
        /// </summary>
        public IDictionary<string, decimal> VariaveisItem { get; }

        /// <summary>
        /// Expressões do calculo.
        /// </summary>
        public IDictionary<string, decimal> ExpressoesCalculo { get; }

        /// <summary>
        /// Item.
        /// </summary>
        public IItemRentabilidade Item { get; }

        /// <summary>
        /// Custo total.
        /// </summary>
        public decimal CustoTotal { get; }

        /// <summary>
        /// Valor da rentabilidade.
        /// </summary>
        public decimal RentabilidadeFinanceira => Item.PrecoVendaSemIPI + CustoTotal;

        /// <summary>
        /// Percentual da rentabilidade.
        /// </summary>
        public decimal PercentualRentabilidade => Item.PrecoVendaSemIPI > 0M ? RentabilidadeFinanceira / Item.PrecoVendaSemIPI : 0M;

        #endregion

        #region Construtores

        /// <summary>
        /// Construtor padrão.
        /// </summary>
        /// <param name="indicadoresFinanceiros"></param>
        /// <param name="variaveisItem"></param>
        /// <param name="expressoesCalculo"></param>
        /// <param name="custoTotal"></param>
        /// <param name="item"></param>
        internal ResultadoRentabilidade(
            Mathematical.IVariavelCollection indicadoresFinanceiros,
            Mathematical.IVariavelCollection variaveisItem,
            Mathematical.IVariavelCollection expressoesCalculo,
            double custoTotal, IItemRentabilidade item)
        {
            item.Require(nameof(item)).NotNull();

            IndicadoresFinanceiros = new Dictionary<string, decimal>();
            VariaveisItem = new Dictionary<string, decimal>();
            ExpressoesCalculo = new Dictionary<string, decimal>();

            foreach (var i in indicadoresFinanceiros)
                IndicadoresFinanceiros.Add(i.Nome, (decimal)i.ObterValor());

            foreach (var i in variaveisItem)
                VariaveisItem.Add(i.Nome, (decimal)i.ObterValor());

            foreach (var i in expressoesCalculo)
                ExpressoesCalculo.Add(i.Nome, (decimal)i.ObterValor());

            CustoTotal = (decimal)custoTotal;
            Item = item;
        }

        #endregion

        #region Métodos Privados

        /// <summary>
        /// Cria os registro associado com o resultado.
        /// </summary>
        /// <param name="factory"></param>
        /// <returns></returns>
        private IEnumerable<IRegistroRentabilidade> CriarRegistros()
        {
            foreach (var indicador in IndicadoresFinanceiros)
                yield return Item.CriarRegistro(TipoRegistroRentabilidade.IndicadorFinaceiro, indicador.Key, indicador.Value);

            foreach (var variavel in VariaveisItem)
                yield return Item.CriarRegistro(TipoRegistroRentabilidade.VariavelItem, variavel.Key, variavel.Value);

            foreach (var expressao in ExpressoesCalculo)
                yield return Item.CriarRegistro(TipoRegistroRentabilidade.Expressao, expressao.Key, expressao.Value);
        }

        #endregion

        #region Métodos Públicos

        /// <summary>
        /// Realiza a aplica do resultado da rentabiliade sobre o item associado.
        /// </summary>
        public void AplicarResultado()
        {
            var registros = CriarRegistros().ToList();

            Item.PercentualRentabilidade = PercentualRentabilidade;
            Item.RentabilidadeFinanceira = RentabilidadeFinanceira;

            // Recupera a coleção de registros do item no formato esperado
            var registrosItem = Item.RegistrosRentabilidade as System.Collections.IList;

            if (registrosItem != null)
            {
                var registrosProcessados = new List<IRegistroRentabilidade>();

                foreach (var registro in registros)
                {
                    // Caso o registro ainda não esteja associado
                    if (!registrosItem.Contains(registro))
                        registrosItem.Add(registro);

                    registrosProcessados.Add(registro);
                }

                // Remove os registros que não são mais usados
                for (var i = 0; i < registrosItem.Count; i++)
                    if (!registrosProcessados.Contains(registrosItem[i]))
                        registrosItem.RemoveAt(i--);

            }
        }

        #endregion
    }
}
