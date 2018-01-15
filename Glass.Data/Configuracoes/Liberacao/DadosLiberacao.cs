using Glass.Data.Helper;

namespace Glass.Configuracoes
{
    public partial class Liberacao
    {
        public static class DadosLiberacao
        {
            /// <summary>
            /// Verifica se a empresa libera os produtos do pedido.
            /// </summary>
            public static bool LiberarPedidoProdutos
            {
                get
                {
                    if (OrdemCargaConfig.UsarControleOrdemCarga)
                        return OrdemCargaConfig.UsarOrdemCargaParcial;

                    return Config.GetConfigItem<bool>(Config.ConfigEnum.LiberacaoParcial);
                }
            }

            /// <summary>
            /// Verifica se a empresa libera apenas os produtos prontos.
            /// </summary>
            public static bool LiberarProdutosProntos
            {
                get
                {
                    return PCPConfig.ControlarProducao && PCPConfig.UsarConferenciaFluxo && Config.GetConfigItem<bool>(Config.ConfigEnum.LiberarProdutosProntos);
                }
            }

            /// <summary>
            /// Verifica se a empresa libera os pedidos de um cliente de rota mesmo sem o pedido estar pronto
            /// </summary>
            public static bool LiberarClienteRota
            {
                get
                {
                    return Config.GetConfigItem<bool>(Config.ConfigEnum.LiberarClienteRotaSemEstarPronto);
                }
            }

            /// <summary>
            /// Indica se a liberação de pedido é bloqueada pelo tipo de venda do pedido.
            /// </summary>
            public static bool BloquearLiberacaoDadosPedido
            {
                get { return Config.GetConfigItem<bool>(Config.ConfigEnum.BloquearLiberacaoDadosPedido); }
            }

            /// <summary>
            /// A empresa usa desconto na liberação de pedido?
            /// (Esta opção não deve existir, pois atrapalha todos os relatórios de vendas do sistema)
            /// </summary>
            public static bool DescontoLiberarPedido
            {
                get { return Config.GetConfigItem<bool>(Config.ConfigEnum.DescontoLiberarPedido); }
            }

            /// <summary>
            /// Apenas o menor prazo será considerado ao liberar vários pedidos?
            /// </summary>
            public static bool UsarMenorPrazoLiberarPedido
            {
                get { return Config.GetConfigItem<bool>(Config.ConfigEnum.UsarMenorPrazoLiberarPedido); }
            }

            /// <summary>
            /// Indica se a empresa usa o relatório de liberação de 4 vias.
            /// </summary>
            public static bool UsarRelatorioLiberacao4Vias
            {
                get { return Config.GetConfigItem<bool>(Config.ConfigEnum.UsarRelatorioLiberacao4Vias); }
            }

            /// <summary>
            /// Indica se a o relatório exibe via da empresa para pedido de reposição
            /// </summary>
            public static bool ExibirViaEmpresaPedidoReposicao
            {
                get { return Config.GetConfigItem<bool>(Config.ConfigEnum.ExibirViaEmpresaPedidoReposicao); }
            }

            /// <summary>
            /// O resumo de liberação deve ser agrupado por produto?
            /// </summary>
            public static bool AgruparResumoLiberacaoProduto
            {
                get { return Config.GetConfigItem<bool>(Config.ConfigEnum.AgruparResumoLiberacaoProduto); }
            }

            /// <summary>
            /// Os pedidos atrasados podem ser liberados parcialmente?
            /// </summary>
            public static bool LiberarPedidoAtrasadoParcialmente
            {
                get
                {
                    if (OrdemCargaConfig.UsarControleOrdemCarga && !OrdemCargaConfig.UsarOrdemCargaParcial)
                        return false;

                    return Config.GetConfigItem<bool>(Config.ConfigEnum.LiberarPedidoAtrasadoParcialmente);
                }
            }

            /// <summary>
            /// Indica se a opção Usar Crédito deve vir marcada por padrão na tela de liberação de pedidos.
            /// </summary>
            public static bool UsarCreditoMarcadoTelaLiberacaoPedido
            {
                get { return Config.GetConfigItem<bool>(Config.ConfigEnum.UsarCreditoMarcadoTelaLiberacaoPedido); }
            }
        }
    }
}
