using System;
using System.Web.UI;

namespace Glass.UI.Web.Relatorios.Administrativos
{
    public partial class PedidosConferidos : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
    
        }
    
        protected void imgPesq_Click(object sender, ImageClickEventArgs e)
        {
            grdPedidoConferido.PageIndex = 0;
        }
    }
}
