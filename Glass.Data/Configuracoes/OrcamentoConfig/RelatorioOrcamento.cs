using System;
using Glass.Data.Helper;
using Glass.Data.Model;
using Glass.Data.DAL;

namespace Glass.Configuracoes
{
    public partial class OrcamentoConfig
    {
        /// <summary>
        /// Classe com as
        /// </summary>
        public class RelatorioOrcamento
        {
            /// <summary>
            /// Define se apenas administrador poderá imprimir orçamento.
            /// </summary>
            public static bool ApenasAdminImprimeOrcamento
            {
                get { return Config.GetConfigItem<bool>(Config.ConfigEnum.ApenasAdminImprimeOrcamento); }
            }
        }
    }
}
