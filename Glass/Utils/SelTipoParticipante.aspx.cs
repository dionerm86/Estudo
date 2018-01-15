using System;

namespace Glass.UI.Web.Utils
{
    public partial class SelTipoParticipante : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
    
        }
    
        protected void rblTipoParticipante_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (rblTipoParticipante.SelectedItem.Text == "Loja")
            {
                grdLoja.Visible = true;
                grdFornecedor.Visible = false;
                grdCliente.Visible = false;
                grdtransportador.Visible = false;
            }
            else if (rblTipoParticipante.SelectedItem.Text == "Fornecedor")
            {
                grdLoja.Visible = false;
                grdFornecedor.Visible = true;
                grdCliente.Visible = false;
                grdtransportador.Visible = false;
            }
            else if (rblTipoParticipante.SelectedItem.Text == "Cliente")
            {
                grdLoja.Visible = false;
                grdFornecedor.Visible = false;
                grdCliente.Visible = true;
                grdtransportador.Visible = false;
            }
            else if (rblTipoParticipante.SelectedItem.Text == "Transportador")
            {
                grdLoja.Visible = false;
                grdFornecedor.Visible = false;
                grdCliente.Visible = false;
                grdtransportador.Visible = true;
            }
        }
    }
}
