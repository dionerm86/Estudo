using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Glass.Data
{
    /// <summary>
    /// Classe com métodos para auxiliar nos calculos da rentabilidade.
    /// </summary>
    static class RentabilidadeHelper
    {
        #region Propriedades

        /// <summary>
        /// Obtém a instância da calculadora de rentabilidade.
        /// </summary>
        public static ICalculadoraRentabilidade<T> ObterCalculadora<T>() => 
            Microsoft.Practices.ServiceLocation.ServiceLocator
                .Current.GetInstance<ICalculadoraRentabilidade<T>>();

        /// <summary>
        /// Obtém a instancia do verificador de rentabilidade para liberação.
        /// </summary>
        /// <returns></returns>
        public static IVerificadorRentabilidadeLiberacao<T> ObterVerificadorLiberacao<T>() =>
            Microsoft.Practices.ServiceLocation.ServiceLocator
                .Current.GetInstance<IVerificadorRentabilidadeLiberacao<T>>();

        #endregion
    }
}
