using System;
using System.Web.UI;

namespace Glass.UI.Web.Relatorios
{
    public partial class ListaDeficitEstoque : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            Ajax.Utility.RegisterTypeForAjax(typeof(MetodosAjax));
        }
    
        protected void lnkPesq_Click(object sender, EventArgs e)
        {
            grdProdutos.PageIndex = 0;
        }
    
        protected void imgPesq_Click(object sender, ImageClickEventArgs e)
        {
            grdProdutos.PageIndex = 0;
        }
    }
}
