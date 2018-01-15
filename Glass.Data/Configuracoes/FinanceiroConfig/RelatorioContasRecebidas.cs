using Glass.Data.Helper;

namespace Glass.Configuracoes
{
    partial class FinanceiroConfig
    {
        public class RelatorioContasRecebidas
        {
            /// <summary>
            /// Nome do arquivo do relatório.
            /// </summary>
            public static string NomeArquivoRelatorioContasRecebida
            {
                get
                {
                    var config = Config.GetConfigItem<string>(Config.ConfigEnum.NomeArquivoRelatorioContasRecebida);

                    if (string.IsNullOrEmpty(config))
                        config = RelatorioContasPagar.ExibirTipoContabilRetrato ? "Relatorios/rptContasRecebidasRetrato.rdlc" : "Relatorios/rptContasRecebidas.rdlc";

                    return config;
                }
            }

            public static bool ExibirPedidos
            {
                get { return Config.GetConfigItem<bool>(Config.ConfigEnum.ExibirPedidos); }
            }

            /// <summary>
            /// Define que a comissão será exibida na impressão do relatório de contas recebidas
            /// </summary>
            public static bool ExibirComissao
            {
                get { return Config.GetConfigItem<bool>(Config.ConfigEnum.ExibirComissao); }
            }
        }
    }
}