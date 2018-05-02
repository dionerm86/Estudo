using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Colosoft;

namespace Glass.Mathematical
{
    /// <summary>
    /// Representa uma formula para calculo.
    /// </summary>
    public class Formula : IVariavel
    {
        #region Variáveis Locais

        private readonly string _nome;
        private Expressao _expressao;
        private readonly AggregateVariavelCollection _agregadorVariaveis = new AggregateVariavelCollection();

        #endregion

        #region Propriedades

        /// <summary>
        /// Tipo de variável.
        /// </summary>
        TipoVariavel IVariavel.TipoVariavel => TipoVariavel.Expressao;

        /// <summary>
        /// Nome da fórmula.
        /// </summary>
        public string Nome => _nome;

        /// <summary>
        /// Coleção das variáveis.
        /// </summary>
        public IEnumerable<IVariavel> Variaveis => _agregadorVariaveis;

        /// <summary>
        /// Recupera o valor de uma variável pelo nome informado.
        /// </summary>
        /// <param name="nome"></param>
        /// <returns></returns>
        public double this[string nome] => _agregadorVariaveis[nome];

        #endregion

        #region Construtores

        /// <summary>
        /// Construtor padrão.
        /// </summary>
        /// <param name="nome">Nome da formula.</param>
        /// <param name="texto">Texto da formula.</param>
        /// <param name="variaveis">Variáveis que será usadas.</param>
        public Formula(string nome, string texto, IEnumerable<IVariavel> variaveis = null)
        {
            nome.Require(nameof(nome)).NotNull().NotEmpty();
            texto.Require(nameof(texto)).NotNull().NotEmpty();

            _nome = nome;
            _expressao = new Expressao(nome, texto, this);

            var variavelCollection = variaveis as IVariavelCollection;
            if (variavelCollection != null)
                _agregadorVariaveis.Add(variavelCollection);
            else if (variaveis != null)
                foreach (var i in variaveis)
                    _agregadorVariaveis.Add(i);

        }

        /// <summary>
        /// Construtor privador.
        /// </summary>
        /// <param name="variaveis"></param>
        private Formula(string nome, IEnumerable<IVariavel> variaveis)
        {
            _nome = nome;
            var variavelCollection = variaveis as IVariavelCollection;
            if (variavelCollection != null)
                _agregadorVariaveis.Add(variavelCollection);
            else if (variaveis != null)
                foreach (var i in variaveis)
                    _agregadorVariaveis.Add(i);
        }

        #endregion

        #region Métodos Públicos

        /// <summary>
        /// Valor da formula.
        /// </summary>
        public double ObterValor() => _expressao.ObterValor();

        /// <summary>
        /// Cria uma cópia da formula usando as variáveis informadas.
        /// </summary>
        /// <param name="formula"></param>
        /// <returns></returns>
        public Formula CreateCopy(IVariavelCollection variaveis)
        {
            var formula = new Formula(Nome, variaveis);
            var expressao = _expressao.CreateCopy(formula);
            formula._expressao = expressao;

            return formula;
        }

        /// <summary>
        /// Compila a fórmula.
        /// </summary>
        public void Compilar()
        {
            _expressao.Compilar();
        }

        /// <summary>
        /// Adiciona uma variável para a formula.
        /// </summary>
        /// <param name="variavel"></param>
        public void AdicionarVariavel(IVariavel variavel)
        {
            variavel.Require(nameof(variavel)).NotNull();
            _agregadorVariaveis.Add(variavel);
        }

        /// <summary>
        /// Adiciona uma coleção de variáveis.
        /// </summary>
        /// <param name="variableCollection"></param>
        public void AdicionarVariaveis(IVariavelCollection variableCollection)
        {
            variableCollection.Require(nameof(variableCollection)).NotNull();
            _agregadorVariaveis.Add(variableCollection);
        }

        /// <summary>
        /// Adiciona uma expressão para a fórmula.
        /// </summary>
        /// <param name="nome">Nome da expressão.</param>
        /// <param name="texto">Texto da expressão.</param>
        public void AdicionarExpressao(string nome, string texto)
        {
            _agregadorVariaveis.Add(new Expressao(nome, texto, this));
        }

        /// <summary>
        /// Adiciona uma constante para a expressão.
        /// </summary>
        /// <param name="nome">Nome da constante.</param>
        /// <param name="value">Valor.</param>
        public void AdicionarConstante(string nome, double value)
        {
            _agregadorVariaveis.Add(new Constante(nome, value));
        }

        /// <summary>
        /// Remove a variável da formula.
        /// </summary>
        /// <param name="variavel"></param>
        public bool RemoverVariavel(IVariavel variavel)
        {
            variavel.Require(nameof(variavel)).NotNull();
            return _agregadorVariaveis.Remove(variavel);
        }

        /// <summary>
        /// Recupera o texto que representa a instancia.
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            return _expressao.ToString();
        }

        #endregion
    }
}
