using Glass.Data.Helper;

namespace Glass.Configuracoes
{
    public partial class OrcamentoConfig
    {
        /// <summary>
        /// Classe com as configurações da tela de orçamento rápido.
        /// </summary>
        public static class TelaOrcamentoRapido
        {
            /// <summary>
            /// O estoque deverá ser verificado ao inserir o produto no orçamento?
            /// (verificação feita ao alterar a quantidade de produtos)
            /// </summary>
            public static bool VerificarEstoqueAoInserirProduto
            {
                get { return Config.GetConfigItem<bool>(Config.ConfigEnum.VerificarEstoqueAoInserirProduto); }
            }
        }
    }
}
