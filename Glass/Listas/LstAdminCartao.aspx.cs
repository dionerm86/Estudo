using System;
using System.Web.UI.WebControls;

namespace Glass.UI.Web.Listas
{
    public partial class LstAdminCartao : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
    
        }
    
        protected void grdAdminCartao_RowCommand(object sender, GridViewCommandEventArgs e)
        {
            grdAdminCartao.ShowFooter = e.CommandName != "Edit";
        }
    }
}
