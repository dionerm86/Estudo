using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Colosoft;

namespace Glass.Mathematical.Accessors
{
    /// <summary>
    /// Classe responsável por fornecer acesso as variáveis
    /// a partir da expressão.
    /// </summary>
    class ExpressaoAccessor
    {
        #region Variáveis Locais

        private readonly IVariavelCollection _variaveis;

        #endregion

        #region Propriedades

        /// <summary>
        /// Expressão associada.
        /// </summary>
        protected Expressao Expressao { get; }

        /// <summary>
        /// Recupera a variável pelo nome.
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        public double this[string name] => _variaveis[name];


        public double Valor => 0.0;

        #endregion

        #region Construtores

        /// <summary>
        /// Construtor padrão.
        /// </summary>
        /// <param name="expressao"></param>
        /// <param name="variavies"></param>
        public ExpressaoAccessor(Expressao expressao, IVariavelCollection variavies)
        {
            expressao.Require(nameof(expressao)).NotNull();
            variavies.Require(nameof(variavies)).NotNull();
            Expressao = expressao;
            _variaveis = variavies;
        }

        #endregion
    }
}
