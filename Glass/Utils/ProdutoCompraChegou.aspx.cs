using System;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Glass.Data.DAL;
using System.Collections.Generic;

namespace Glass.UI.Web.Utils
{
    public partial class ProdutoCompraChegou : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            lblIdCompra.Text = Request["idCompra"];
        }
    
        protected void btnFinalizar_Click(object sender, EventArgs e)
        {
            try
            {
                Dictionary<uint, float> idProdQtdeNf = new Dictionary<uint, float>();
    
                foreach (GridViewRow r in grdProdutosCompra.Rows)
                {
                    float qtdeContabil = Glass.Conversoes.StrParaFloat(((TextBox)r.FindControl("txtQtdeContabil")).Text);
                    if (qtdeContabil > 0)
                    {
                        uint idProdCompra = Glass.Conversoes.StrParaUint(((HiddenField)r.FindControl("hdfIdProdCompra")).Value);
                        idProdQtdeNf.Add(idProdCompra, qtdeContabil);
                    }
    
                    decimal valorFiscalAlterado = Glass.Conversoes.StrParaDecimal(((TextBox)r.FindControl("txtValorFiscal")).Text);
                    decimal valorFiscalProduto = Glass.Conversoes.StrParaDecimal(((HiddenField)r.FindControl("hdfValorFiscal")).Value);
    
                    if (valorFiscalProduto.CompareTo(valorFiscalAlterado) != 0)
                    {
                        uint idProd = Glass.Conversoes.StrParaUint(((HiddenField)r.FindControl("hdfIdProd")).Value);
                        ProdutoDAO.Instance.AtualizaValorFiscal(idProd, valorFiscalAlterado);
                    }
                }
    
                uint? idNaturezaOperacao = ctrlNaturezaOperacao.CodigoNaturezaOperacao;
                if (idProdQtdeNf.Count > 0 && idNaturezaOperacao.GetValueOrDefault() == 0)
                {
                    Glass.MensagemAlerta.ShowMsg("Selecione a natureza da operação.", Page);
                    return;
                }
    
                uint idNf = CompraDAO.Instance.FinalizarAndamento(Glass.Conversoes.StrParaUint(Request["idCompra"]), idNaturezaOperacao, idProdQtdeNf);
    
                string script = "";
    
                if (idProdQtdeNf.Count > 0)
                    script += "window.opener.redirectUrl('" + Data.Helper.Utils.GetFullUrl(HttpContext.Current, "~/Cadastros/CadNotaFiscal.aspx") +
                        "?idNf=" + idNf + "');";
                else
                    script += "window.opener.redirectUrl('" + Data.Helper.Utils.GetFullUrl(HttpContext.Current, "~/Listas/LstCompras.aspx") + "');";
    
                script += "alert('Compra finalizada!'); closeWindow(); ";
    
                Page.ClientScript.RegisterClientScriptBlock(GetType(), "fechar", script, true);
            }
            catch (Exception ex)
            {
                Glass.MensagemAlerta.ErrorMsg("Falha ao finalizar compra.", ex, Page);
            }
        }
    }
}
