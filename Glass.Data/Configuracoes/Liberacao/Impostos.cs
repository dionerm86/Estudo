using Glass.Data.Helper;

namespace Glass.Configuracoes
{
    public partial class Liberacao
    {
        public static class Impostos
        {
            /// <summary>
            /// Verifica se a empresa calcula o valor do ICMS na liberação.
            /// </summary>
            public static bool CalcularIcmsLiberacao
            {
                get { return PedidoConfig.Impostos.CalcularIcmsPedido || Config.GetConfigItem<bool>(Config.ConfigEnum.CalcularIcmsLiberacao); }
            }

            /// <summary>
            /// Verifica se a empresa calcula o valor do IPI na liberação.
            /// </summary>
            public static bool CalcularIpiLiberacao
            {
                get { return PedidoConfig.Impostos.CalcularIpiPedido || 
                    Config.GetConfigItem<bool>(Config.ConfigEnum.CalcularIpiLiberacao); }
            }                        
        }
    }
}
