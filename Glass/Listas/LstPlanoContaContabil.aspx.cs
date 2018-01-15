using System;
using System.Web.UI;
using System.Web.UI.WebControls;
using Glass.Data.DAL;
using Glass.Data.Model;
using Glass.Data.Helper;

namespace Glass.UI.Web.Listas
{
    public partial class LstPlanoContaContabil : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {

        }
    
        protected void grdPlanoContaCont_DataBound(object sender, EventArgs e)
        {
            if (grdPlanoContaCont.Rows.Count == 1)
                grdPlanoContaCont.Rows[0].Visible = PlanoContaContabilDAO.Instance.GetCountReal() > 0;
            else
                grdPlanoContaCont.Rows[0].Visible = true;
        }
    
        protected void grdPlanoContaCont_RowCommand(object sender, GridViewCommandEventArgs e)
        {
            grdPlanoContaCont.ShowFooter = e.CommandName != "Edit";
        }
    
        protected void imgAdd_Click(object sender, ImageClickEventArgs e)
        {
            try
            {
                string codInterno = ((TextBox)grdPlanoContaCont.FooterRow.FindControl("txtCodInterno")).Text;
                string descricao = ((TextBox)grdPlanoContaCont.FooterRow.FindControl("txtDescricao")).Text;
                string codNatureza = ((DropDownList)grdPlanoContaCont.FooterRow.FindControl("drpNatureza")).SelectedValue;
    
                PlanoContaContabil novo = new PlanoContaContabil();
                novo.CodInterno = codInterno;
                novo.Descricao = descricao;
                novo.Natureza = Glass.Conversoes.StrParaInt(codNatureza);
    
                PlanoContaContabilDAO.Instance.Insert(novo);
                grdPlanoContaCont.DataBind();
            }
            catch (Exception ex)
            {
                Glass.MensagemAlerta.ErrorMsg("Falha ao inserir plano de conta contábil.", ex, Page);
            }
        }
    
        protected void odsPlanoContaCont_Deleted(object sender, Colosoft.WebControls.VirtualObjectDataSourceStatusEventArgs e)
        {
            if (e.Exception != null)
            {
                Glass.MensagemAlerta.ErrorMsg("Falha ao excluir plano de conta contábil.", e.Exception, Page);
                e.ExceptionHandled = true;
            }
        }
    
        protected void odsPlanoContaCont_Updated(object sender, Colosoft.WebControls.VirtualObjectDataSourceStatusEventArgs e)
        {
            if (e.Exception != null)
            {
                Glass.MensagemAlerta.ErrorMsg("Falha ao atualizar plano de conta contábil.", e.Exception, Page);
                e.ExceptionHandled = true;
            }
        }
    }
}
