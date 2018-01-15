using System;
using System.Web.UI;

namespace Glass.UI.Web.Relatorios.Producao
{
    public partial class ArqOtimiz : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
    
        }
    
        protected void imgPesq_Click(object sender, ImageClickEventArgs e)
        {
            grdArqOtimiz.PageIndex = 0;
        }
    }
}
