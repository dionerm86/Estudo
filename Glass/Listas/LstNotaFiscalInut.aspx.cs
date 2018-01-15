using System;
using System.Web.UI;

namespace Glass.UI.Web.Listas
{
    public partial class LstNotaFiscalInut : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
    
        }
    
        protected void imgPesq_Click(object sender, ImageClickEventArgs e)
        {
            grdNf.PageIndex = 0;
        }
    }
}
