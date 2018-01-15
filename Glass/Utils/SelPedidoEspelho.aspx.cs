using System;
using System.Web.UI;

namespace Glass.UI.Web.Utils
{
    public partial class SelPedidoEspelho : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            Ajax.Utility.RegisterTypeForAjax(typeof(MetodosAjax));
            hdfFinalizados.Value = (Request["finalizados"] == "1").ToString();
        }
        
        protected void imgPesq_Click(object sender, ImageClickEventArgs e)
        {
            grdPedido.PageIndex = 0;
        }
    
        protected void drpSituacao_SelectedIndexChanged(object sender, EventArgs e)
        {
            grdPedido.PageIndex = 0;
        }
    }
}
