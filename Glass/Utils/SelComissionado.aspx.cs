using System;
using System.Web.UI;

namespace Glass.UI.Web.Utils
{
    public partial class SelComissionado : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
    
        }
    
        protected void imgPesq_Click(object sender, ImageClickEventArgs e)
        {
            grdComissionado.PageIndex = 0;
        }
    }
}
