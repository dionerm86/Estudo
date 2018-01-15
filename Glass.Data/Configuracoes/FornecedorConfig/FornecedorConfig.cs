using Glass.Data.Helper;

namespace Glass.Configuracoes
{
    public partial class FornecedorConfig
    {
        /// <summary>
        /// Indica o tipo de uso da antecipação de fornecedor.
        /// </summary>
        public static DataSources.TipoUsoAntecipacaoFornecedor TipoUsoAntecipacaoFornecedor
        {
            get { return Config.GetConfigItem<DataSources.TipoUsoAntecipacaoFornecedor>(Config.ConfigEnum.TipoUsoAntecipacaoFornecedor); }
        }        
    }
}
