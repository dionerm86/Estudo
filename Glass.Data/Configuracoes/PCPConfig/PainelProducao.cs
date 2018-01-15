using Glass.Data.Helper;

namespace Glass.Configuracoes
{
    public partial class PCPConfig
    {
        public static class PainelProducao
        {
            /// <summary>
            /// Verifica se no rodapé deve ser exibido o total de peças que foram lidas em cada setor, no dia atual.
            /// </summary>
            public static bool ExibirTotalM2LidoNoDia
            {
                get
                {
                    if (System.Configuration.ConfigurationManager.AppSettings["ExibirTotalLidoNoDia"] == "true")
                        return true;

                    return Config.GetConfigItem<bool>(Config.ConfigEnum.ExibirTotalM2LidoNoDia);
                }
            }
 
            /// <summary>
            /// Verifica se no rodapé deve ser exibido o total de peças que foram lidas em cada setor, no dia atual.
            /// </summary>
            public static bool ExibirTotalQtdeLidoNoDia
            {
                get { return Config.GetConfigItem<bool>(Config.ConfigEnum.ExibirTotalQtdeLidoNoDia); }
            }

            /// <summary>
            /// Define se as perdas de chapa serão contabilizadas no total de perda dos painéis.
            /// </summary>
            public static bool ContabilizarPerdaChapaVidroNoTotalDePerdaPainel
            {
                get { return Config.GetConfigItem<bool>(Config.ConfigEnum.ContabilizarPerdaChapaVidroNoTotalDePerdaPainel); }
            }
        }
    }
}