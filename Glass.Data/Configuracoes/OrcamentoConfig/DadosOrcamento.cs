using Glass.Data.Helper;

namespace Glass.Configuracoes
{
    public partial class OrcamentoConfig
    {
        public static class DadosOrcamento
        {
            public static string FormaPagtoOrcamento
            {
                get { return Config.GetConfigItem<string>(Config.ConfigEnum.FormaPagtoOrcamento); }
            }

            public static string PrazoEntregaOrcamento
            {
                get { return Config.GetConfigItem<string>(Config.ConfigEnum.PrazoEntregaOrcamento); }
            }

            public static string ValidadeOrcamento
            {
                get { return Config.GetConfigItem<string>(Config.ConfigEnum.ValidadeOrcamento); }
            }

            public static bool ListaApenasOrcamentosVendedor
            {
                get { return Config.GetConfigItem<bool>(Config.ConfigEnum.ListaApenasOrcamentosVendedor); }
            }
        }
    }
}
