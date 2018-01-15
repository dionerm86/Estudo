using Glass.Data.Helper;

namespace Glass.Configuracoes
{
    public partial class FinanceiroConfig
    {
        public static class CaixaDiario
        {
            /// <summary>
            /// Define se nesta empresa pode informar o valor a ser transferido do Cx. Diário para o Cx. Geral,
            /// se for false, será transferido todo o valor do caixa diário
            /// </summary>
            public static bool InformarValorTransf
            {
                get { return Config.GetConfigItem<bool>(Config.ConfigEnum.InformarValorTransf); }
            }

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
