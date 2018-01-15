using Glass.Data.Helper;

namespace Glass.Configuracoes
{
    public partial class OrcamentoConfig
    {
        public static class ItensProdutos
        {
            /// <summary>
            /// Define se a empresa sempre insere itens dos produtos no orçamento.
            /// </summary>
            public static bool SempreInserirItensProdutosOrcamento
            {
                get
                {
                    return Config.GetConfigItem<bool>(Config.ConfigEnum.SempreInserirItensProdutosOrcamento);
                }
            }            
        }
    }
}
