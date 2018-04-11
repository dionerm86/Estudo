using System.Collections.Generic;
using Glass.Data.Helper;

namespace Glass.Configuracoes
{
    public partial class ProdutoConfig
    {
        /// <summary>
        /// Classe com as configurações da tela de preço de tabela por cliente (relatório).
        /// </summary>
        public static class TelaPrecoTabelaClienteRelatorio
        {
            /// <summary>
            /// Alteração dos valores dos subgrupos selecionados ao abrir a tela.
            /// (se houver alteração, mudar o key pelo value)
            /// </summary>
            public static KeyValuePair<string, string> AlterarSubgruposSelecionados
            {
                get
                {
                    var config = Config.GetConfigItem<string>(Config.ConfigEnum.AlterarSubgruposSelecionados);

                    if (string.IsNullOrEmpty(config))
                        return new KeyValuePair<string, string>();

                    var dadosConfig = config.Split('|');

                    return new KeyValuePair<string, string>(dadosConfig[0], dadosConfig[1]);
                }
            }
            
            /// <summary>
            /// Subgrupos que devem vir marcados por padrão na tela.
            /// </summary>
            public static string SubgruposPadraoFiltro
            {
                get { return Config.GetConfigItem<string>(Config.ConfigEnum.SubgruposPadraoFiltro); }
            }

            /// <summary>
            /// Subgrupos que devem vir marcados por padrão na tela.
            /// </summary>
            public static bool UsarRelatorioPrecoTabelClienteRetrato
            {
                get { return Config.GetConfigItem<bool>(Config.ConfigEnum.UsarRelatorioPrecoTabelClienteRetrato); }
            }
        }
    }
}
