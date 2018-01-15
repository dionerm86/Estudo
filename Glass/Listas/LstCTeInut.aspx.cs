using System;
using System.Web.UI;

namespace Glass.UI.Web.Listas
{
    public partial class LstCTeInut : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
    
        }
    
        protected void imgPesq_Click(object sender, ImageClickEventArgs e)
        {
            grdCte.PageIndex = 0;
        }
    }
}
