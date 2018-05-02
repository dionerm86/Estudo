using System.Collections.Generic;
using Glass.Data.Helper;
using Glass.Data.Model;

namespace Glass.Configuracoes
{
    /// <summary>
    /// Configuração relaciondas com a rentabilidade.
    /// </summary>
    public static partial class RentabilidadeConfig
    {
        /// <summary>
        /// Define e está pode calcular valores de rentabilidade.
        /// </summary>
        public static bool CalcularRentabilidade
        {
            get { return Config.GetConfigItem<bool>(Config.ConfigEnum.CalcularRentabilidade); }
        }
    }
}
