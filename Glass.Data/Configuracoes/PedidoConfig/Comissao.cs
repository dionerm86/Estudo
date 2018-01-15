using Glass.Data.Helper;

namespace Glass.Configuracoes
{
    public partial class PedidoConfig
    {
        public static class Comissao
        {
            /// <summary>
            /// Verifica se a empresa trabalha com comissão de comissionado no pedido e no orçamento
            /// </summary>
            /// <returns></returns>
            public static bool ComissaoPedido
            {
                get { return Config.GetConfigItem<bool>(Config.ConfigEnum.ComissaoPedido); }
            }

            /// <summary>
            /// O vendedor pode alterar o percentual da comissão do comissionado?
            /// </summary>
            public static bool AlterarPercComissionado
            {
                get { return Config.GetConfigItem<bool>(Config.ConfigEnum.AlterarPercComissionado); }
            }

            /// <summary>
            /// Define se será obrigatório buscar o comissionado do cliente para todos os pedidos que o cliente possuir um associado
            /// </summary>
            public static bool UsarComissionadoCliente
            {
                get { return Config.GetConfigItem<bool>(Config.ConfigEnum.UsarComissionadoCliente); }
            }

            /// <summary>
            /// A comissão altera o valor do orçamento/pedido?
            /// </summary>
            public static bool ComissaoAlteraValor
            {
                get { return Config.GetConfigItem<bool>(Config.ConfigEnum.ComissaoAlteraValor); }
            }

            public static bool PerComissaoPedido
            {
                get
                {
                    return Config.GetConfigItem<bool>(Config.ConfigEnum.PerComissaoPedido);
                }
            }

            /// <summary>
            /// Define se o percentual de comissão do pedido será exibido com 2 casas decimais (do contrário exibe 4 casas)
            /// </summary>
            public static bool ExibirPercentualCom2CasasDecimais
            {
                get { return Config.GetConfigItem<bool>(Config.ConfigEnum.ExibirPercentualCom2CasasDecimais); }
            }
        }
    }
}
