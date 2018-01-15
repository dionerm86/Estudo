using System;
using System.Web.UI;

namespace Glass.UI.Web.Relatorios.Administrativos
{
    public partial class ListaContasPagarReceber : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            Ajax.Utility.RegisterTypeForAjax(typeof(MetodosAjax));
        }
        protected void imgPesq_Click(object sender, ImageClickEventArgs e)
        {
    
        }
    }
}
