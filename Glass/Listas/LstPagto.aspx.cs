using System;
using System.Web.UI;

namespace Glass.UI.Web.Listas
{
    public partial class LstPagto : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            Ajax.Utility.RegisterTypeForAjax(typeof(MetodosAjax));
        }
    
        protected void imgPesq_Click(object sender, ImageClickEventArgs e)
        {
            grdPagto.PageIndex = 0;
        }
    }
}
