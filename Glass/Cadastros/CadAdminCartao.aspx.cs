using System;
using System.Web.UI.WebControls;

namespace Glass.UI.Web.Cadastros
{
    public partial class CadAdminCartao : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                if (!String.IsNullOrEmpty(Request["idAdminCartao"]))
                    dtvAdminCartao.ChangeMode(DetailsViewMode.Edit);
            }
        }
    
        protected void btnCancelar_Click(object sender, EventArgs e)
        {
            Response.Redirect("../Listas/LstAdminCartao.aspx");
        }
    
        protected void odsAdminCartao_Updated(object sender, Colosoft.WebControls.VirtualObjectDataSourceStatusEventArgs e)
        {
            if (e.Exception != null)
            {
                Glass.MensagemAlerta.ErrorMsg("Falha ao atualizar administradora de cartão.", e.Exception, Page);
                e.ExceptionHandled = true;
            }
            else
                btnCancelar_Click(sender, e);
        }
        protected void odsAdminCartao_Inserted(object sender, Colosoft.WebControls.VirtualObjectDataSourceStatusEventArgs e)
        {
            if (e.Exception != null)
            {
                Glass.MensagemAlerta.ErrorMsg("Falha ao inserir administradora de cartão.", e.Exception, Page);
                e.ExceptionHandled = true;
            }
            else
                btnCancelar_Click(sender, e);
        }
    }
}
