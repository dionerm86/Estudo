using System;
using System.Web.UI;

namespace Glass.UI.Web.Cadastros.Projeto
{
    public partial class CadTextoOrcamento : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
    
        }
    
        protected void imgPesq_Click(object sender, ImageClickEventArgs e)
        {
            grdModelos.PageIndex = 0;
        }
    
        protected void odsProjetoModelo_Updated(object sender, Colosoft.WebControls.VirtualObjectDataSourceStatusEventArgs e)
        {
            if (e.Exception != null)
            {
                e.ExceptionHandled = true;
                Glass.MensagemAlerta.ErrorMsg("Falha ao atualizar texto de orçamento.", e.Exception, Page);
            }
            else
                Glass.MensagemAlerta.ShowMsg("Texto de orçamento atualizado.", Page);
        }
    }
}
