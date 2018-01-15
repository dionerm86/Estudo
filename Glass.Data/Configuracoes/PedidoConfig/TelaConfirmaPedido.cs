using Glass.Data.Helper;

namespace Glass.Configuracoes
{
    public partial class PedidoConfig
    {
        public static class TelaConfirmaPedido
        {
            /// <summary>
            /// O campo que permite a verificação de parcelas deve ser exibido?
            /// </summary>
            public static bool ExibirVerificarParcelas
            {
                get { return Config.GetConfigItem<bool>(Config.ConfigEnum.ExibirVerificarParcelas); }
            }

            /// <summary>
            /// O relatório do pedido deve ser exibido ao confirmar?
            /// </summary>
            public static bool ExibirRelatorio
            {
                get { return Config.GetConfigItem<bool>(Config.ConfigEnum.ExibirRelatorio); }
            }

            /// <summary>
            /// Reabrir a tela em branco ao confirmar o pedido?
            /// </summary>
            public static bool ExibirTelaEmBrancoAoConfirmar
            {
                get { return Config.GetConfigItem<bool>(Config.ConfigEnum.ExibirTelaEmBrancoAoConfirmar); }
            }
        }
    }
}
