using Glass.Data.Helper;

namespace Glass.Configuracoes
{
    public partial class ProdutoConfig
    {
        /// <summary>
        /// Classe com as configurações da tela de produtos vendidos (relatório).
        /// </summary>
        public static class TelaProdutosVendidosRelatorio
        {
            /// <summary>
            /// O filtro de data padrão deve ser o filtro pela data
            /// do pedido?
            /// </summary>
            public static bool UsarFiltroDataPedidoComoPadrao
            {
                get { return Config.GetConfigItem<bool>(Config.ConfigEnum.UsarFiltroDataPedidoComoPadrao); }
            }
        }
    }
}
