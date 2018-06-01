using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Glass.Data
{
    /// <summary>
    /// Assinatura da calculadora de impostos.
    /// </summary>
    /// <typeparam name="T">Tipo para o qual os impostos serão calculados.</typeparam>
    public interface ICalculadoraImposto<in T>
    {
        #region Métodos

        /// <summary>
        /// Realiza o calculo do imposto para a instancia informada.
        /// </summary>
        /// <param name="sessao">Sessão com o banco de dados que será usada para realizar os calculos.</param>
        /// <param name="instancia">Instancia para qual serão calculado os valores.</param>
        /// <returns></returns>
        ICalculoImpostoResultado Calcular(GDA.GDASession sessao, T instancia);

        #endregion
    }

    /// <summary>
    /// Classe com métodos para auxiliar o acesso a calculadora de impostos.
    /// </summary>
    static class CalculadoraImpostoHelper
    {
        #region Propriedades

        /// <summary>
        /// Instancia da calculadora de imposto.
        /// </summary>
        public static ICalculadoraImposto<T> ObterCalculadora<T>() => Microsoft.Practices.ServiceLocation
            .ServiceLocator.Current.GetInstance<ICalculadoraImposto<T>>();

        #endregion
    }
}
