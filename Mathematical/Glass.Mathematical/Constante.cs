using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Colosoft;

namespace Glass.Mathematical
{
    /// <summary>
    /// Representa um valor constante.
    /// </summary>
    public class Constante : IVariavel
    {
        #region Propriedades

        /// <summary>
        /// Tipo de variável.
        /// </summary>
        public TipoVariavel TipoVariavel => TipoVariavel.Constante;

        /// <summary>
        /// Nome.
        /// </summary>
        public string Nome { get; }

        /// <summary>
        /// Valor da constante.
        /// </summary>
        public double Valor { get; }

        #endregion

        #region Construtores

        /// <summary>
        /// Construtor padrão.
        /// </summary>
        /// <param name="nome"></param>
        /// <param name="valor"></param>
        public Constante(string nome, double valor)
        {
            nome.Require(nameof(nome)).NotNull().NotEmpty();
            Nome = nome;
            Valor = valor;
        }

        #endregion

        #region Métodos Públicos

        /// <summary>
        /// Recupera o valor da constante.
        /// </summary>
        /// <returns></returns>
        public double ObterValor() => Valor;

        /// <summary>
        /// Recupera o texto que representa a instancia.
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            return $"{Nome}: ({Valor})";
        }

        #endregion
    }
}
