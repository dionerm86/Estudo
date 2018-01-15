using Glass.Data.Helper;

namespace Glass.Configuracoes
{
    public partial class PedidoConfig
    {
        /// <summary>
        /// Classe com as configurações da tela de lista de pedidos (relatório).
        /// </summary>
        public static class TelaListagemRelatorio
        {
            /// <summary>
            /// O relatório pode ter a opção de exibir os produtos?
            /// </summary>
            public static bool ExibirCampoProdutosRelatorio
            {
                get { return Config.GetConfigItem<bool>(Config.ConfigEnum.ExibirCampoProdutosRelatorio); }
            }

            /// <summary>
            /// Mostrar o valor do IPI do pedido na listagem de pedidos
            /// </summary>
            public static bool ExibirValorIPI
            {
                get { return Config.GetConfigItem<bool>(Config.ConfigEnum.ExibirValorIPI); }
            }
        }
    }
}
