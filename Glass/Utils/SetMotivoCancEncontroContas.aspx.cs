using System;

namespace Glass.UI.Web.Utils
{
    public partial class SetMotivoCancEncontroContas : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
    
        }
    
        protected void btnConfirmar_Click(object sender, EventArgs e)
        {
            uint idEncontroContas =  Glass.Conversoes.StrParaUint(Request["IdEncontroContas"]);
    
            try
            {
                WebGlass.Business.EncontroContas.Fluxo.EncontroContas.Instance.Cancelar(idEncontroContas, txtMotivo.Text);
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
