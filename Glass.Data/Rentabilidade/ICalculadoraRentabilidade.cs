using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Glass.Data
{
    /// <summary>
    /// Assinatura da classe responsável pelo calculo da rentabilidade.
    /// </summary>
    public interface ICalculadoraRentabilidade<in T>
    {
        #region Métodos

        /// <summary>
        /// Executa o calculo da rentabilidade para o tipo principal da calculadora..
        /// </summary>
        /// <param name="id">Identificador da instancia principal.</param>
        ICalculoRentabilidadeResultado Calcular(GDA.GDASession sessao, uint id);

        /// <summary>
        /// Executa o calculo da rentabilidade para o tipo principal da calculadora..
        /// </summary>
        /// <param name="instancia">Instancia principal.</param>
        ICalculoRentabilidadeResultado Calcular(GDA.GDASession sessao, T instancia);

        #endregion
    }
}
