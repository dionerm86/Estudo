using Glass.Data.Helper;

namespace Glass.Configuracoes
{
    partial class FinanceiroConfig
    {
        public class RelatorioResumoDiario
        {
            /// <summary>
            /// Define que os pagamentos feitos com crédito entrarão na soma
            /// </summary>
            public static bool PagamentoChequeDevolvidoOutrosConsiderarCredito
            {
                get { return Config.GetConfigItem<bool>(Config.ConfigEnum.PagamentoChequeDevolvidoOutrosConsiderarCredito); }
            }
        }
    }
}
