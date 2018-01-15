using System;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace Glass.UI.Web.Relatorios
{
    public partial class ListaVendasAVista : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            hdfFormaPagtoCartao.Value = ((uint)Glass.Data.Model.Pagto.FormaPagto.Cartao).ToString();
    
            if (!IsPostBack)
            {
                ((TextBox)ctrlDataIni.FindControl("txtData")).Text = DateTime.Now.ToString("01/MM/yyyy");
                ((TextBox)ctrlDataFim.FindControl("txtData")).Text = DateTime.Now.ToString("dd/MM/yyyy");
            }
        }
    
        protected void imgPesq_Click(object sender, ImageClickEventArgs e)
        {
            grdVendasAVista.PageIndex = 0;
        }
    
        protected void drpFormaPagto_SelectedIndexChanged(object sender, EventArgs e)
        {
            drpTipoCartao.Visible = drpFormaPagto.SelectedValue == ((uint)Glass.Data.Model.Pagto.FormaPagto.Cartao).ToString();
        }
    }
}
