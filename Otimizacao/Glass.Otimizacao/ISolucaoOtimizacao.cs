using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Glass.Otimizacao
{
    /// <summary>
    /// Assinatura da solução de otimização.
    /// </summary>
    public interface ISolucaoOtimizacao
    {
        #region Propriedades

        /// <summary>
        /// Obtém o identificador único da solução.
        /// </summary>
        Guid Uid { get; }

        #endregion
    }
}
