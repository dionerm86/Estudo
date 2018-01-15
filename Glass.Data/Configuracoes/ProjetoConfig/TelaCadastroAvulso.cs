using Glass.Data.Helper;

namespace Glass.Configuracoes
{
    public partial class ProjetoConfig
    {
        /// <summary>
        /// Classe com as configurações da tela de cadastro de projeto avulso.
        /// </summary>
        public static class TelaCadastroAvulso
        {
            /// <summary>
            /// O texto do label de ambiente deve ser alterado?
            /// </summary>
            public static bool AlterarTextoLabelAmbiente
            {
                get { return Config.GetConfigItem<bool>(Config.ConfigEnum.AlterarTextoLabelAmbiente); }
            }

            /// <summary>
            /// O botão "Imprimir" deve ser exibido na tela de projeto?
            /// </summary>
            public static bool ExibirBotaoImprimirProjeto
            {
                get { return Config.GetConfigItem<bool>(Config.ConfigEnum.ExibirBotaoImprimirProjeto); }
            }

            /// <summary>
            /// Aumentar o número de materiais exibidos na lista?
            /// (virtualmente remove a paginação)
            /// </summary>
            public static bool AumentarNumeroMateriaisListagem
            {
                get { return Config.GetConfigItem<bool>(Config.ConfigEnum.AumentarNumeroMateriaisListagem); }
            }

            /// <summary>
            /// Define que o total de m² do vão do projeto será o somatório das peças de vidro do mesmo
            /// </summary>
            public static bool AreaTotalItemProjetoAreaPecas
            {
                get { return Config.GetConfigItem<bool>(Config.ConfigEnum.AreaTotalItemProjetoAreaPecas); }
            }
        }
    }
}
