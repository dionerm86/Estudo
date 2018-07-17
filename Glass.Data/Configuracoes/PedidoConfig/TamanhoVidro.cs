using Glass.Data.Helper;

namespace Glass.Configuracoes
{
    public partial class PedidoConfig
    {
        public static class TamanhoVidro
        {
            public static bool UsarTamanhoMaximoVidro
            {
                get { return Config.GetConfigItem<bool>(Config.ConfigEnum.UsarTamanhoMaximoVidro); }
            }

            public static int AlturaMaximaVidro
            {
                get { return Config.GetConfigItem<int>(Config.ConfigEnum.AlturaMaximaVidro); }
            }

            public static int LarguraMaximaVidro
            {
                get { return Config.GetConfigItem<int>(Config.ConfigEnum.LarguraMaximaVidro); }
            }

            public static int AlturaELarguraMinimasParaPecasTemperadas
            {
                get { return Config.GetConfigItem<int>(Config.ConfigEnum.AlturaELarguraMinimasParaPecasTemperadas); }
            }

            public static int AlturaELarguraMinimaParaPecasComBisote
            {
                get { return Config.GetConfigItem<int>(Config.ConfigEnum.AlturaELarguraMinimaParaPecasComBisote); }
            }

            public static int AlturaELarguraMinimaParaPecasComLapidacao
            {
                get { return Config.GetConfigItem<int>(Config.ConfigEnum.AlturaELarguraMinimaParaPecasComLapidacao); }
            }
        }
    }
}
