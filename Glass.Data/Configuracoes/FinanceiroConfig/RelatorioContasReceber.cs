using Glass.Data.Helper;

namespace Glass.Configuracoes
{
    partial class FinanceiroConfig
    {
        public class RelatorioContasReceber
        {
            /// <summary>
            /// Nome do arquivo do relatório.
            /// </summary>
            public static string NomeArquivoRelatorioContasReceber
            {
                get
                {
                    var config = Config.GetConfigItem<string>(Config.ConfigEnum.NomeArquivoRelatorioContasReceber);

                    if (string.IsNullOrEmpty(config))
                        config = RelatorioContasPagar.ExibirTipoContabilRetrato ? "Relatorios/rptContasReceberRetrato.rdlc" : "Relatorios/rptContasReceber.rdlc";

                    return config;
                }
            }

            /// <summary>
            /// Verifica se deve ser exibido o campo PedCli no relatório
            /// </summary>
            public static bool ExibirPedCli
            {
                get { return Config.GetConfigItem<bool>(Config.ConfigEnum.ExibirPedCli); }
            }
        }
    }
}
