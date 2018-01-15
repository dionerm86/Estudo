using Glass.Data.Helper;

namespace Glass.Configuracoes
{
    public partial class PCPConfig
    {
        public static class Etiqueta
        {
            public static bool AgruparEtiquetaCorEspessura
            {
                get { return Config.GetConfigItem<bool>(Config.ConfigEnum.AgruparEtiquetaCorEspessura); }
            }

            /// <summary>
            /// Define se o arquivo SAG, da mesa de corte, será gerado juntamente com arquivo de etiquetas para otimização
            /// </summary>
            public static bool GerarArquivoMesaCorte
            {
                get { return Config.GetConfigItem<bool>(Config.ConfigEnum.GerarArquivoMesaCorte); }
            }

            /// <summary>
            /// Verifica se a empresa trabalha com otimização de chapa de vidro e consequentemente plano de corte,
            /// utilizando para isso o Corte Certo ou o Opty-Way
            /// </summary>
            public static bool UsarPlanoCorte
            {
                get { return Config.GetConfigItem<bool>(Config.ConfigEnum.UsarPlanoCorte); }
            }

            /// <summary>
            /// Diferença, em dias úteis, entre a data de fábrica e a data de entrega.
            /// </summary>
            public static int DiasDataFabrica
            {
                get { return Config.GetConfigItem<int>(Config.ConfigEnum.DiasDataFabrica); }
            }

            public static int FolgaRetalho
            {
                get { return Config.GetConfigItem<int>(Config.ConfigEnum.FolgaRetalho); }
            }

            /// <summary>
            /// Usar controle de corte por chapa?
            /// </summary>
            public static bool UsarControleChapaCorte
            {
                get { return Config.GetConfigItem<bool>(Config.ConfigEnum.UsarControleChapaCorte); }
            }

            /// <summary>
            /// Usar controle de retalhos?
            /// </summary>
            public static bool UsarControleRetalhos
            {
                get { return Config.GetConfigItem<bool>(Config.ConfigEnum.UsarControleRetalhos); }
            }

            /// <summary>
            /// Define que o nome do arquivo FML será registrado no banco
            /// </summary>
            public static bool RegistrarNomeArquivoFml
            {
                get { return Config.GetConfigItem<bool>(Config.ConfigEnum.RegistrarNomeArquivoFml); }
            }

            /// <summary>
            /// Define que o campo altura/largura será destacado se a aresta da peça for 0 ou se na descrição do benef tiver "cnc"
            /// </summary>
            public static bool DestacarAlturaLarguraSeAresta0OuCNC
            {
                get { return Config.GetConfigItem<bool>(Config.ConfigEnum.DestacarAlturaLarguraSeAresta0OuCNC); }
            }

            /// <summary>
            /// Define que as etiquetas exportadas para o optyway irão ordenadas por cor e espessura
            /// </summary>
            public static bool ExportarEtiquetasOptywayOrdenadasCorEEspessura
            {
                get { return Config.GetConfigItem<bool>(Config.ConfigEnum.ExportarEtiquetasOptywayOrdenadasCorEEspessura); }
            }
        }
    }
}
