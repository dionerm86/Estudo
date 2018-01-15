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
            /// O controle que indica que o orçamento foi liberado será exibido?
            /// </summary>
            public static bool ExibirControleLiberarOrcamento
            {
                get { return Config.GetConfigItem<bool>(Config.ConfigEnum.ExibirControleLiberarOrcamento); }
            }

            /// <summary>
            /// Define que a listagem de ambientes será ordenada pelo nome e não pela ordem de cadastro
            /// </summary>
            public static bool OrdenaAmbientesPeloNome
            {
                get { return Config.GetConfigItem<bool>(Config.ConfigEnum.OrdenaAmbientesPeloNome); }
            }

            /// <summary>
            /// Define se será permitido remover o comissionado do orçamento
            /// </summary>
            public static bool PermitirRemoverComissionado
            {
                get { return Config.GetConfigItem<bool>(Config.ConfigEnum.PermitirRemoverComissionado); }
            }

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