using Glass.Data.Helper;

namespace Glass.Configuracoes
{
    partial class PedidoConfig
    {
        /// <summary>
        /// Classe com as configurações da tela de cadastro de pedido reposição.
        /// </summary>
        public static class TelaCadastroPedidoReposicao
        {
            /// <summary>
            /// Os campos de data de entrega devem ser escondidos?
            /// </summary>
            public static bool EsconderCamposDataEntrega
            {
                get { return Config.GetConfigItem<bool>(Config.ConfigEnum.EsconderCamposDataEntrega); }
            }

            /// <summary>
            /// A data em que o cliente foi informado da reposição deve
            /// ser informada obrigatoriamente?
            /// </summary>
            public static bool ExigirDataClienteInformadoReposicaoAoFinalizar
            {
                get { return Config.GetConfigItem<bool>(Config.ConfigEnum.ExigirDataClienteInformadoReposicaoAoFinalizar); }
            }

            /// <summary>
            /// será selecionada loja quando for gerar pedido de reposição?
            /// </summary>
            public static bool SelecionarLojaGerarPedidoReposicao
            {
                get { return Config.GetConfigItem<bool>(Config.ConfigEnum.SelecionarLojaGerarPedidoReposicao); }
            }
        }
    }
}
