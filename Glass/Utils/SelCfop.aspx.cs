using System;
using System.Web.UI;

namespace Glass.UI.Web.Utils
{
    public partial class SelCfop : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            hdfNfe.Value = Request["Nfe"];
        }
    
        protected void imgPesq_Click(object sender, ImageClickEventArgs e)
        {
            grdCfop.PageIndex = 0;
        }
    }
}
