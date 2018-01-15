using Glass.Data.Helper;

namespace Glass.Configuracoes
{
    public partial class ReciboConfig
    {
        /// <summary>
        /// Classe com as configurações do relatório de recibo.
        /// </summary>
        public static class Relatorio
        {
            /// <summary>
            /// Usar parcelas de pedido/liberacao no relatório?
            /// </summary>
            public static bool UsarParcelasPedido
            {
                get { return Config.GetConfigItem<bool>(Config.ConfigEnum.UsarParcelasPedido); }
            }
        }
    }
}
