using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Colosoft;
using System.Runtime.Serialization;
using System.Linq.Expressions;

namespace Glass.Mathematical
{
    /// <summary>
    /// Representa uma expressão para o calculo da rentabilidade.
    /// </summary>
    class Expressao : IVariavel, IProvedorDependencias, IFormulaContainer
    {
        #region Variáveis Locais

        private Delegate _expressaoCompilada;
        private LambdaExpression _lambdaExpression;
        private readonly Colosoft.Threading.SimpleMonitor _stateMonitor = new Colosoft.Threading.SimpleMonitor();
        private List<string> _variaveisDependencia = new List<string>();

        #endregion

        #region Propriedades

        /// <summary>
        /// Tipo de variável.
        /// </summary>
        public TipoVariavel TipoVariavel => TipoVariavel.Expressao;

        /// <summary>
        /// Texto de expressão.
        /// </summary>
        public string Texto { get; }

        /// <summary>
        /// Nome.
        /// </summary>
        public string Nome { get; }

        /// <summary>
        /// Formula associada.
        /// </summary>
        public Formula Formula { get; }

        /// <summary>
        /// Expressão compilada.
        /// </summary>
        private Delegate ExpressaoCompilada
        {
            get
            {
                if (_expressaoCompilada == null)
                    Compilar();

                return _expressaoCompilada;
            }
        }

        #endregion

        #region Construtores

        /// <summary>
        /// Construtor padrão.
        /// </summary>
        /// <param name="nome">Nome da expressão.</param>
        /// <param name="texto">Texto associado.</param>
        /// <param name="formula">Formula pai.</param>
        internal Expressao(string nome, string texto, Formula formula)
        {
            nome.Require(nameof(nome)).NotNull().NotEmpty();
            texto.Require(nameof(texto)).NotNull().NotEmpty();
            formula.Require(nameof(formula)).NotNull();

            Nome = nome;
            Texto = texto;
            Formula = formula;
        }

        #endregion

        #region Métodos Privados

        /// <summary>
        /// Converte a variável para uma expressão.
        /// </summary>
        /// <param name="variavel"></param>
        /// <returns></returns>
        private static ParameterExpression Converter(IVariavel variavel)
        {
            return ParameterExpression.Variable(typeof(double), variavel.Nome);
        }

        /// <summary>
        /// Instancia da expressão associada.
        /// </summary>
        private System.Linq.Expressions.LambdaExpression CriarExpression()
        {
            var variaveis = Formula.Variaveis.Where(f => f != this && f.Nome != this.Nome);

            var parameters = new[]
            {
                ParameterExpression.Variable(typeof(Accessors.ExpressaoAccessor), "$f")
            }.Concat(variaveis.Select(Converter)).ToArray();

            return Colosoft.Linq.Dynamics.DynamicExpression.ParseLambda(parameters, typeof(double), Texto);
        }

        /// <summary>
        /// Executa o calculo do valor da expressão.
        /// </summary>
        /// <param name="variaveis">Variáveis que serão usadas no cálculo da expressão.</param>
        /// <returns></returns>
        private double Calcular(IVariavelCollection variaveis)
        {
            variaveis.Require(nameof(variaveis)).NotNull();

            if (ExpressaoCompilada != null)
                try
                {
                    var accessor = new Accessors.ExpressaoAccessor(this, variaveis);

                    var values = new object[] { accessor }
                        .Concat(
                            _lambdaExpression
                                .Parameters
                                .Skip(1)
                                // Recupera somente os valores das variáveis de dependencia
                                .Select(f => _variaveisDependencia.Contains(f.Name) ? variaveis[f.Name] : 0.0)
                                .OfType<object>())
                        .ToArray();

                    return (double)ExpressaoCompilada.DynamicInvoke(values);
                }
                catch (Exception ex)
                {
                    throw new InvalidOperationException("Um erro ocorreu ao tentar compilar a expressão.", ex);
                }

            return 0.0;
        }

        #endregion

        #region Métodos Públicos

        /// <summary>
        /// Valor da expressão.
        /// </summary>
        public double ObterValor()
        {
            if (Formula == null)
                throw new InvalidOperationException("Não foi contrada nenhuma formula associada com a expressão.");

            return Calcular(Formula.Variaveis as IVariavelCollection);
        }

        /// <summary>
        /// Executa a compilação da expressão.
        /// </summary>
        public void Compilar()
        {
            if (!_stateMonitor.Busy)
            {
                _stateMonitor.Enter();

                try
                {
                    _lambdaExpression = CriarExpression();

                    var visitor = new Visitors.ExpressionVisitor(this, _lambdaExpression);
                    visitor.Visit();
                    _expressaoCompilada = _lambdaExpression.Compile();
                }
                finally
                {
                    _stateMonitor.Dispose();
                }
            }
        }


        /// <summary>
        /// Cria uma cópia da expressão para a formula informda.
        /// </summary>
        /// <param name="formula"></param>
        /// <returns></returns>
        public Expressao CreateCopy(Formula formula)
        {
            var expressao = new Expressao(Nome, Texto, formula);

            Compilar();

            expressao._expressaoCompilada = _expressaoCompilada;
            expressao._lambdaExpression = _lambdaExpression;
            expressao._variaveisDependencia = _variaveisDependencia;

            return expressao;
        }

        /// <summary>
        /// Recupera o texto que representa a instancia.
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            return $"{Nome}: ({Texto})";
        }

        #endregion

        #region Membros IProvedorDependencias

        /// <summary>
        /// Variáveis de dependencia.
        /// </summary>
        IEnumerable<string> IProvedorDependencias.Variaveis => _variaveisDependencia;

        /// <summary>
        /// Adiciona a variável de dependencia.
        /// </summary>
        /// <param name="nome"></param>
        void IProvedorDependencias.AdicionarDependencia(string nome)
        {
            if (!_variaveisDependencia.Contains(nome))
                _variaveisDependencia.Add(nome);
        }

        #endregion
    }
}
