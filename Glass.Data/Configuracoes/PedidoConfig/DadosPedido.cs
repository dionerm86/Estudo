using Glass.Data.Helper;

namespace Glass.Configuracoes
{
    public partial class PedidoConfig
    {
        public static class DadosPedido
        {
            /// <summary>
            /// Verifica se a empresa trabalha com ambiente de pedido
            /// </summary>
            public static bool AmbientePedido
            {
                get { return Config.GetConfigItem<bool>(Config.ConfigEnum.AmbientePedido); }
            }

            /// <summary>
            /// A empresa usa o controle novo de obras (com produtos)?
            /// </summary>
            public static bool UsarControleNovoObra
            {
                get { return Config.GetConfigItem<bool>(Config.ConfigEnum.UsarControleNovoObra); }
            }

            /// <summary>
            /// Define se há bloqueio de produtos para os pedidos comuns e mão de obra.
            /// </summary>
            public static bool BloqueioPedidoMaoDeObra
            {
                get { return Config.GetConfigItem<bool>(Config.ConfigEnum.BloqueioPedidoMaoDeObra); }
            }

            public static bool CalcularAreaMinimaApenasVidroBeneficiado
            {
                get { return Config.GetConfigItem<bool>(Config.ConfigEnum.CalcularAreaMinimaApenasVidroBeneficiado); }
            }

            /// <summary>
            /// Define se os dados do cliente serão bloqueados no pedido.
            /// </summary>
            public static bool BloquearDadosClientePedido
            {
                get { return Config.GetConfigItem<bool>(Config.ConfigEnum.BloquearDadosClientePedido); }
            }

            /// <summary>
            /// Define se o usuário poderá alterar o valor unitário do produto para mais.
            /// </summary>
            public static bool AlterarValorUnitarioProduto
            {
                get { return Config.GetConfigItem<bool>(Config.ConfigEnum.AlterarValorUnitarioProduto); }
            }

            /// <summary>
            /// Define se a empresa exige que os vidros que podem ser beneficiados tenham processo e aplicação.
            /// </summary>
            public static bool ObrigarProcAplVidros
            {
                get
                {
                    if (!Geral.ControlePCP || Geral.SistemaLite)
                        return false;

                    return Config.GetConfigItem<bool>(Config.ConfigEnum.ObrigarProcAplVidros);
                }
            }

            /// <summary>
            /// Define se o pedido de venda/revenda bloqueia os itens pelo seu tipo.
            /// </summary>
            public static bool BloquearItensTipoPedido
            {
                get { return Config.GetConfigItem<bool>(Config.ConfigEnum.BloquearItensTipoPedido); }
            }

            /// <summary>
            /// Define se o pedido de venda bloqueia os itens de cor e espessura diferentes.
            /// </summary>
            public static bool BloquearItensCorEspessura
            {
                get { return PedidoConfig.DadosPedido.BloquearItensTipoPedido && 
                    Config.GetConfigItem<bool>(Config.ConfigEnum.BloquearItensCorEspessura); }
            }

            /// <summary>
            /// Indica se a empresa exibe o popup com o estoque dos vidros (produção).
            /// </summary>
            public static bool ExibePopupVidrosEstoque
            {
                get { return Config.GetConfigItem<bool>(Config.ConfigEnum.ExibePopupVidrosEstoque); }
            }

            /// <summary>
            /// Define se ao cadastrar pedido, será buscado o vendedor associado ao cliente selecionado
            /// </summary>
            public static bool BuscarVendedorEmitirPedido
            {
                get { return Config.GetConfigItem<bool>(Config.ConfigEnum.BuscarVendedorEmitirPedido); }
            }

            /// <summary>
            /// Define se irá mostrar botões para confirmar pedido gerando conferência na tela de pedidos
            /// </summary>
            /// <returns></returns>
            public static bool ExibirBotoesConfirmacaoPedido
            {
                get { return PedidoConfig.LiberarPedido && Config.GetConfigItem<bool>(Config.ConfigEnum.ExibirBotoesConfirmacaoPedido); }
            }

            /// <summary>
            /// Define se é obrigatório informar o pedido do cliente no cadastro do pedido
            /// </summary>
            public static bool ObrigarInformarPedidoCliente
            {
                get { return Config.GetConfigItem<bool>(Config.ConfigEnum.ObrigarInformarPedidoCliente); }
            }

            /// <summary>
            /// Exibe apenas os pedidos do vendedor nas listas de pedidos.
            /// </summary>
            public static bool ListaApenasPedidosVendedor
            {
                get { return Config.GetConfigItem<bool>(Config.ConfigEnum.ListaApenasPedidosVendedor); }
            }

            /// <summary>
            /// Define se é possível alterar a data de entrega do pedido para datas inferiores à data atual
            /// </summary>
            public static bool AlterarDataEntregaPedidoDataRetroativa
            {
                get { return Config.GetConfigItem<bool>(Config.ConfigEnum.AlterarDataEntregaPedidoDataRetroativa); }
            }
        }
    }
}
