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

        /// <summary>
        /// Define se será permitido inserir um beneficiamento do tipo seleção simples e calculado por quantidade
        /// </summary>
        public static bool PermitirControleSelecaoSimplesComCalculoQtd
        {
            get { return Config.GetConfigItem<bool>(Config.ConfigEnum.PermitirControleSelecaoSimplesComCalculoQtd); }
        }
    }
}
