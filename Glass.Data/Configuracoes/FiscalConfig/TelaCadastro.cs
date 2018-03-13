using Glass.Data.Helper;

namespace Glass.Configuracoes
{
    partial class FiscalConfig
    {
        public class TelaCadastro
        {
            /// <summary>
            /// Forma de pagamento padrão para NFCe.
            /// </summary>
            public static Data.Model.FormaPagtoNotaFiscalEnum? FormaPagtoPadraoNFCe
            {
                get
                {
                    var formaPagto = Config.GetConfigItem<int>(Config.ConfigEnum.FormaPagtoPadraoNFCe);

                    if (formaPagto == 0)
                        return null;

                    return (Data.Model.FormaPagtoNotaFiscalEnum)formaPagto;
                }
            }
        }
    }
}
