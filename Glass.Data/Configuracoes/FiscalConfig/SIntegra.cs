using Glass.Data.Helper;

namespace Glass.Configuracoes
{
    public partial class FiscalConfig
    {
        public static class SIntegra
        {
            public static bool SIntegraGerarRegistro50
            {
                get { return Config.GetConfigItem<bool>(Config.ConfigEnum.SIntegraRegistro50); }
            }

            public static bool SIntegraGerarRegistro51
            {
                get { return Config.GetConfigItem<bool>(Config.ConfigEnum.SIntegraRegistro51); }
            }

            public static bool SIntegraGerarRegistro53
            {
                get { return Config.GetConfigItem<bool>(Config.ConfigEnum.SIntegraRegistro53); }
            }

            public static bool SIntegraGerarRegistro54
            {
                get { return Config.GetConfigItem<bool>(Config.ConfigEnum.SIntegraRegistro54); }
            }
            
            public static bool SIntegraGerarRegistro61
            {
                get { return Config.GetConfigItem<bool>(Config.ConfigEnum.SIntegraRegistro61); }
            }

            public static bool SIntegraGerarRegistro70
            {
                get { return Config.GetConfigItem<bool>(Config.ConfigEnum.SIntegraRegistro70); }
            }

            public static bool SIntegraGerarRegistro74
            {
                get { return Config.GetConfigItem<bool>(Config.ConfigEnum.SIntegraRegistro74); }
            }

            public static bool SIntegraGerarRegistro75
            {
                get { return Config.GetConfigItem<bool>(Config.ConfigEnum.SIntegraRegistro75); }
            }
        }
    }
}
