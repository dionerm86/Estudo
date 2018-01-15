using System;
using System.Web.UI;

namespace Glass.UI.Web.Utils
{
    public partial class SelProdProj : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
    
        }
    
        protected void imgPesq_Click(object sender, ImageClickEventArgs e)
        {
            grdProdProj.PageIndex = 0;
        }
    
        protected void drpTipo_SelectedIndexChanged(object sender, EventArgs e)
        {
            grdProdProj.PageIndex = 0;
        }
    }
}
