using Glass.Data.Helper;

namespace Glass.Configuracoes
{
    partial class FinanceiroConfig
    {
        public class RelatorioContasPagar
        {
            /// <summary>
            /// Nome do arquivo do relatório.
            /// </summary>
            public static string NomeArquivoRelatorioContasPagar
            {
                get
                {
                    var config = Config.GetConfigItem<string>(Config.ConfigEnum.NomeArquivoRelatorioContasPagar);

                    if (!string.IsNullOrEmpty(config))
                        return config;

                    return "Relatorios/rptContasPagarRetrato.rdlc";
                }
            }
        }
    }
}