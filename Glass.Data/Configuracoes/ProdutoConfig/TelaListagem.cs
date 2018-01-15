using Glass.Data.Helper;

namespace Glass.Configuracoes
{
    public partial class ProdutoConfig
    {
        /// <summary>
        /// Classe com as configurações da tela de listagem de produtos.
        /// </summary>
        public static class TelaListagem
        {
            /// <summary>
            /// Usar como relatório de produtos um relatório diferente do original?
            /// </summary>
            public static bool UsarRelatorioProdutosDiferente
            {
                get { return Config.GetConfigItem<bool>(Config.ConfigEnum.UsarRelatorioProdutosDiferente); }
            }

            /// <summary>
            /// Define se a imagem do produto será exibida na grid de seleção de produto
            /// </summary>
            public static bool ExibirImagemAoSelecionarProduto
            {
                get { return Config.GetConfigItem<bool>(Config.ConfigEnum.ExibirImagemAoSelecionarProduto); }
            }

            /// <summary>
            /// Define que serão buscados apenas produtos ativos na tela de consulta de produtos (LstConsultaProd)
            /// </summary>
            public static bool BuscarApenasProdutosAtivosConsultaProd
            {
                get { return Config.GetConfigItem<bool>(Config.ConfigEnum.BuscarApenasProdutosAtivosConsultaProd); }
            }
        }
    }
}
