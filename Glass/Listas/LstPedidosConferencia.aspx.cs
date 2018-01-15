using System;
using System.Web.UI;

namespace Glass.UI.Web.Listas
{
    public partial class LstPedidosConferencia : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            Ajax.Utility.RegisterTypeForAjax(typeof(MetodosAjax));
        }
    
        protected void imgPesq_Click(object sender, ImageClickEventArgs e)
        {
            grdPedidosConferencia.PageIndex = 0;
        }
    
        protected void drpSituacao_SelectedIndexChanged(object sender, EventArgs e)
        {
            grdPedidosConferencia.PageIndex = 0;
        }
    }
}
