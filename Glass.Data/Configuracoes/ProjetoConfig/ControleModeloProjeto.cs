using Glass.Data.Helper;

namespace Glass.Configuracoes
{
    public partial class ProjetoConfig
    {
        public static class ControleModeloProjeto
        {
            /// <summary>
            /// Define se será obrigatório informar a cor do alumínio e da ferragem no projeto.
            /// </summary>
            public static bool ObrigarInformarCorAlumFerragem
            {
                get { return Config.GetConfigItem<bool>(Config.ConfigEnum.ObrigatorioCorAlumFerragem); }
            }

            /// <summary>
            /// Os modelos de projeto vem com a opção "Apenas Vidros" selecionada por padrão?
            /// </summary>
            public static bool ApenasVidrosPadrao
            {
                get { return Config.GetConfigItem<bool>(Config.ConfigEnum.ApenasVidrosPadrao); }
            }

            /// <summary>
            /// Define se a opção medida exata deverá ser marcada por padrão.
            /// </summary>
            public static bool MedidaExataPadrao
            {
                get
                {
                    if (!ApenasVidrosPadrao)
                        return false;

                    return Config.GetConfigItem<bool>(Config.ConfigEnum.MedidaExataPadrao);
                }
            }

            /// <summary>
            /// Define se apenas o Admin Sync pode ativar modelo de projeto
            /// </summary>
            public static bool ApenasAdminSyncAtivarModeloProjeto
            {
                get { return Config.GetConfigItem<bool>(Config.ConfigEnum.ApenasAdminSyncAtivarModeloProjeto); }
            }
        }
    }
}
