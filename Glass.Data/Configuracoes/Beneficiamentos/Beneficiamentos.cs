using Glass.Data.Helper;

namespace Glass.Configuracoes
{
    public static partial class Beneficiamentos
    {
        /// <summary>
        /// Define que ao usar beneficiamentos que calculam com base no m², será considerado a área mínima
        /// </summary>
        public static bool UsarM2CalcBeneficiamentos
        {
            get { return Config.GetConfigItem<bool>(Config.ConfigEnum.UsarM2CalcBeneficiamentos); }
        }
    }
}
