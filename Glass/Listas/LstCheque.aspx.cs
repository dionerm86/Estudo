using Glass.Configuracoes;
using System;

namespace Glass.UI.Web.Listas
{
    public partial class LstCheque : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!PedidoConfig.LiberarPedido)
            {
                lblLiberacao.Visible = false;
                imgPesqLib.Visible = false;
                txtNumLiberarPedido.Visible = false;
            }
        }
    }
}
