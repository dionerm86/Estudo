using Glass.Data.Helper;

namespace Glass.Configuracoes
{
    partial class FinanceiroConfig
    {
        public class TelaFinalizacaoFinanceiro
        {
            /// <summary>
            /// Define se será exibido a razão social na listagem.
            /// </summary>
            public static bool ExibirRazaoSocial
            {
                get { return Config.GetConfigItem<bool>(Config.ConfigEnum.ExibirRazaoSocial); }
            }
        }
    }
}
