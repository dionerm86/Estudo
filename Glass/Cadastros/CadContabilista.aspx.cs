using System;
using System.Web.UI.WebControls;

namespace Glass.UI.Web.Cadastros
{
    public partial class CadContabilista : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (Request["idContabilista"] != null)
                dtvCliente.ChangeMode(DetailsViewMode.Edit);
    
            Ajax.Utility.RegisterTypeForAjax(typeof(MetodosAjax));
        }
    
        protected void odsContabilista_Inserted(object sender, Colosoft.WebControls.VirtualObjectDataSourceStatusEventArgs e)
        {
            if (e.Exception != null)
            {
                Glass.MensagemAlerta.ErrorMsg("Falha ao cadastrar Contabilista.", e.Exception, Page);
                e.ExceptionHandled = true;
            }
            else
                Response.Redirect("../Listas/LstContabilista.aspx");
        }
    
        protected void odsContabilista_Updated(object sender, Colosoft.WebControls.VirtualObjectDataSourceStatusEventArgs e)
        {
            if (e.Exception != null)
            {
                Glass.MensagemAlerta.ErrorMsg("Falha ao atualizar dados do Contabilista.", e.Exception, Page);
                e.ExceptionHandled = true;
            }
            else
                Response.Redirect("../Listas/LstContabilista.aspx");
        }
    
        protected void btnCancelar_Click(object sender, EventArgs e)
        {
            Response.Redirect("../Listas/LstContabilista.aspx");
        }
    }
}
