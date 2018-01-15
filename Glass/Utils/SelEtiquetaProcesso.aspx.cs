using System;
using System.Web.UI;

namespace Glass.UI.Web.Utils
{
    public partial class SelEtiquetaProcesso : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            Ajax.Utility.RegisterTypeForAjax(typeof(MetodosAjax));

        }
    
        protected void imgPesq_Click(object sender, ImageClickEventArgs e)
        {
            grdProcesso.DataBind();
        }
    }
}
