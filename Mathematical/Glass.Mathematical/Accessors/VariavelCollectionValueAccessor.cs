using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Glass.Mathematical.Accessors
{
    /// <summary>
    /// Implementação de um acessor do valor de varíaveis.
    /// </summary>
    class VariavelCollectionValueAccessor
    {
        #region Local Variables

        private readonly IVariavelCollection _variaveis;

        #endregion

        #region Properties

        /// <summary>
        /// Recupera o valor associado com o nome da variável.
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        public double this[string name]
        {
            get
            {
                return _variaveis[name];
            }
        }

        #endregion

        #region Constructors

        /// <summary>
        /// Construtor padrão.
        /// </summary>
        /// <param name="variaveis"></param>
        public VariavelCollectionValueAccessor(IVariavelCollection variaveis)
        {
            _variaveis = variaveis;
        }

        #endregion
    }
}
