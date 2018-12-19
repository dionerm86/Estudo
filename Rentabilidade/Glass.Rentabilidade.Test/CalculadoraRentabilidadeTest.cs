using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Collections.Generic;

namespace Glass.Rentabilidade.Test
{
    [TestClass]
    public class CalculadoraRentabilidadeTest
    {
        [TestMethod]
        public void CalculoBasico()
        {
            var indicadoresFinanceiros = new Dictionary<string, decimal>();
            indicadoresFinanceiros.Add("PCustoADM", 0.18m);
            indicadoresFinanceiros.Add("PIMPFED", 0.0593m);
            indicadoresFinanceiros.Add("TaxaFinanceira", 0.03m);

            var calculo = new CalculoRentabilidade();
            calculo.Add("PrecoCusto2", "-PrecoCusto");
            calculo.Add("CustoFinanceiro", "PrecoVenda * -((TaxaFinanceira/30) * PrazoMedio)");
            calculo.Add("ICMSCompra", "PICMSCompra * PrecoVenda");
            calculo.Add("ICMSVenda", "PrecoVenda * -PICMSVenda");
            calculo.Add("SubstituicaoCompras", "-PrecoCusto * FatorICMSSubstituicao");
            calculo.Add("IPICompra", "PrecoCusto * PIPICompra");
            calculo.Add("IPIVenda", "PrecoVenda * -PIPIVenda");
            calculo.Add("Comissao", "PrecoVenda * -PComissao");
            calculo.Add("CustoADM", "PrecoVenda * -PCustoADM");
            calculo.Add("TOTIMPFED", "PIMPFED * -PrecoVenda");
            calculo.Add("Extras", "-CustosExtras");

            calculo.Formula = "PrecoCusto2 + CustoFinanceiro + ICMSCompra + ICMSVenda + SubstituicaoCompras + IPICompra + IPIVenda + Comissao + CustoADM + TOTIMPFED + Extras";

            var calculadora = new CalculadoraRentabilidade(new ProvedorIndicadorFinanceiro(indicadoresFinanceiros), calculo);
            calculadora.Preparar();

            var item = new MockRetabilidadeItem
            {
                PrecoVendaSemIPI = 10000,
                PrecoCusto = 4500,
                PrazoMedio = 30,
                PercentualICMSCompra = 0.18M,
                PercentualICMSVenda = 0.12M,
                FatorICMSSubstituicao = 0,
                PercentualIPICompra = 0,
                PercentualIPIVenda = 0,
                PercentualComissao = 0.03M,
                CustosExtras = 1500.0M
            };

            var rentabilidade = calculadora.Calcular(item);

            Assert.AreEqual(-8393, rentabilidade.CustoTotal);
            Assert.AreEqual(1607, rentabilidade.RentabilidadeFinanceira);
        }

        #region Tipos Aninhados

        class ProvedorIndicadorFinanceiro : IProvedorIndicadorFinanceiro
        {
            #region Variáveis Locais

            private readonly IDictionary<string, decimal> _valores;

            #endregion

            #region Propriedades

            /// <summary>
            /// Nome dos indicadores disponíveis. 
            /// </summary>
            public IEnumerable<string> Nomes => _valores.Keys;

            /// <summary>
            /// Recupera o valor do indicador financiero pelo nome informado.
            /// </summary>
            /// <param name="nome">Nome do indicador.</param>
            /// <returns></returns>
            public decimal this[string nome] => _valores[nome];

            #endregion

            #region Construtores

            public ProvedorIndicadorFinanceiro(IDictionary<string, decimal> valores)
	        {
                _valores = valores;
	        }

            #endregion

            #region Métodos

            /// <summary>
            /// Verifica se existe um indicador com o nome informado.
            /// </summary>
            /// <param name="nome">Nome do indicador.</param>
            /// <returns></returns>
            public bool Contains(string nome) => _valores.ContainsKey(nome);

            #endregion
        }

        #endregion
    }
}
