using System;
using Glass.Data.Helper;

namespace Glass.Configuracoes
{
    public partial class PedidoConfig
    {
        /// <summary>
        /// Classe com as configurações da tela de confirmação de pedido para liberação.
        /// </summary>
        public static class TelaConfirmaPedidoLiberacao
        {
            /// <summary>
            /// O campo "Gerar Pedido" deve vir marcado ao abrir a tela?
            /// </summary>
            public static bool GerarPedidoMarcado
            {
                get { return Config.GetConfigItem<bool>(Config.ConfigEnum.GerarPedidoMarcado); }
            }

            /// <summary>
            /// Define se o pedido será finalizado ao gerar o pedido PCP.
            /// </summary>
            public static bool FinalizarPedidoAoGerarEspelho
            {
                get { return Config.GetConfigItem<bool>(Config.ConfigEnum.FinalizarPedidoAoGerarEspelho); }
            }

            /// <summary>
            /// Define quais subgrupos devem ser destacados na tela de confirmação de pedido.
            /// </summary>
            public static string SubgruposDestacar
            {
                get { return Config.GetConfigItem<string>(Config.ConfigEnum.SubgruposDestacar); }
            }
        }
    }
}
