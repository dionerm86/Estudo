using System;
using System.Web.UI.WebControls;
using Glass.Data.Model;

namespace Glass.UI.Web.Utils
{
    public partial class SelPedidoEspelhoImprimir : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            
        }
    
        protected void chkImprimir_DataBinding(object sender, EventArgs e)
        {
            GridViewRow linha = ((CheckBox)sender).Parent.Parent as GridViewRow;
            if (linha == null)
                return;
    
            PedidoEspelho pe = linha.DataItem as PedidoEspelho;
            if (pe == null)
                return;
    
            ((CheckBox)sender).Attributes.Add("IdPedido", pe.IdPedido.ToString());
        }
    }
}
