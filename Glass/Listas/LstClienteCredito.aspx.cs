using System;
using System.Web.UI;

namespace Glass.UI.Web.Listas
{
    public partial class LstClienteCredito : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            Ajax.Utility.RegisterTypeForAjax(typeof(MetodosAjax));
        }
    
        protected void imgPesq_Click(object sender, ImageClickEventArgs e)
        {
            grdCliente.PageIndex = 0;
        }
    }
}
