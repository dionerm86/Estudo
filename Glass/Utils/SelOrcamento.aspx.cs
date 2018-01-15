using System;
using System.Web.UI;

namespace Glass.UI.Web.Utils
{
    public partial class SelOrcamento : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
    
        }
    
        protected void imgPesq_Click(object sender, ImageClickEventArgs e)
        {
            grdOrcamento.PageIndex = 0;
        }
    
        protected void drpSituacao_SelectedIndexChanged(object sender, EventArgs e)
        {
            grdOrcamento.PageIndex = 0;
        }
    }
}
