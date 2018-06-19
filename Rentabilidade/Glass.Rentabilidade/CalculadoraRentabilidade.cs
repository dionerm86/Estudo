using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Colosoft;

namespace Glass.Rentabilidade
{
    /// <summary>
    /// Representa a calculadora a rentabilidade.
    /// </summary>
    public class CalculadoraRentabilidade
    {
        #region Variáveis Locais

        private Mathematical.IVariavelCollection _variaveisIndicadoresFinanceiro;
        private Mathematical.IVariavelCollection _variaveisCalculo;
        private Mathematical.Formula _formulaCalculo;

        #endregion

        #region Propriedades

        /// <summary>
        /// Indicadores financeiros que serão usados no cálculo de rentabilidade.
        /// </summary>
        public IProvedorIndicadorFinanceiro IndicadoresFinanceiros { get; }

        /// <summary>
        /// Cálculo da rentabilidade.
        /// </summary>
        public CalculoRentabilidade Calculo { get; }

        #endregion

        #region Construtores

        /// <summary>
        /// Construtor padrão.
        /// </summary>
        /// <param name="indicadoresFinanceiros">Dicionário com a relação dos indicadores financeiros.</param>
        /// <param name="calculo">Cálculo.</param>
        public CalculadoraRentabilidade(
            IProvedorIndicadorFinanceiro indicadoresFinanceiros, 
            CalculoRentabilidade calculo)
        {
            indicadoresFinanceiros.Require(nameof(indicadoresFinanceiros)).NotNull();
            calculo.Require(nameof(calculo)).NotNull();

            IndicadoresFinanceiros = indicadoresFinanceiros;
            Calculo = calculo;
        }

        #endregion

        #region Métodos Privados

        /// <summary>
        /// Cria as variáveis dos indicadores financeiros.
        /// </summary>
        /// <returns></returns>
        private Mathematical.IVariavelCollection CriarIndicadoresFinanceiros()
        {
            var constantes = IndicadoresFinanceiros.Nomes.Select(f =>
                new Mathematical.Constante(f, (double)IndicadoresFinanceiros[f]));

            return new Mathematical.VariavelCollection(constantes);
        }

        /// <summary>
        /// Cria as constantes para o item informado.
        /// </summary>
        /// <param name="item"></param>
        /// <returns></returns>
        private Mathematical.IVariavelCollection CriarVariaveisItem(IItemRentabilidade item)
        {
            var constantes = new[]
            {
                new Mathematical.Constante(nameof(TipoVariavelItemRentabilidade.PrecoVenda), (double)item.PrecoVendaSemIPI),
                new Mathematical.Constante(nameof(TipoVariavelItemRentabilidade.PrecoCusto), (double)item.PrecoCusto),
                new Mathematical.Constante(nameof(TipoVariavelItemRentabilidade.PrazoMedio), item.PrazoMedio),
                new Mathematical.Constante(nameof(TipoVariavelItemRentabilidade.PICMSCompra), (double)item.PercentualICMSCompra),
                new Mathematical.Constante(nameof(TipoVariavelItemRentabilidade.PICMSVenda), (double)item.PercentualICMSVenda),
                new Mathematical.Constante(nameof(TipoVariavelItemRentabilidade.FatorICMSSubstituicao), (double)item.FatorICMSSubstituicao),
                new Mathematical.Constante(nameof(TipoVariavelItemRentabilidade.PIPICompra), (double)item.PercentualIPICompra),
                new Mathematical.Constante(nameof(TipoVariavelItemRentabilidade.PIPIVenda), (double)item.PercentualIPIVenda),
                new Mathematical.Constante(nameof(TipoVariavelItemRentabilidade.PComissao), (double)item.PercentualComissao),
                new Mathematical.Constante(nameof(TipoVariavelItemRentabilidade.CustosExtras), (double)item.CustosExtras),
            };

            return new Mathematical.VariavelCollection(constantes);
        }

