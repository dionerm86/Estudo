using System;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace Glass.UI.Web.Cadastros
{
    public partial class CadInventarioEstoque : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack && !String.IsNullOrEmpty(Request["id"]))
                dtvInventarioEstoque.ChangeMode(DetailsViewMode.Edit);
        }
    
        protected void btnCancelar_Click(object sender, EventArgs e)
        {
            Page.ClientScript.RegisterClientScriptBlock(GetType(), "fechar", "window.opener.atualizarPagina(); closeWindow();", true);
        }
    
        protected void odsInventarioEstoque_Inserted(object sender, Colosoft.WebControls.VirtualObjectDataSourceStatusEventArgs e)
        {
            if (e.Exception != null)
            {
                e.ExceptionHandled = true;
                Glass.MensagemAlerta.ErrorMsg("Falha ao inserir inventário.", e.Exception, Page);
            }
            else
                btnCancelar_Click(sender, e);
        }
    
        protected void odsInventarioEstoque_Updated(object sender, Colosoft.WebControls.VirtualObjectDataSourceStatusEventArgs e)
        {
            if (e.Exception != null)
            {
                e.ExceptionHandled = true;
                Glass.MensagemAlerta.ErrorMsg("Falha ao atualizar inventário.", e.Exception, Page);
            }
            else
                btnCancelar_Click(sender, e);
        }
    }
}
