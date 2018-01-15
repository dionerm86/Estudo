using Glass.Data.Helper;

namespace Glass.Configuracoes
{
    public partial class OrcamentoConfig
    {
        /// <summary>
        /// Classe com as configurações do relatório de orçamento rápido.
        /// </summary>
        public static class RelatorioOrcamentoRapido
        {
            /// <summary>
            /// Exibir o logotipo no relatório?
            /// </summary>
            public static bool ExibirLogotipo
            {
                get { return Config.GetConfigItem<bool>(Config.ConfigEnum.ExibirLogotipo); }
            }
        }
    }
}
