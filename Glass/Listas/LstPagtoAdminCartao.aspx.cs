using System;
using System.Web.UI;
using System.Web.UI.WebControls;
using Glass.Data.DAL;
using Glass.Data.Model;

namespace Glass.UI.Web.Listas
{
    public partial class LstPagtoAdminCartao : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
    
        }
    
        protected uint GetIdAdminCartao()
        {
            return Glass.Conversoes.StrParaUint(Request["idAdminCartao"]);
        }
    
        protected void grdPagtoAdminCartao_DataBound(object sender, EventArgs e)
        {
            if (grdPagtoAdminCartao.Rows.Count == 1)
                grdPagtoAdminCartao.Rows[0].Visible = PagtoAdministradoraCartaoDAO.Instance.GetCountReal(GetIdAdminCartao()) > 0;
            else
                grdPagtoAdminCartao.Rows[0].Visible = true;
        }
    
        protected void grdPagtoAdminCartao_RowCommand(object sender, GridViewCommandEventArgs e)
        {
            grdPagtoAdminCartao.ShowFooter = e.CommandName != "Edit";
        }
    
        protected void imgInsert_Click(object sender, ImageClickEventArgs e)
        {
            try
            {
                PagtoAdministradoraCartao novo = new PagtoAdministradoraCartao();
                novo.IdAdminCartao = GetIdAdminCartao();
                novo.IdLoja = Glass.Conversoes.StrParaUint(((DropDownList)grdPagtoAdminCartao.FooterRow.FindControl("drpLoja")).SelectedValue);
                novo.Mes = Glass.Conversoes.StrParaInt(((DropDownList)grdPagtoAdminCartao.FooterRow.FindControl("drpMes")).SelectedValue);
                novo.Ano = Glass.Conversoes.StrParaInt(((TextBox)grdPagtoAdminCartao.FooterRow.FindControl("txtAno")).Text);
                novo.ValorCredito = Glass.Conversoes.StrParaDecimal(((TextBox)grdPagtoAdminCartao.FooterRow.FindControl("txtValorCredito")).Text);
                novo.ValorDebito = Glass.Conversoes.StrParaDecimal(((TextBox)grdPagtoAdminCartao.FooterRow.FindControl("txtValorDebito")).Text);
    
                PagtoAdministradoraCartaoDAO.Instance.Insert(novo);
                grdPagtoAdminCartao.DataBind();
            }
            catch (Exception ex)
            {
                Glass.MensagemAlerta.ErrorMsg("Falha ao inserir pagamento.", ex, Page);
            }
        }
    
        protected void odsPagtoAdminCartao_Updated(object sender, Colosoft.WebControls.VirtualObjectDataSourceStatusEventArgs e)
        {
            if (e.Exception != null)
            {
                e.ExceptionHandled = true;
                Glass.MensagemAlerta.ErrorMsg("Falha ao atualizar pagamento.", e.Exception, Page);
            }
        }
    }
}
