using Glass.Data.Helper;

namespace Glass.Configuracoes
{
    partial class FinanceiroConfig
    {
        public class TelaContasRecebidas
        {
            /// <summary>
            /// A coluna com o percentual da comissão deve ser exibida na lista.
            /// </summary>
            public static bool ExibirPercComissaoNaLista
            {
                get { return Config.GetConfigItem<bool>(Config.ConfigEnum.ExibirPercComissaoNaLista); }
            }
        }
    }
}
