using System;
using System.Web.UI;
using System.Web.UI.WebControls;
using Glass.Configuracoes;

namespace Glass.UI.Web.Listas
{
    public partial class LstAcerto : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            Ajax.Utility.RegisterTypeForAjax(typeof(MetodosAjax));
    
            if (!PedidoConfig.LiberarPedido)
            {
                Label2.Visible = false;
                txtNumLiberarPedido.Visible = false;
                ImageButton2.Visible = false;
            }
        }
    
        protected void imgPesq_Click(object sender, ImageClickEventArgs e)
        {
            grdAcerto.PageIndex = 0;
        }
    
        protected void drpFormaPagto_SelectedIndexChanged(object sender, EventArgs e)
        {
            drpTipoBoleto.Visible = drpFormaPagto.SelectedValue == ((uint)Glass.Data.Model.Pagto.FormaPagto.Boleto).ToString();
        }
    
        protected void grdAcerto_PreRender(object sender, EventArgs e)
        {
            foreach (GridViewRow linha in grdAcerto.Rows)
            {
                if (((HiddenField)linha.Cells[0].FindControl("hdfRenegociacao")).Value == "True")
                {
                    foreach (TableCell celula in linha.Cells)
                        celula.ForeColor = System.Drawing.Color.Blue;
                }
            }
        }
    }
}
