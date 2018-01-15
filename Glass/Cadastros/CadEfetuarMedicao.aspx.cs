using System;

namespace Glass.UI.Web.Cadastros
{
    public partial class CadEfetuarMedicao : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            Ajax.Utility.RegisterTypeForAjax(typeof(MetodosAjax));
            Ajax.Utility.RegisterTypeForAjax(typeof(Glass.UI.Web.Cadastros.CadEfetuarMedicao));
        }
    
        [Ajax.AjaxMethod()]
        public string Confirmar(string idsMedicao, string idMedidor, string dataEfetuar)
        {
            return WebGlass.Business.Medicao.Fluxo.Confirmar.Ajax.ConfirmarMedicao(idsMedicao, idMedidor, dataEfetuar);
        }
    
        [Ajax.AjaxMethod()]
        public string GetMedicao(string idMedicao)
        {
            return WebGlass.Business.Medicao.Fluxo.BuscarEValidar.Ajax.GetMedicao(idMedicao);
        }
    }
}
