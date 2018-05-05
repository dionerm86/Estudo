using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Glass.Rentabilidade
{
    /// <summary>
    /// Assinatura do provedor de indicadores financeiros.
    /// </summary>
    public interface IProvedorIndicadorFinanceiro
    {
        #region Propriedades

        /// <summary>
        /// Nome dos indicadores disponíveis. 
        /// </summary>
        IEnumerable<string> Nomes { get; }

        /// <summary>
        /// Recupera o valor do indicador financiero pelo nome informado.
        /// </summary>
        /// <param name="nome">Nome do indicador.</param>
        /// <returns></returns>
        decimal this[string nome] { get; }

        #endregion

        #region Métodos

        /// <summary>
        /// Verifica se existe um indicador com o nome informado.
        /// </summary>
        /// <param name="nome">Nome do indicador.</param>
        /// <returns></returns>
        bool Contains(string nome);

        #endregion
    }
}
