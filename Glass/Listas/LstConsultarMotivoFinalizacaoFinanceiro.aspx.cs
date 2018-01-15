using System;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace Glass.UI.Web.Listas
{
    public partial class LstConsultarMotivoFinalizacaoFinanceiro : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            Ajax.Utility.RegisterTypeForAjax(typeof(MetodosAjax));
        }
    
        protected void grdObservacaoFinanceiro_RowDataBound(object sender, GridViewRowEventArgs e)
        {
            if (e.Row.RowType != DataControlRowType.DataRow || e.Row.DataItem == null)
                return;
    
            var item = e.Row.DataItem as WebGlass.Business.Pedido.Entidade.MotivoFinalizacaoFinanceiro;
    
            if (item.CorLinhaLista != null)
                foreach (TableCell c in e.Row.Cells)
                    c.ForeColor = item.CorLinhaLista.Value;
        }
    
        protected void imgPesq_Click(object sender, ImageClickEventArgs e)
        {
    
        }
    }
}
