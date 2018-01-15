using System;
using System.Configuration;
using Glass.Data.DAL;

namespace Glass.UI.Web.CFe
{
    public partial class FuncoesSat : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            Ajax.Utility.RegisterTypeForAjax(typeof(CFe.FuncoesSat));
        }
    
        #region Metodos Ajax
    
        [Ajax.AjaxMethod]
        public string ObterNumeroSessao()
        {
            string numSessao = CupomFiscalDAO.Instance.GerarNumeroSessao().ToString();
            return numSessao;
        }
    
        [Ajax.AjaxMethod]
        public string ObterCodigoAtivacao()
        {
            return ConfigurationManager.AppSettings.Get("CodigoAtivacaoSAT");
        }
    
        #endregion
    }
}
