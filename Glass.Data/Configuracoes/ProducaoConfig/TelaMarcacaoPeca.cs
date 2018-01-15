using Glass.Data.Helper;

namespace Glass.Configuracoes
{
    partial class ProducaoConfig
    {
        /// <summary>
        /// Classe com as configurações da tela de marcação da peça na produção.
        /// </summary>
        public static class TelaMarcacaoPeca
        {
            /// <summary>
            /// Exibir os anexos do pedido ao consultar peça, caso o pedido seja
            /// mão-de-obra?
            /// </summary>
            public static bool ExibirAnexosPedidosMaoDeObraAoConsultarPeca
            {
                get { return Config.GetConfigItem<bool>(Config.ConfigEnum.ExibirAnexosPedidosMaoDeObraAoConsultarPeca); }
            }

            /// <summary>
            /// Sempre exibir os anexos do pedido ao consultar peça?
            /// </summary>
            public static bool SempreExibirAnexosPedidosAoConsultarPeca
            {
                get { return Config.GetConfigItem<bool>(Config.ConfigEnum.SempreExibirAnexosPedidosAoConsultarPeca); }
            }

            /// <summary>
            /// Define que a chapa só poderá ser lida apenas uma vez pelo plano de corte, ou várias vezes desde que todas
            /// as peças sejam do mesmo plano de corte
            /// </summary>
            public static bool ImpedirLeituraChapaComPlanoCorteVinculado
            {
                get { return Config.GetConfigItem<bool>(Config.ConfigEnum.ImpedirLeituraChapaComPlanoCorteVinculado); }
            }

            /// <summary>
            /// Define se o usuário poderá marcar todas as peças do pedido acrescentando o caractere P antes do número do pedido.
            /// </summary>
            public static bool ImpedirLeituraTodasPecasPedido
            {
                get { return Config.GetConfigItem<bool>(Config.ConfigEnum.ImpedirLeituraTodasPecasPedido); }
            }

            /// <summary>
            /// Define se o usuário poderá marcar todas as peças do pedido, no setor entregue, acrescentando o caractere P antes do número do pedido.
            /// </summary>
            public static bool ImpedirLeituraSetorEntregueTodasPecasPedido
            {
                get { return Config.GetConfigItem<bool>(Config.ConfigEnum.ImpedirLeituraSetorEntregueTodasPecasPedido); }
            }

            /// <summary>
            /// Define se na tela de marcação será exibido o painel de setores ao invés do painel de produção
            /// </summary>
            public static bool ExibirPainelSetores
            {
                get { return Config.GetConfigItem<bool>(Config.ConfigEnum.ExibirPainelSetores); }
            }

            /// <summary>
            /// Define se ao marcar plano de corte do optyway em chapa, caso ocorra uma falha na leitura de uma das peças, desfaça a leitura das demais
            /// </summary>
            public static bool CancelarLeiturasSeUmaFalhar
            {
                get { return Config.GetConfigItem<bool>(Config.ConfigEnum.CancelarLeiturasSeUmaFalhar); }
            }

            /// <summary>
            /// Define se a obs do projeto vai ser mostrada ao consultar ou marcar a peça
            /// </summary>
            public static bool ExibirObsProjeto
            {
                get { return Config.GetConfigItem<bool>(Config.ConfigEnum.ExibirObsProjeto); }
            }
        }
    }
}