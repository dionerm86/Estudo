using System;

namespace Glass.UI.Web.Cadastros
{
    public partial class CadCustoFixoRetificar : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            Ajax.Utility.RegisterTypeForAjax(typeof(Glass.UI.Web.Cadastros.CadCustoFixoRetificar));
        }
    
        /// <summary>
        /// Retifica o custo fixo via AJAX
        /// </summary>
        /// <param name="idCustoFixo"></param>
        /// <param name="mesAno"></param>
        /// <param name="valor"></param>
        /// <param name="diaVenc"></param>
        /// <returns></returns>
        [Ajax.AjaxMethod()]
        public string Retificar(string idCustoFixo, string mesAno, string valor, string diaVenc)
        {
            return WebGlass.Business.CustoFixo.Fluxo.Retificar.Ajax.RetificarCustoFixo(idCustoFixo, mesAno,
                valor, diaVenc);
        }
    }
}
