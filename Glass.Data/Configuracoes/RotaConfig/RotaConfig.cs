using Glass.Data.Helper;

namespace Glass.Configuracoes
{
    public static partial class RotaConfig
    {
        /// <summary>
        /// Define se serão usados dias corridos no cálculo da rota
        /// </summary>
        public static bool UsarDiasCorridosCalculoRota
        {
            get { return Config.GetConfigItem<bool>(Config.ConfigEnum.UsarDiasCorridosCalculoRota); }
        }
    }
}
