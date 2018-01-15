using Microsoft.VisualStudio.TestTools.UnitTesting;
using WebGlass.Business.OrdemCarga.Fluxo;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Glass.Data.DAL;

namespace WebGlass.Business.OrdemCarga.Fluxo.Tests
{
    [TestClass()]
    public class CarregamentoTeste
    {
        [TestMethod()]
        public void EstornoCarregamentoTest()
        {

            var carregamentos = CarregamentoDAO.Instance.GetAll();
            //CarregamentoFluxo.Ajax.EfetuaLeitura()

            //Assert..Fail();
        }
    }
}