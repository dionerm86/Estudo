using System;
using System.Web.UI;

namespace Glass.UI.Web.Utils
{
    public partial class SetMotivoCancCotacaoCompra : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            
        }
    
        protected void btnConfirmar_Click(object sender, EventArgs e)
        {
            uint idCotacaoCompra = Glass.Conversoes.StrParaUint(Request["idCotacaoCompra"]);
    
            try
            {
                WebGlass.Business.CotacaoCompra.Fluxo.CancelarCotacaoCompra.Instance.Cancelar(idCotacaoCompra, txtMotivo.Text);
                Page.ClientScript.RegisterClientScriptBlock(GetType(), "ok",
                    "alert('Cotação de compra cancelada.'); window.opener.redirectUrl(window.opener.location.href); closeWindow();", true);
            }
            catch (Exception ex)
            {
                Glass.MensagemAlerta.ErrorMsg("Falha ao cancelar cotação de compra.", ex, Page);
            }
        }
    }
}
