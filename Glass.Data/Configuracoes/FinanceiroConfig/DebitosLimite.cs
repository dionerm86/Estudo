using Glass.Data.Helper;

namespace Glass.Configuracoes
{
    public partial class FinanceiroConfig
    {
        public static class DebitosLimite
        {
            /// <summary>
            /// A empresa considera cheques abertos nos débitos do cliente?
            /// </summary>
            public static bool EmpresaConsideraChequeLimite
            {
                get { return Config.GetConfigItem<bool>(Config.ConfigEnum.EmpresaConsideraChequeLimite); }
            }

            /// <summary>
            /// Define se cheques depositados e não vencidos serão considerados no limite do cliente
            /// </summary>
            public static bool ConsiderarChequeDepositadoVencidoNoLimite
            {
                get { return Config.GetConfigItem<bool>(Config.ConfigEnum.ConsiderarChequeDepositadoVencidoNoLimite); }
            }

            /// <summary>
            /// A empresa considera pedidos conferidos nos débitos do cliente?
            /// </summary>
            public static bool EmpresaConsideraPedidoConferidoLimite
            {
                get { return Config.GetConfigItem<bool>(Config.ConfigEnum.EmpresaConsideraPedidoConferidoLimite); }
            }

            /// <summary>
            /// A empresa considera pedidos ativos nos débitos do cliente?
            /// </summary>
            public static bool EmpresaConsideraPedidoAtivoLimite
            {
                get { return Config.GetConfigItem<bool>(Config.ConfigEnum.EmpresaConsideraPedidoAtivoLimite); }
            }
        }
    }
}
