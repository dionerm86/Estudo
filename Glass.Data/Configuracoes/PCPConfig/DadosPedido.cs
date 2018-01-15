using Glass.Data.Helper;

namespace Glass.Configuracoes
{
    public partial class PCPConfig
    {
        public static class DadosPedido
        {
            /// <summary>
            /// Exibir os dados da comissÃ£o no pedido PCP?
            /// </summary>
            public static bool ComissaoPedidoPcp
            {
                get { return PedidoConfig.Comissao.ComissaoPedido &&  Config.GetConfigItem<bool>(Config.ConfigEnum.ComissaoPedidoPcp); }
            }

            /// <summary>
            /// Exibir os dados de desconto/acrÃ©scimo no pedido PCP?
            /// </summary>
            public static bool DescontoAcrescimoPedidoPcp
            {
                get { return Config.GetConfigItem<bool>(Config.ConfigEnum.DescontoAcrescimoPedidoPcp); }
            }
        }
    }
}
