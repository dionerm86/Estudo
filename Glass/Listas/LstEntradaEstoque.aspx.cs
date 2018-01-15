using System;
using System.Web.UI;

namespace Glass.UI.Web.Listas
{
    public partial class LstEntradaEstoque : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
    
        }
    
        protected void imgPesq_Click(object sender, ImageClickEventArgs e)
        {
            grdEntradas.PageIndex = 0;
            grdEntradas.DataBind();
        }
        protected void odsEntradas_Deleted(object sender, Colosoft.WebControls.VirtualObjectDataSourceStatusEventArgs e)
        {
    
        }
    }
}
