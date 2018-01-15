using System;
using System.Web.UI;

namespace Glass.UI.Web.Relatorios
{
    public partial class ListaProdTrib : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            Ajax.Utility.RegisterTypeForAjax(typeof(MetodosAjax));
        }
    
        protected void imgPesq_Click(object sender, ImageClickEventArgs e)
        {
            grdProduto.PageIndex = 0;
        }
    
        protected void lnkPesq_Click(object sender, EventArgs e)
        {
            grdProduto.PageIndex = 0;
        }
    
        protected void drpGrupo_SelectedIndexChanged(object sender, EventArgs e)
        {
            grdProduto.PageIndex = 0;
        }
    
        protected void drpSubgrupo_SelectedIndexChanged(object sender, EventArgs e)
        {
            grdProduto.PageIndex = 0;
        }
    
        protected void odsProdutos_Updated(object sender, Colosoft.WebControls.VirtualObjectDataSourceStatusEventArgs e)
        {
            if (e.Exception != null)
            {
                Glass.MensagemAlerta.ErrorMsg("Falha ao atualizar dados do produto.", e.Exception, Page);
                e.ExceptionHandled = true;
            }
        }
    }
}
