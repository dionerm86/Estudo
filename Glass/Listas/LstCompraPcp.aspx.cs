using System;
using System.Web.UI;
using System.Web.UI.WebControls;
using Glass.Data.DAL;

namespace Glass.UI.Web.Listas
{
    public partial class LstCompraPcp : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (Request["idCompra"] != null)
                Page.ClientScript.RegisterStartupScript(GetType(), "exibida", "novaCompra(" + Request["idCompra"] + ");\n", true);
    
            Ajax.Utility.RegisterTypeForAjax(typeof(MetodosAjax));
    
        }
    
        protected void imgPesq_Click(object sender, ImageClickEventArgs e)
        {
            grdCompraPcp.PageIndex = 0;
        }
    
        protected void lnkInserir_Click(object sender, EventArgs e)
        {
            Response.Redirect("../Cadastros/CadCompraPcp.aspx");
        }
    
        protected void grdCompraPcp_RowCommand(object sender, GridViewCommandEventArgs e)
        {
            if (e.CommandName == "Cancelar")
            {
                try
                {
                    CompraDAO.Instance.CancelarCompra(Glass.Conversoes.StrParaUint(e.CommandArgument.ToString()), null);
                    grdCompraPcp.DataBind();
                }
                catch (Exception ex)
                {
                    if (ex.Message.ToLower().Contains("compra cancelada"))
                        grdCompraPcp.DataBind();
                    
                    Glass.MensagemAlerta.ShowMsg(ex.Message, Page);
                    return;
                }
            }
            else if (e.CommandName == "Reabrir")
            {
                try
                {
                    CompraDAO.Instance.ReabrirCompra(Glass.Conversoes.StrParaUint(e.CommandArgument.ToString()));
                    grdCompraPcp.DataBind();
                }
                catch (Exception ex)
                {
                    Glass.MensagemAlerta.ShowMsg(ex.Message, Page);
                    return;
                }
            }
            else if (e.CommandName == "GerarNFe")
            {
                try
                {
                    uint idCompra = Glass.Conversoes.StrParaUint(e.CommandArgument.ToString());
                    uint idNf = NotaFiscalDAO.Instance.GerarNfCompraComTransacao(idCompra, null, ProdutosCompraDAO.Instance.GetByCompra(idCompra));
    
                    ClientScript.RegisterClientScriptBlock(GetType(), "nfeGerada", "alert('Nota fiscal gerada com sucesso!'); " +
                        "redirectUrl('../Cadastros/CadNotaFiscal.aspx?idNf=" + idNf + "&tipo=3');", true);
                }
                catch (Exception ex)
                {
                    Glass.MensagemAlerta.ShowMsg(ex.Message, Page);
                    return;
                }
            }
        }
    
        protected void odsCompraPcp_Deleted(object sender, Colosoft.WebControls.VirtualObjectDataSourceStatusEventArgs e)
        {
            if (e.Exception != null)
            {
                Glass.MensagemAlerta.ErrorMsg(null, e.Exception, Page);
                e.ExceptionHandled = true;
            }
        }
    }
}
