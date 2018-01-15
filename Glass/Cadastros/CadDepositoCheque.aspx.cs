using System;
using System.Web.UI.WebControls;

namespace Glass.UI.Web.Cadastros
{
    public partial class CadDepositoCheque : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            Ajax.Utility.RegisterTypeForAjax(typeof(Glass.UI.Web.Cadastros.CadDepositoCheque));
            ((TextBox)ctrlDataDeposito.FindControl("txtData")).Text = DateTime.Now.ToString("dd/MM/yyyy");
            contaBanco.Visible = true;
        }
    
        [Ajax.AjaxMethod()]
        public string GetCheques(string numero)
        {
            return WebGlass.Business.Cheque.Fluxo.BuscarEValidar.Ajax.GetCheques(numero);
        }
    
        [Ajax.AjaxMethod()]
        public string Confirmar(string idsCheque, string idContaBanco, string dataDeposito, string taxaAntecip, string valorDeposito, string obs, string noCache)
        {
            return WebGlass.Business.DepositoCheque.Fluxo.Confirmar.Ajax.ConfirmarDeposito(idsCheque, idContaBanco,
                dataDeposito, taxaAntecip, valorDeposito, obs, noCache);
        }
    }
}
