using System;
using System.Web.UI;

namespace Glass.UI.Web.Utils
{
    public partial class SelPlanoConta : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
    
        }
    
        protected void imgPesq_Click(object sender, ImageClickEventArgs e)
        {
            grdCfop.PageIndex = 0;
        }
    
        protected void drp_SelectedIndexChanged(object sender, EventArgs e)
        {
            grdCfop.PageIndex = 0;
        }
    }
}
