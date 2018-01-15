using System;
using System.Web.UI;
using System.Web.UI.WebControls;
using Glass.Data.DAL;
using Glass.Data.Model;

namespace Glass.UI.Web.Listas
{
    public partial class LstObsLancFiscal : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
    
        }
    
        protected void grdObsLancFiscal_RowCommand(object sender, GridViewCommandEventArgs e)
        {
            grdObsLancFiscal.ShowFooter = e.CommandName != "Edit";
        }
    
        protected void grdObsLancFiscal_DataBound(object sender, EventArgs e)
        {
            if (grdObsLancFiscal.Rows.Count == 1)
                grdObsLancFiscal.Rows[0].Visible = ObsLancFiscalDAO.Instance.GetCountReal(0, 0) > 0;
            else
                grdObsLancFiscal.Rows[0].Visible = true;
        }
    
        protected void imgAdd_Click(object sender, ImageClickEventArgs e)
        {
            ObsLancFiscal nova = new ObsLancFiscal();
            nova.Descricao = ((TextBox)grdObsLancFiscal.FooterRow.FindControl("txtDescricao")).Text;
    
            if (nova.Descricao.Length > 200)
                nova.Descricao = nova.Descricao.Substring(0, 200);
    
            ObsLancFiscalDAO.Instance.Insert(nova);
            grdObsLancFiscal.DataBind();
        }
    
        protected void grdObsLancFiscal_RowDeleted(object sender, GridViewDeletedEventArgs e)
        {
            if (e.Exception != null)
            {
                e.ExceptionHandled = true;
                Glass.MensagemAlerta.ErrorMsg("Falha ao excluir observação do lançamento fiscal.", e.Exception, Page);
            }
        }
    }
}
