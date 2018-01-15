using System;

namespace Glass.UI.Web.Utils
{
    public partial class SelLoja : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            hdfNfe.Value = Request["Nfe"];
        }
    }
}
