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

                    return ExibirTipoContabilRetrato ? "Relatorios/rptContasPagarRetrato.rdlc" : "Relatorios/rptContasPagar.rdlc";
                }
            }

            /// <summary>
            /// Exibir relatório de contas a pagar/pagas/receber/recebidas próprio para controle contábil
            /// </summary>
            public static bool ExibirTipoContabilRetrato
            {
                get
                {
                    var config = Config.GetConfigItem<bool>(Config.ConfigEnum.ExibirTipoContabilRetrato);

                    return config && !SepararValoresFiscaisEReaisContasPagar;
                }
            }
        }
    }
}