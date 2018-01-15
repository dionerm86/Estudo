using Glass.Data.Helper;

namespace Glass.Configuracoes
{
    public partial class OrcamentoConfig
    {
        public static class ItensProdutos
        {
            /// <summary>
            /// Identifica se a empresa trabalha com itens dos produtos do orçamento.
            /// </summary>
            public static bool ItensProdutosOrcamento
            {
                get { return Config.GetConfigItem<bool>(Config.ConfigEnum.ItensProdutosOrcamento); }
            }

            /// <summary>
            /// Define se a empresa sempre insere itens dos produtos no orçamento.
            /// </summary>
            public static bool SempreInserirItensProdutosOrcamento
            {
                get
                {
                    if (!ItensProdutosOrcamento)
                        return false;

                    return Config.GetConfigItem<bool>(Config.ConfigEnum.SempreInserirItensProdutosOrcamento);
                }
            }            
        }
    }
}
