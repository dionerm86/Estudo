using Glass;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;

namespace Glass.Comum.Teste
{
    
    
    /// <summary>
    ///This is a test class for RepositorioEnumedoresTest and is intended
    ///to contain all RepositorioEnumedoresTest Unit Tests
    ///</summary>
    [TestClass()]
    public class RepositorioEnumedoresTest
    {

        /// <summary>
        ///A test for ObterInformacoes
        ///</summary>
        [TestMethod()]
        public void ObterInformacoesTest()
        {
            var actual = RepositorioEnumedores.ObterInformacoes(typeof(DiaSemana));
        }

        /// <summary>
        ///A test for ObterInformacoes
        ///</summary>
        [TestMethod()]
        public void ObterInformacoesProvedorTraducaoTest()
        {
            var actual = RepositorioEnumedores.ObterInformacoes(typeof(SituacaoRegistro));
        }

        /// <summary>
        ///A test for ObterInformacoes
        ///</summary>
        [TestMethod()]
        public void ObterInformacoesProvedorMultiplaTraducaoTest()
        {
            var traducao1 = RepositorioEnumedores.ObterInformacoes(typeof(TipoCalculo), 0);
            var traducao2 = RepositorioEnumedores.ObterInformacoes(typeof(TipoCalculo), 1);
        }

        /// <summary>
        ///A test for Traduz
        ///</summary>
        [TestMethod()]
        public void TraduzTest()
        {
            var actual = RepositorioEnumedores.Traduz(TipoCalculo.M2);
        }
    }
}
