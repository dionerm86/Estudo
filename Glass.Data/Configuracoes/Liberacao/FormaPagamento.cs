using Glass.Data.Helper;

namespace Glass.Configuracoes
{
    public partial class Liberacao
    {
        public static class FormaPagamento
        {
            /// <summary>
            /// Número de dias para limitar a data do cheque à vista na liberação de pedido.
            /// </summary>
            public static int NumeroDiasChequeVistaLiberarPedido
            {
                get { return Config.GetConfigItem<int>(Config.ConfigEnum.NumeroDiasChequeVistaLiberarPedido); }
            }
        }
    }
}
