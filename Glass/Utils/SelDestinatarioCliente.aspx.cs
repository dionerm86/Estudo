using System;
using System.Web.UI;

namespace Glass.UI.Web.Utils
{
    public partial class SelDestinatarioCliente : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
    
        }
    
        protected void imgPesq_Click(object sender, ImageClickEventArgs e)
        {
            this.grdCliente.PageIndex = 0;
        }
    }
}
