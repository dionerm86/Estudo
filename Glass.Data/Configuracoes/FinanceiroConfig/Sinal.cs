using Glass.Data.Helper;

namespace Glass.Configuracoes
{
    public partial class FinanceiroConfig
    {
        public static class Sinal
        {
            /// <summary>
            /// Bloqueia o recebimento de pagamento antecipado caso o pedido esteja ativo
            /// </summary>
            public static bool BloquearRecebimentoPagtoAntecipadoPedidoAtivo
            {
                get { return Config.GetConfigItem<bool>(Config.ConfigEnum.BloquearRecebimentoPagtoAntecipadoPedidoAtivo); }
            }

            public static bool LimitarCredito
            {
                get { return Config.GetConfigItem<bool>(Config.ConfigEnum.LimitarCredito); }
            }
        }
    }
}