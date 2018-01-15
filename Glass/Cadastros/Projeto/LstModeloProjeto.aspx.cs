using System;
using System.Web.UI;
using System.Web.UI.WebControls;
using Glass.Data.DAL;

namespace Glass.UI.Web.Cadastros.Projeto
{
    public partial class LstModeloProjeto : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            
        }
    
        protected void imgPesq_Click(object sender, ImageClickEventArgs e)
        {
            grdModeloProjeto.PageIndex = 0;
        }
    
        protected void grdModeloProjeto_RowCommand(object sender, GridViewCommandEventArgs e)
        {
            if (e.CommandName == "Inativar")
            {
                try
                {
                    ProjetoModeloDAO.Instance.AtivarInativarProjeto(Glass.Conversoes.StrParaUint(e.CommandArgument.ToString()));
                    grdModeloProjeto.DataBind();
                }
                catch (Exception ex)
                {
                    Glass.MensagemAlerta.ErrorMsg("Falha ao inativar modelo de projeto.", ex, Page);
                }
            }
        }
    
        protected void lnkNovoModelo_Click(object sender, EventArgs e)
        {
            Response.Redirect("CadModeloProjeto.aspx");
        }
    }
}
