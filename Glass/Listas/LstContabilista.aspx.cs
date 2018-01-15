using System;

namespace Glass.UI.Web.Listas
{
    public partial class LstContabilista : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
    
        }
    
        protected void lnkInserir_Click(object sender, EventArgs e)
        {
            Response.Redirect("../Cadastros/CadContabilista.aspx");
        }
    
        protected void odsContabilista_Deleted(object sender, Colosoft.WebControls.VirtualObjectDataSourceStatusEventArgs e)
        {
            if (e.Exception != null)
            {
                Glass.MensagemAlerta.ErrorMsg("Falha ao excluir contabilista.", e.Exception, Page);
                e.ExceptionHandled = true;
            }
        }
    }
}
