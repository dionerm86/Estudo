using Glass.Data.Helper;

namespace Glass.Configuracoes
{
    partial class ProjetoConfig
    {
        public static class TelaCadastroParceiros
        {
            /// <summary>
            /// Os campos de tipo de entrega devem ser escondidos?
            /// </summary>
            public static bool EsconderCamposTipoEntrega
            {
                get { return Config.GetConfigItem<bool>(Config.ConfigEnum.EsconderCamposTipoEntrega); }
            }

            /// <summary>
            /// Bloqueia o tipo de entrega do pedido de acordo com a rota do cliente
            /// (clientes de rota são de tipo "Entrega" e clientes sem rota "Balcão").
            /// </summary>
            public static bool BloquearTipoEntregaClientesRota
            {
                get { return Config.GetConfigItem<bool>(Config.ConfigEnum.BloquearTipoEntregaClientesRota); }
            }

            /// <summary>
            /// Bloqueia o tipo de entrega do pedido como Entrega
            /// Para melhor funcionamento deve ser utilizada com a config BloquearTipoEntregaClientesRota desabilitada
            /// e deve ser desabilitada para utilizar BloquearTipoEntregaClientesRota
            /// </summary>
            public static bool BloquearTipoEntregaEntrega
            {
                get { return Config.GetConfigItem<bool>(Config.ConfigEnum.BloquearTipoEntregaEntrega); }
            }
            
            /// <summary>
            /// Define se os campos de "Alterar cores dos materiais de todos os cálculos" devem ser escondidos
            /// </summary>
            public static bool EsconderCamposAlteraCorItemProjeto
            {
                get { return Config.GetConfigItem<bool>(Config.ConfigEnum.EsconderCamposAlteraCorItemProjeto); }
            }

            /// <summary>
            /// Define se será exibido as opções de cores de Aluminio e Ferragem ao inserir projeto pelo WebGlassParceiros
            /// </summary>
            public static bool ExibirCorAluminioFerragemWebGlassParceiros
            {
                get { return Config.GetConfigItem<bool>(Config.ConfigEnum.ExibirCorAluminioFerragemWebGlassParceiros); }
            }

            /// <summary>
            /// Retorna o ID da loja que será associada ao pedido do parceiro, por tipo do pedido.
            /// </summary>
            public static int? IdLojaPorTipoPedidoComVidro
            {
                get { return Config.GetConfigItem<int?>(Config.ConfigEnum.IdLojaPorTipoPedidoComVidro); }
            }

            /// <summary>
            /// Retorna o ID da loja que será associada ao pedido do parceiro, por tipo do pedido.
            /// </summary>
            public static int? IdLojaPorTipoPedidoSemVidro
            {
                get { return Config.GetConfigItem<int?>(Config.ConfigEnum.IdLojaPorTipoPedidoSemVidro); }
            }

            /// <summary>
            /// Confirmar pedido, gerar espelho e finalizar espelho ao gerar pedido pelo E-Commerce.
            /// </summary>
            public static bool ConfirmarPedidoGerarPCPFinalizarPCPAoGerarPedido
            {
                get { return Config.GetConfigItem<bool>(Config.ConfigEnum.ConfirmarPedidoGerarPCPFinalizarPCPAoGerarPedido); }
            }
        }
    }
}