        /// <summary>
        /// Cria as variáveis para as expressões de calculo.
        /// </summary>
        /// <returns></returns>
        private Mathematical.IVariavelCollection CriarVariaveisCalculo()
        {
            var agregador = new Mathematical.AggregateVariavelCollection();
            agregador.Add(CriarIndicadoresFinanceiros());
            agregador.Add(CriarVariaveisItem(new ItemRentabilidadeFake()));

            foreach (var i in Calculo.Expressoes.Select(f => new Mathematical.Constante(f.Nome, 0.0)))
                agregador.Add(i);

            var formulas = new List<Mathematical.Formula>();
            foreach (var expressao in Calculo.Expressoes)
            {
                var formula = new Mathematical.Formula(expressao.Nome, expressao.Texto, agregador);
                formula.Compilar();

                formulas.Add(formula);
            }

            return new Mathematical.VariavelCollection(formulas);
        }

        #endregion

        #region Métodos Públicos

        /// <summary>
        /// Executa a preparação dos parametros da calculadora.
        /// </summary>
        public void Preparar()
        {
            _variaveisIndicadoresFinanceiro = CriarIndicadoresFinanceiros();
            _variaveisCalculo = CriarVariaveisCalculo();

            var formula = Calculo.Formula;

            if (string.IsNullOrEmpty(formula))
                formula = "0";

            // Monta a formula do calculo com a soma das expressões
            _formulaCalculo = new Mathematical.Formula("f1", formula, _variaveisCalculo);

            _formulaCalculo.Compilar();
        }

        /// <summary>
        /// Executa o calculo da rentabilidade sobre o item informado.
        /// </summary>
        /// <param name="item"></param>
        /// <returns></returns>
        public ResultadoRentabilidade Calcular(IItemRentabilidade item)
        {
            item.Require(nameof(item)).NotNull();

            var variaveisItem = CriarVariaveisItem(item);
            var agregador = new Mathematical.AggregateVariavelCollection();
            agregador.Add(variaveisItem);
            agregador.Add(_variaveisIndicadoresFinanceiro);

            // Percorre as variáveis de calculo compiladas
            var variaveisCalculo = new Mathematical.VariavelCollection(
                _variaveisCalculo.Select(f => ((Mathematical.Formula)f).CreateCopy(agregador)));

            agregador.Add(variaveisCalculo);

            var formula = _formulaCalculo.CreateCopy(agregador);

            var custoTotal = formula.ObterValor();

            return new ResultadoRentabilidade(_variaveisIndicadoresFinanceiro, variaveisItem, variaveisCalculo, custoTotal, item);
        }

        #endregion

        #region Nested Types

        /// <summary>
        /// Implementação base do item de rentabilidade que
        /// é usado somente para a compilação das formulas.
        /// </summary>
        class ItemRentabilidadeFake : IItemRentabilidade
        {
            #region Propriedades

            public string Descricao => "";

            public decimal FatorICMSSubstituicao => 0;

            public decimal PercentualComissao => 0;

            public decimal ValorComissao => 0;

            public decimal PercentualICMSCompra => 0;

            public decimal PercentualICMSVenda => 0;

            public decimal PercentualIPICompra => 0;

            public decimal PercentualIPIVenda => 0;

            public int PrazoMedio => 0;

            public decimal PrecoCusto => 0;

            public decimal PrecoVendaSemIPI => 0;

            public decimal CustosExtras => 0;

            /// <summary>
            /// Percentual de rentabilidade.
            /// </summary>
            public decimal PercentualRentabilidade { get; set; }

            /// <summary>
            /// Rentabilidade financeira.
            /// </summary>
            public decimal RentabilidadeFinanceira { get; set; }

            /// <summary>
            /// Registros de rentabilidade associados.
            /// </summary>
            public IEnumerable<IRegistroRentabilidade> RegistrosRentabilidade => new IRegistroRentabilidade[0];

            #endregion

            #region Métodos

            /// <summary>
            /// Limpa os registros de rentabilidade.
            /// </summary>
            public void LimparRegistros()
            {
                throw new NotImplementedException();
            }

            /// <summary>
            /// Cria uma instancia do registro.
            /// </summary>
            /// <param name="tipo">Tipo do registro.</param>
            /// <param name="nome">Nome do registro.</param>
            /// <param name="valor">Valor do registro</param>
            /// <returns></returns>
            public IRegistroRentabilidade CriarRegistro(TipoRegistroRentabilidade tipo, string nome, decimal valor) => null;

            #endregion
        }

        #endregion
    }
}
