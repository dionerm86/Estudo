using System;

namespace Glass.UI.Web.Listas
{
    public partial class LstProprietarioVeiculo : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            
        }
    
        protected void lnkInserir_Click(object sender, EventArgs e)
        {
            Response.Redirect("../Cadastros/CadProprietarioVeiculo.aspx");
        }
    
        protected void odsProprietarioVeiculo_Deleted(object sender, Colosoft.WebControls.VirtualObjectDataSourceStatusEventArgs e)
        {
            if (e.Exception != null)
            {
                Glass.MensagemAlerta.ErrorMsg("Falha ao excluir Proprietario.", e.Exception, Page);
                e.ExceptionHandled = true;
            }
        }
    }
}
