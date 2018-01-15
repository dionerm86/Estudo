using System;
using System.Web.UI;

namespace Glass.UI.Web.Utils
{
    public partial class SelCidade : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
    
        }
    
        protected void lnkPesq_Click(object sender, ImageClickEventArgs e)
        {
            grdCidade.PageIndex = 0;
        }
    }
}
