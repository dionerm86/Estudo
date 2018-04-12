using Glass.Data.Helper;

namespace Glass.Configuracoes
{
    public partial class PedidoConfig
    {
        public static class ExportacaoPedido
        {
            /// <summary>
            /// A URL do sistema WebGlass pode ser alterada no fornecedor?
            /// </summary>
            public static bool AlterarUrlWebGlassFornec
            {
                get { return Config.GetConfigItem<bool>(Config.ConfigEnum.AlterarUrlWebGlassFornec); }
            }

            /// <summary>
            /// Define que será mantido o tipo de entrega do pedido
            /// </summary>
            public static bool ManterTipoEntregaPedido
            {
                get { return Config.GetConfigItem<bool>(Config.ConfigEnum.ManterTipoEntregaPedido); }
            }
        }
    }
}
