using Glass.Data.Helper;

namespace Glass.Configuracoes
{
    public partial class PedidoConfig
    {
        public static class Impostos
        {
            /// <summary>
            /// Verifica se a empresa calcula o valor do ICMS no pedido.
            /// </summary>
            public static bool CalcularIcmsPedido
            { 
                get { return Config.GetConfigItem<bool>(Config.ConfigEnum.CalcularIcmsPedido); }
            }

            /// <summary>
            /// Verifica se a empresa calcula o valor do IPI no pedido.
            /// </summary>
            public static bool CalcularIpiPedido
            {
                get { return Config.GetConfigItem<bool>(Config.ConfigEnum.CalcularIpiPedido); }
            }
        }
    }
}
