using System;

namespace Glass.UI.Web.Utils
{
    public partial class SelRetalho : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            Ajax.Utility.RegisterTypeForAjax(typeof(MetodosAjax));
        }
        protected void lnkPesq_Click(object sender, EventArgs e)
        {
        }
    }
}
