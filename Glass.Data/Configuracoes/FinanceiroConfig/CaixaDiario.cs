using Glass.Data.Helper;

namespace Glass.Configuracoes
{
    public partial class FinanceiroConfig
    {
        public static class CaixaDiario
        {
            /// <summary>
            /// Define que serão exibidos os pedidos relacionados ao sinal no campo referência
            /// </summary>
            public static bool ExibirPedidosDoSinal
            {
                get { return Config.GetConfigItem<bool>(Config.ConfigEnum.ExibirPedidosDoSinal); }
            }

            /// <summary>
            /// Define que serão exibidos os pedidos relacionados ao sinal no campo referência
            /// </summary>
            public static bool ExibirPedidosDoAcerto
            {
                get { return Config.GetConfigItem<bool>(Config.ConfigEnum.ExibirPedidosDoAcerto); }
            }

            /// <summary>
            /// Define que serão exibidos os pedidos relacionados ao sinal no campo referência
            /// </summary>
            public static bool ExibirPedidosDaLiberacao
            {
                get { return Config.GetConfigItem<bool>(Config.ConfigEnum.ExibirPedidosDaLiberacao); }
            }
        }
    }
}
