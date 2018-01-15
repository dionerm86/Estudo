using System;
using System.Web.UI;

namespace Glass.UI.Web.Listas
{
    public partial class LstChequesPagto : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            Ajax.Utility.RegisterTypeForAjax(typeof(MetodosAjax));
        }
    
        protected void drpContaBanco_SelectedIndexChanged(object sender, EventArgs e)
        {
            grdCheque.PageIndex = 0;
        }
    
        protected void imgPesq_Click(object sender, ImageClickEventArgs e)
        {
            grdCheque.PageIndex = 0;
        }
    }
}
