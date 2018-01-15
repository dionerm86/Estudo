using System;
using System.Web.UI;

namespace Glass.UI.Web.Cadastros
{
    public partial class CadChequePagtoTerc : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
    
        }
    
        protected void imgPesq_Click(object sender, ImageClickEventArgs e)
        {
            grdCheque.PageIndex = 0;
        }
    }
}
