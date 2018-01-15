using System;
using System.Web.UI;
using Glass.Data.DAL;

namespace Glass.UI.Web.Cadastros.Projeto
{
    public partial class LstProjeto : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
                grdProjeto.Columns[3].Visible = LojaDAO.Instance.GetCount() > 1;
    
            Ajax.Utility.RegisterTypeForAjax(typeof(MetodosAjax));
        }
    
        protected void imgPesq_Click(object sender, ImageClickEventArgs e)
        {
            grdProjeto.PageIndex = 0;
        }
    
        protected void odsProjeto_Deleted(object sender, Colosoft.WebControls.VirtualObjectDataSourceStatusEventArgs e)
        {
            if (e.Exception != null)
            {
                Glass.MensagemAlerta.ErrorMsg(null, e.Exception, Page);
                e.ExceptionHandled = true;
            }
        }
    
        protected void lnkEfetuarProjeto_Click(object sender, EventArgs e)
        {
            Response.Redirect("CadProjeto.aspx");
        }
    }
}
