using Glass.Data.Helper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Glass.Configuracoes
{
    /// <summary>
    /// Configurações relacionadas com a otimização.
    /// </summary>
    public static class OtimizacaoConfig
    {
        /// <summary>
        /// Obtém o tipo de estoque de chapa para a otimização.
        /// </summary>
        public static DataSources.TipoEstoqueChapasOtimizacaoEnum TipoEstoqueChapas =>
            Config.GetConfigItem<DataSources.TipoEstoqueChapasOtimizacaoEnum>(Config.ConfigEnum.TipoEstoqueChapasOtimizacao);
    }
}
