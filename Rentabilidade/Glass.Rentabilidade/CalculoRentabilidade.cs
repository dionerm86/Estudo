using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Colosoft;

namespace Glass.Rentabilidade
{
    /// <summary>
    /// Representa o calculo da rentabilidade.
    /// </summary>
    public class CalculoRentabilidade
    {
        #region Variáveis Locais

        private readonly List<Expressao> _expressoes = new List<Expressao>();

        #endregion

        #region Propriedades

        /// <summary>
        /// Expressões do calculo.
        /// </summary>
        public IEnumerable<Expressao> Expressoes => _expressoes.Select(f => f);

        /// <summary>
        /// Formula geral do calculo da rentabilidade.
        /// </summary>
        public string Formula { get; set; }

        #endregion

        #region Métodos Públicos

        /// <summary>
        /// Adiciona a expressa informada.
        /// </summary>
        /// <param name="expressao"></param>
        public void Add(Expressao expressao)
        {
            expressao.Require(nameof(expressao)).NotNull();
            _expressoes.Add(expressao);
        }

        /// <summary>
        /// Adiciona uma expressão para o cálculo.
        /// </summary>
        /// <param name="nome">Nome da expressão.</param>
        /// <param name="texto">Texto</param>
        public void Add(string nome, string texto)
        {
            nome.Require(nameof(nome)).NotNull().NotEmpty();
            texto.Require(nameof(texto)).NotNull().NotEmpty();

            Add(new Expressao(nome, texto));
        }

        #endregion

        #region Tipos Aninhados

        /// <summary>
        /// Representa uma expressao do calculo.
        /// </summary>
        public class Expressao
        {
            #region Propriedades

            /// <summary>
            /// Nome da expressão.
            /// </summary>
            public string Nome { get; }

            /// <summary>
            /// Texto da expressão.
            /// </summary>
            public string Texto { get; }

            #endregion

            #region Construtores

            /// <summary>
            /// Construtor padrão.
            /// </summary>
            /// <param name="nome">Nome da expressão.</param>
            /// <param name="texto">Texto que representa a expressão.</param>
            public Expressao(string nome, string texto)
            {
                nome.Require(nameof(nome)).NotNull().NotEmpty();
                Nome = nome;
                Texto = texto;
            }

            #endregion
        }

        #endregion
    }
}
