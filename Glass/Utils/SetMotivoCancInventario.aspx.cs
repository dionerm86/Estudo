using System;
using System.Web.UI;

namespace Glass.UI.Web.Utils
{
    public partial class SetMotivoCancInventario : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            
        }
    
        protected void btnConfirmar_Click(object sender, EventArgs e)
        {
            uint codigoInventario = Glass.Conversoes.StrParaUint(Request["id"]);
    
            try
            {
                WebGlass.Business.InventarioEstoque.Fluxo.Cancelar.Instance.CancelarInventario(codigoInventario, txtMotivo.Text);
                Page.ClientScript.RegisterClientScriptBlock(GetType(), "ok", 
                    "alert('Inventário de estoque cancelado.'); window.opener.redirectUrl(window.opener.location.href); closeWindow();", true);
            }
            catch (Exception ex)
            {
                Glass.MensagemAlerta.ErrorMsg("Falha ao cancelar inventário de estoque.", ex, Page);
            }
        }
    }
}
