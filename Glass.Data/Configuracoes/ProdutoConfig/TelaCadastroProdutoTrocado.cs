using Glass.Data.Helper;

namespace Glass.Configuracoes
{
    public partial class ProdutoConfig
    {
        /// <summary>
        /// Classe com as configurações da tela de cadastro de troca.
        /// </summary>
        public static class TelaCadastroProdutoTrocado
        {
            /// <summary>
            /// O valor do produto trocado deve estar acima do valor mínimo?
            /// </summary>
            public static bool BloquearValorProdutoAbaixoDoMinimo
            {
                get { return Config.GetConfigItem<bool>(Config.ConfigEnum.BloquearValorProdutoAbaixoDoMinimo); }
            }
        }
    }
}
