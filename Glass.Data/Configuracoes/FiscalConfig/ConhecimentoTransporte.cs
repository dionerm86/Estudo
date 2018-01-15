using Glass.Data.Helper;

namespace Glass.Configuracoes
{
    public partial class FiscalConfig
    {
        public static class ConhecimentoTransporte
        {
            #region CTe

            /// <summary>
            /// Indica se a empresa usará a contingência de CT-e.
            /// </summary>
            public static DataSources.TipoContingenciaCTe ContingenciaCTe
            {
                get { return Config.GetConfigItem<DataSources.TipoContingenciaCTe>(Config.ConfigEnum.ContingenciaCTe); }
            }

            /// <summary>
            /// Retorna a justificativa da contingência do CT-e.
            /// </summary>
            public static string JustificativaContingenciaCTE
            {
                get { return Config.GetConfigItem<string>(Config.ConfigEnum.JustificativaContingenciaCTe); }
            }

            /// <summary>
            /// Retorna qual participante deve ser associado à conta a pagar.
            /// </summary>
            public static bool ExibirGridNotaFiscal
            {
                get { return Config.GetConfigItem<bool>(Config.ConfigEnum.ExibirGridNotaFiscal); }
            }
 
            /// <summary>
            /// Define se a logomarca da empresa será exibida no Dacte.
            /// </summary>
            public static bool ExibirLogomarcaNoDacte
            {
                get { return Config.GetConfigItem<bool>(Config.ConfigEnum.ExibirLogomarcaNoDacte); }
            }

            /// <summary>
            /// Define que o complemento do CTe e alguns dados do transporte rodoviário não serão exibidos caso seja de CT-e de saída
            /// </summary>
            public static bool EsconderComplEDadosTransRodCteSaida
            {
                get { return Config.GetConfigItem<bool>(Config.ConfigEnum.EsconderComplEDadosTransRodCteSaida); }
            }

            #endregion
        }
    }
}
