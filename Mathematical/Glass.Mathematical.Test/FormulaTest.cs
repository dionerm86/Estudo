using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Glass.Mathematical.Test
{
    [TestClass]
    public class FormulaTest
    {
        /// <summary>
        /// Testa a formula com uma expressão de soma.
        /// </summary>
        [TestMethod]
        public void FormulaComExpressaoDeSoma()
        {
            var formula = new Formula("f1", "1 + 2");
            Assert.AreEqual(3, formula.ObterValor());
        }

        /// <summary>
        /// Testar a criação da uma formula que trabalho com 
        /// valores constantes.
        /// </summary>
        [TestMethod]
        public void FormulaTrabalhandoComConstantes()
        {
            var formula = new Formula("f1", "a + b");
            formula.AdicionarConstante("a", 10);
            formula.AdicionarConstante("b", 15);

            Assert.AreEqual(25, formula.ObterValor());
        }

        [TestMethod]
        public void FormulaTrabalhandoComExpressoes()
        {
            var formula = new Formula("f1", "(a + b) * m");
            formula.AdicionarConstante("a", 1);
            formula.AdicionarConstante("b", 1);
            formula.AdicionarExpressao("m", "Math.Sqrt(64) + 1");

            Assert.AreEqual(18, formula.ObterValor());
        }

        [TestMethod]
        public void FormulasEmEscoposDiferentes()
        {
            var formula1 = new Formula("f1", "a + b");
            formula1.AdicionarConstante("a", 3);
            formula1.AdicionarConstante("b", 5);

            var formula2 = new Formula("f2", "a + b");
            formula2.AdicionarConstante("a", 7);
            formula2.AdicionarConstante("b", 10);

            var formula3 = new Formula("f3", "f1 + f2");
            formula3.AdicionarVariavel(formula1);
            formula3.AdicionarVariavel(formula2);

            Assert.AreEqual(25, formula3.ObterValor());
        }
    }
}
