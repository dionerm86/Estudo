using System;
using System.Web.UI.WebControls;

namespace Glass.UI.Web.Listas
{
    public partial class LstEquipeInstalacao : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {

        }
    
        protected void lnkInserir_Click(object sender, EventArgs e)
        {
            Response.Redirect("../Cadastros/CadEquipeInstalacao.aspx");
        }
    
        protected void odsEquipe_Deleted(object sender, Colosoft.WebControls.VirtualObjectDataSourceStatusEventArgs e)
        {
            if (e.Exception != null)
            {
                Glass.MensagemAlerta.ErrorMsg("Falha ao excluir Equipe.", e.Exception, Page);
                e.ExceptionHandled = true;
            }
        }
    
        protected void grdEquipes_RowDataBound(object sender, GridViewRowEventArgs e)
        {

        }
    }
}
