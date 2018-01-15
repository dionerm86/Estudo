using System;
using System.Web.UI.WebControls;

namespace Glass.UI.Web.Cadastros
{
    public partial class CadAntecipContaRec : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            ((TextBox)ctrlDataRecebimento.FindControl("txtData")).Text = DateTime.Now.ToString("dd/MM/yyyy");
            ((ImageButton)ctrlDataRecebimento.FindControl("imgData")).Visible = true;
    
            Ajax.Utility.RegisterTypeForAjax(typeof(Glass.UI.Web.Cadastros.CadAntecipContaRec));
        }
    
        [Ajax.AjaxMethod()]
        public string GetContasRecFromPedido(string idPedido)
        {
            return WebGlass.Business.ContasReceber.Fluxo.BuscarEValidar.Ajax.GetContasRecFromPedido(idPedido);
        }
    
        [Ajax.AjaxMethod()]
        public string Antecipar(string idsContasRec, string idContaBanco, string total, string taxa, string juros, 
            string iof, string dataRec, string obs)
        {
            return WebGlass.Business.ContasReceber.Fluxo.Antecipacao.Ajax.Antecipar(idsContasRec, 
                idContaBanco, total, taxa, juros, iof, dataRec, obs);
        }
    }
}
