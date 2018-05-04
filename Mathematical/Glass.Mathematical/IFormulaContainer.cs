using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Glass.Mathematical
{
    /// <summary>
    /// Assinatura das classes que contém uma formula associada.
    /// </summary>
    interface IFormulaContainer
    {
        #region Propriedades

        /// <summary>
        /// Formula associada.
        /// </summary>
        Formula Formula { get; }

        #endregion
    }
}
