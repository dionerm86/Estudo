using Glass.Data.Helper;
using Glass.Data.Model;

namespace Glass.Configuracoes
{
    public partial class OrcamentoConfig
    {
        /// <summary>
        /// Classe com as configurações da tela de cadastro de orçamento.
        /// </summary>
        public static class TelaCadastro
        {
            /// <summary>
            /// Tipo Orçamento padrão ao cadastrar um orçamento.
            /// </summary>
            public static Orcamento.TipoOrcamentoEnum? TipoOrcamentoPadrao
            {
                get
                {
                    var config = Config.GetConfigItem<string>(Config.ConfigEnum.TipoOrcamentoPadrao);

                    if (string.IsNullOrEmpty(config))
                        return null;

                    return (Orcamento.TipoOrcamentoEnum)System.Enum.Parse(typeof(Orcamento.TipoOrcamentoEnum), config);
                }
            }

            public static bool PermitirInserirClienteInativoBloqueado
            {
                get { return Config.GetConfigItem<bool>(Config.ConfigEnum.PermitirInserirClienteInativoBloqueado); }
            }

            public static bool PermitirInserirSemTipoOrcamento
            {
                get { return Config.GetConfigItem<bool>(Config.ConfigEnum.PermitirInserirSemTipoOrcamento); }
            }
        }
    }
}