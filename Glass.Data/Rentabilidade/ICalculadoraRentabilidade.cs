using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Glass.Data
{
    /// <summary>
    /// Assinatura da classe responsável pelo calculo da rentabilidade.
    /// </summary>
    public interface ICalculadoraRentabilidade<T>
    {
        #region Methods

        /// <summary>
        /// Executa o calculo da rentabilidade para o tipo principal da calculadora..
        /// </summary>
        /// <param name="id">Identificador da instancia principal.</param>
        ICalculoRentabilidadeResultado Calcular(GDA.GDASession sessao, uint id);

        #endregion
    }

    /// <summary>
    /// Classe com métodos para auxiliar nos calculos da rentabilidade.
    /// </summary>
    static class RentabilidadeHelper
    {
        #region Properties

        /// <summary>
        /// Instancia da calculadora de rentabilidade.
        /// </summary>
        public static ICalculadoraRentabilidade<T> ObterCalculadora<T>() => Microsoft.Practices.ServiceLocation
            .ServiceLocator.Current.GetInstance<ICalculadoraRentabilidade<T>>();

        #endregion
    }
}
