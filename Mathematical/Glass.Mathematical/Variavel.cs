using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Colosoft;

namespace Glass.Mathematical
{
    /// <summary>
    /// Representa um valor constante.
    /// </summary>
    public class Variavel : IVariavel
    {
        #region Propriedades

        /// <summary>
        /// Tipo de variável.
        /// </summary>
        public TipoVariavel TipoVariavel => TipoVariavel.Variavel;

        /// <summary>
        /// Nome.
        /// </summary>
        public string Nome { get; }

        /// <summary>
        /// Valor da constante.
        /// </summary>
        public double Valor { get; set; }

        #endregion

        #region Constructors

        /// <summary>
        /// Construtor padrão.
        /// </summary>
        /// <param name="nome"></param>
        /// <param name="valor"></param>
        public Variavel(string nome, double valor)
        {
            nome.Require(nameof(nome)).NotNull().NotEmpty();
            Nome = nome;
            Valor = valor;
        }

        #endregion

        #region Métodos Públicos

        /// <summary>
        /// Recupera o valor da variável.
        /// </summary>
        /// <returns></returns>
        public double ObterValor() => Valor;

        #endregion
    }
}
