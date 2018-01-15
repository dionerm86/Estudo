using System;
using System.Web.UI;

namespace Glass.UI.Web.Listas
{
    public partial class LstDeposito : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
    
        }
    
        protected void imgPesq_Click(object sender, ImageClickEventArgs e)
        {
            grdDepositos.PageIndex = 0;
        }
    }
}
