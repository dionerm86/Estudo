using Glass.Data.Helper;

namespace Glass.Configuracoes
{
    public static partial class ChequesConfig
    {
        public static class TelaCadastro
        {
            /// <summary>
            /// Número de dígitos exigidos para os cheques?
            /// </summary>
            public static int NumeroDigitosCheque
            {
                get { return Config.GetConfigItem<int>(Config.ConfigEnum.NumeroDigitosCheque); }
            }
        }
    }
}
