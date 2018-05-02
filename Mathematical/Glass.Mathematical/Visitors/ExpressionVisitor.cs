using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace Glass.Mathematical.Visitors
{
    /// <summary>
    /// Implementação do Visitor para a expressão da função.
    /// </summary>
    class ExpressionVisitor : System.Linq.Expressions.ExpressionVisitor
    {
        #region Local Variables

        private readonly IProvedorDependencias _provedorDependencias;
        private readonly System.Linq.Expressions.Expression _expression;

        private bool _isFirstVisit = true;

        #endregion

        #region Constructors

        /// <summary>
        /// Construtor padrão.
        /// </summary>
        /// <param name="validation"></param>
        /// <param name="expression"></param>
        public ExpressionVisitor(IProvedorDependencias dependencyProvider, System.Linq.Expressions.Expression expression)
        {
            _provedorDependencias = dependencyProvider;
            _expression = expression;
        }

        #endregion

        #region Public Methods

        /// <summary>
        /// Visita a expressão.
        /// </summary>
        public void Visit()
        {
            Visit(_expression);
        }

        #endregion

        #region Protected Methods

        protected override Expression VisitLambda<T>(Expression<T> node)
        {
            if (_isFirstVisit)
            {
                var visitor2 = new ParametersVisitor(_provedorDependencias);
                visitor2.Visit(node.Body);
            }

            _isFirstVisit = false;
            return base.VisitLambda<T>(node);
        }

        /// <summary>
        /// Método acionado quando uma chamada de método é visitada.
        /// </summary>
        /// <param name="node"></param>
        /// <returns></returns>
        protected override System.Linq.Expressions.Expression VisitMethodCall(System.Linq.Expressions.MethodCallExpression node)
        {
            var result = base.VisitMethodCall(node);

            if (node.Method.Name == "get_Item" &&
                node.Method.DeclaringType == typeof(Accessors.VariavelCollectionValueAccessor))
            {
                var constantExpression = node.Arguments[0] as System.Linq.Expressions.ConstantExpression;

                if (constantExpression != null)
                {
                    var name = constantExpression.Value.ToString();
                    if (!_provedorDependencias.Variaveis.Contains(name))
                        _provedorDependencias.AdicionarDependencia(name);
                }
            }

            return result;
        }

        #endregion

        #region Tipos Aninhados

        class ParametersVisitor : System.Linq.Expressions.ExpressionVisitor
        {
            #region Variáveis Locais

            private readonly IProvedorDependencias _provedorDependencias;

            #endregion

            #region Constructors

            /// <summary>
            /// Construtor padrão.
            /// </summary>
            /// <param name="provedorDependencias"></param>
            public ParametersVisitor(IProvedorDependencias provedorDependencias)
            {
                _provedorDependencias = provedorDependencias;
            }

            #endregion

            /// <summary>
            /// Método acionado quando um parâmetro for visitado.
            /// </summary>
            /// <param name="node"></param>
            /// <returns></returns>
            protected override Expression VisitParameter(ParameterExpression node)
            {
                if (node != null)
                {
                    if (!_provedorDependencias.Variaveis.Contains(node.Name))
                        _provedorDependencias.AdicionarDependencia(node.Name);
                }
                return base.VisitParameter(node);
            }
        }

        #endregion
    }
}
