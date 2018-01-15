using Glass.Data.Helper;

namespace Glass.Configuracoes
{
    public partial class PedidoConfig
    {
        public static class DataEntrega
        {
            /// <summary>
            /// Define o número de dias úteis para a data de entrega do pedido (0 para desabilitar).
            /// </summary>
            public static int NumeroDiasUteisDataEntregaPedido
            {
                get { return Config.GetConfigItem<int>(Config.ConfigEnum.NumeroDiasUteisDataEntregaPedido); }
            }

            /// <summary>
            /// Define o número de dias úteis para a data de entrega do pedido de revenda (0 para desabilitar).
            /// </summary>
            public static int NumeroDiasUteisDataEntregaPedidoRevenda
            {
                get { return Config.GetConfigItem<int>(Config.ConfigEnum.NumeroDiasUteisDataEntregaPedidoRevenda); }
            }

            /// <summary>
            /// Define o número de dias úteis para a data de entrega do pedido de mão-de-obra (0 para desabilitar).
            /// </summary>
            public static int NumeroDiasUteisDataEntregaPedidoMaoDeObra
            {
                get { return Config.GetConfigItem<int>(Config.ConfigEnum.NumeroDiasUteisDataEntregaPedidoMaoDeObra); }
            }

            /// <summary>
            /// Define se a empresa bloqueia a edição da data de entrega ao vendedor.
            /// </summary>
            public static bool BloquearDataEntregaPedidoVendedor
            {
                get { return Config.GetConfigItem<bool>(Config.ConfigEnum.BloquearDataEntregaPedidoVendedor); }
            } 
        }
    }
}
