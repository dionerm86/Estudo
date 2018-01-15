using Glass.Data.Helper;

namespace Glass.Configuracoes
{
    partial class OrcamentoConfig
    {
        public class TelaCadastroProdutoOrcamento
        {
            /// <summary>
            /// A descrição do produto pai deve ser atualizada ao inserir ou
            /// remover um produto filho?
            /// </summary>
            public static bool AtualizarDescricaoPaiAoInserirOuRemoverProduto
            {
                get { return Config.GetConfigItem<bool>(Config.ConfigEnum.AtualizarDescricaoPaiAoInserirOuRemoverProduto); }
            }
        }
    }
}
