using System;
using System.Web.UI;
using System.Web.UI.WebControls;
using Glass.Data.DAL;
using Glass.Data.Model;

namespace Glass.UI.Web.Utils
{
    public partial class InfoAdicProdNf : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            
        }
    
        protected uint GetIdProdNf()
        {
            return Glass.Conversoes.StrParaUint(Request["idProdNf"]);
        }
    
        protected void imgAdd_Click(object sender, ImageClickEventArgs e)
        {
            try
            {
                ProdutoNfAdicao p = new ProdutoNfAdicao();
                p.IdProdNf = GetIdProdNf();
                p.NumAdicao = Glass.Conversoes.StrParaInt(((TextBox)grdProdNfAdic.FooterRow.FindControl("txtNumAdicao")).Text);
                p.DescAdicao = Glass.Conversoes.StrParaFloat(((TextBox)grdProdNfAdic.FooterRow.FindControl("txtDesconto")).Text);
                p.CodFabricante = ((TextBox)grdProdNfAdic.FooterRow.FindControl("txtCodFabric")).Text;
    
                ProdutoNfAdicaoDAO.Instance.Insert(p);
                grdProdNfAdic.DataBind();
            }
            catch (Exception ex)
            {
                Glass.MensagemAlerta.ErrorMsg("Falha ao inserir adição no produto da nota fiscal.", ex, Page);
            }
        }
    
        protected void grdProdNfAdic_DataBound(object sender, EventArgs e)
        {
            if (grdProdNfAdic.Rows.Count == 1 && grdProdNfAdic.EditIndex == -1)
                grdProdNfAdic.Rows[0].Visible = ((Label)grdProdNfAdic.Rows[0].FindControl("Label1")).Text != "0";
            else
                grdProdNfAdic.Rows[0].Visible = true;
        }
    
        protected void grdProdNfAdic_RowCommand(object sender, GridViewCommandEventArgs e)
        {
            grdProdNfAdic.ShowFooter = e.CommandName != "Edit";
        }
    }
}
