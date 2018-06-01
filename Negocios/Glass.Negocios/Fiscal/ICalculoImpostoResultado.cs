using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Glass.Fiscal.Negocios
{
    /// <summary>
    /// Representa o resultado doa calculo de imposto.
    /// </summary>
    public interface ICalculoImpostoResultado
    {
        #region Propriedades

        /// <summary>
        /// Container dos itens usados no cálculo.
        /// </summary>
        IItemImpostoContainer Container { get; }

        /// <summary>
        /// Itens do resultado.
        /// </summary>
        IEnumerable<IItemCalculoImpostoResultado> Itens { get; }

        #endregion
    }
}
