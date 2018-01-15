using System;

namespace Glass.UI.Web.Utils
{
    public partial class SetMotivoCancDepositoNaoIdentificado : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
    
        }
    
        protected void btnConfirmar_Click(object sender, EventArgs e)
        {
            uint idDepositoNaoIdentificado =  Glass.Conversoes.StrParaUint(Request["IdDepositoNaoIdentificado"]);
    
            try
            {
                WebGlass.Business.DepositoNaoIdentificado.Fluxo.DepositoNaoIdentificado.Instance.Cancelar(idDepositoNaoIdentificado, txtMotivo.Text);
            }
            catch (Exception ex)
            {
                Glass.MensagemAlerta.ErrorMsg(null, ex, Page);
                return;
            }
    
            ClientScript.RegisterClientScriptBlock(this.GetType(), "ok", "window.opener.redirectUrl(window.opener.location.href);closeWindow();", true);
        }
    
    }
}
