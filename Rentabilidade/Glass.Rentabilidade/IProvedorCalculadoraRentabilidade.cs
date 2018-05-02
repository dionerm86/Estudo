using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Glass.Rentabilidade
{
    /// <summary>
    /// Assinatura da provedor da calculadora de rentabilidade.
    /// </summary>
    public interface IProvedorCalculadoraRentabilidade
    {
        #region Propriedades

        /// <summary>
        /// Instancia da calculadora associada.
        /// </summary>
        CalculadoraRentabilidade Calculadora { get; }

        #endregion
    }
}
