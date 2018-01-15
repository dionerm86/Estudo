using System;
using Glass.Data.Helper;
using Glass.Data.Model;
using Glass.Data.DAL;

namespace Glass.Configuracoes
{
    public partial class OrcamentoConfig
    {
        /// <summary>
        /// Classe com as
        /// </summary>
        public class RelatorioOrcamento
        {
            /// <summary>
            /// Define se apenas administrador poderá imprimir orçamento.
            /// </summary>
            public static bool ApenasAdminImprimeOrcamento
            {
                get { return Config.GetConfigItem<bool>(Config.ConfigEnum.ApenasAdminImprimeOrcamento); }
            }

            /// <summary>
            /// Define se ao imprimir o orçamento, o sistema irá ordenar os ambientes pelo numSeq e agrupar pelo item
            /// </summary>
            public static bool OrdenarAgruparProdutosPorNumSeqItem
            {
                get { return Config.GetConfigItem<bool>(Config.ConfigEnum.OrdenarAgruparProdutosPorNumSeqItem); }
            }

            /// <summary>
            /// Chamado 33621: Define se ao calcular o total de acréscimo do orçamento será exibido diretamente, sem cálculos, o valor inserido no orçamento
            /// </summary>
            public static bool UsarValorPercTextoAcrescimo
            {
                get { return Config.GetConfigItem<bool>(Config.ConfigEnum.UsarValorPercTextoAcrescimo); }
            }
        }
    }
}
