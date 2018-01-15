using Glass.Data.Helper;

namespace Glass.Configuracoes
{
    partial class CompraConfig
    {
        public class TelaCadastroPcp
        {
            /// <summary>
            /// Usar o custo do beneficiamento no lugar do valor?
            /// </summary>
            public static bool UsarCustoBeneficiamento
            {
                get { return Config.GetConfigItem<bool>(Config.ConfigEnum.UsarCustoBeneficiamento); }
            }
        }
    }
}
