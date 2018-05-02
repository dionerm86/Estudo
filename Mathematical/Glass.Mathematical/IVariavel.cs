using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Glass.Mathematical
{
    /// <summary>
    /// Possíveis tipos de variável.
    /// </summary>
    public enum TipoVariavel
    {
        /// <summary>
        /// Constante.
        /// </summary>
        Constante,
        /// <summary>
        /// Variavel.
        /// </summary>
        Variavel,
        /// <summary>
        /// Expressão.
        /// </summary>
        Expressao
    }

    /// <summary>
    /// Assinatura de uma variável do calculo de rentabilidade.
    /// </summary>
    public interface IVariavel
    {
        #region Propriedades

        /// <summary>
        /// Tipo de variável.
        /// </summary>
        TipoVariavel TipoVariavel { get; }

        /// <summary>
        /// Nome da variável.
        /// </summary>
        string Nome { get; }

        #endregion

        #region Métodos

        /// <summary>
        /// Valor.
        /// </summary>
        double ObterValor();

        #endregion
    }
}
