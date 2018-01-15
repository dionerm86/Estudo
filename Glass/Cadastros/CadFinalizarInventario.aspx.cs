using System;
using System.Web.UI;

namespace Glass.UI.Web.Cadastros
{
    public partial class CadFinalizarInventario : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
    
        }
    
        protected void btnCancelar_Click(object sender, EventArgs e)
        {
            Page.ClientScript.RegisterClientScriptBlock(GetType(), "fechar", "window.opener.atualizarPagina(); closeWindow();", true);
        }
    
        protected void btnAtualizar_Click(object sender, EventArgs e)
        {
            try
            {
                for (int i = 0; i < grdProdutos.Rows.Count; i++)
                    grdProdutos.UpdateRow(i, false);
    
                WebGlass.Business.InventarioEstoque.Fluxo.Finalizar.Instance.FinalizarInventario(Glass.Conversoes.StrParaUint(Request["id"]));
                btnCancelar_Click(sender, e);
            }
            catch (Exception ex)
            {
                Glass.MensagemAlerta.ErrorMsg("Falha ao finalizar inventário.", ex, Page);
            }
        }
    }
}
