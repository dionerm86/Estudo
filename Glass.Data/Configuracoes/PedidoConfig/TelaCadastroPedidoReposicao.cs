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
            /// será selecionada loja quando for gerar pedido de reposição?
            /// </summary>
            public static bool SelecionarLojaGerarPedidoReposicao
            {
                get { return Config.GetConfigItem<bool>(Config.ConfigEnum.SelecionarLojaGerarPedidoReposicao); }
            }
        }
    }
}
