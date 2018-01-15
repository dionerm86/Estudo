using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Glass.Data.DAL;

namespace WebGlass.Business.Cfop.Ajax
{
    public interface IBuscarEValidar
    {
        string CfopCalcSt(string idCfop, string idLoja);
    }

    internal class BuscarEValidar : IBuscarEValidar
    {
        public string CfopCalcSt(string idCfop, string idLoja)
        {
            return CfopDAO.Instance.CalculaIcmsSt(null, uint.Parse(idCfop), uint.Parse(idLoja)).ToString().ToLower();
        }
    }
}
