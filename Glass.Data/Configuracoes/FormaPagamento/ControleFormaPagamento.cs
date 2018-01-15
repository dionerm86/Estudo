using Glass.Data.Helper;

namespace Glass.Configuracoes
{
    public partial class FormaPagamento
    {
        public class ControleFormaPagamento
        {
            /// <summary>
            /// Define se o controle exibirá os tipos de boleto.
            /// </summary>
            public static bool ExibirTiposBoleto
            {
                get { return Config.GetConfigItem<bool>(Config.ConfigEnum.ExibirTiposBoleto); }
            }
        }
    }
}
