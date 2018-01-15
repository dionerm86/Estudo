using System;
using System.Web.UI;

namespace Glass.UI.Web.Utils
{
    public partial class SetMotivoCancConciliacao : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            
        }
    
        protected void btnConfirmar_Click(object sender, EventArgs e)
        {
            uint codigoConciliacao = Glass.Conversoes.StrParaUint(Request["codigoConciliacao"]);
    
            try
            {
                WebGlass.Business.ConciliacaoBancaria.Fluxo.CRUD.Instance.CancelarConciliacao(codigoConciliacao, txtMotivo.Text);
                Page.ClientScript.RegisterClientScriptBlock(GetType(), "ok", 
                    "alert('Conciliação bancária cancelada.'); window.opener.redirectUrl(window.opener.location.href); closeWindow();", true);
            }
            catch (Exception ex)
            {
                Glass.MensagemAlerta.ErrorMsg("Falha ao cancelar conciliação bancária.", ex, Page);
            }
        }
    }
}
