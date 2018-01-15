using Glass.Data.Helper;

namespace Glass.Configuracoes
{
    public partial class ProjetoConfig
    {
        /// <summary>
        /// Classe com as configurações do relatório de imagem de projeto.
        /// </summary>
        public static class RelatorioImagemProjeto
        {
            /// <summary>
            /// Define o percentual de tamanho da imagem no relatório.
            /// </summary>
            public static float PercentualTamanhoImagemRelatorio
            {
                get { return Config.GetConfigItem<float>(Config.ConfigEnum.PercentualTamanhoImagemRelatorio); }
            }

            /// <summary>
            /// Retorna a cor da observação que será usada no relatório.
            /// </summary>
            public static string CorObsNoRelatorio
            {
                get { return Config.GetConfigItem<string>(Config.ConfigEnum.CorObsNoRelatorio); }
            }
        }
    }
}
