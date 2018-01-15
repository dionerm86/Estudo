using Glass.Data.Helper;

namespace Glass.Configuracoes
{
    public partial class PedidoConfig
    {
        public static class Pedido_MaximoVendas
        {
            /// <summary>
            /// Informa o máximo de m² que pode ser vendido num período determinado
            /// </summary>
            public static int MaximoVendasM2
            {
                get { return MaximoVendas ? Config.GetConfigItem<int>(Config.ConfigEnum.MaximoVendasM2) : 0; }
            }

            /// <summary>
            /// A empresa usa um número máximo de m²?
            /// </summary>
            public static bool MaximoVendas
            {
                get { return Config.GetConfigItem<bool>(Config.ConfigEnum.MaximoVendas); }
            }

            /// <summary>
            /// Constante que define o número de dias permitidos para agendamento da entrega.
            /// Pode ser 0 para considerar qualquer dia.
            /// </summary>
            public static uint NumMaxDiasMaximoVendas
            {
                get
                {
                    if (!MaximoVendas)
                        return 0;

                    return Config.GetConfigItem<uint>(Config.ConfigEnum.NumMaxDiasMaximoVendas);
                }
            }

            /// <summary>
            /// Informa o período em dias que o MaximoVendasM2 será aplicado
            /// </summary>
            public static int MaximoVendasPeriodo
            {
                get { return MaximoVendas ? Config.GetConfigItem<int>(Config.ConfigEnum.MaximoVendasPeriodo) : 0; }
            }
        }
    }
}
