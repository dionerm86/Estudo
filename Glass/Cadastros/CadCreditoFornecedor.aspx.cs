using System;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace Glass.UI.Web.Cadastros
{
    public partial class CadCreditoFornecedor : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            Ajax.Utility.RegisterTypeForAjax(typeof(MetodosAjax));
    
            if (!string.IsNullOrEmpty(Request["id"]))
                dtvCredFornec.ChangeMode(DetailsViewMode.Edit);
    
        }
    
        protected void ctrlFormaPagto1_Load(object sender, EventArgs e)
        {
            Glass.UI.Web.Controls.ctrlFormaPagto ctrlFormaPagto = (Glass.UI.Web.Controls.ctrlFormaPagto)sender;
        }
    
        protected void btnCancelar_Click(object sender, EventArgs e)
        {
            Response.Redirect("~/Listas/LstCreditoFornecedor.aspx");
        }
        protected void odsCredFornec_Inserted(object sender, Colosoft.WebControls.VirtualObjectDataSourceStatusEventArgs e)
        {           
            if (e.Exception == null)
            {
                string textoMensagem = "Crédito gerado.";
                Page.ClientScript.RegisterClientScriptBlock(GetType(), "redirect", "alert('" + textoMensagem + "'); redirectUrl('" +
                    this.ResolveClientUrl("~/Listas/LstCreditoFornecedor.aspx") + "');", true);
    
                Response.Redirect("~/Listas/LstCreditoFornecedor.aspx");
            }
            else
            {
                Glass.MensagemAlerta.ErrorMsg("Falha ao cadastrar crédito.", e.Exception, this);
                e.ExceptionHandled = true;
            }
        }
    }
}
