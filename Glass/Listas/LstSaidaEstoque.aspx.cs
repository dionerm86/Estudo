using System;
using System.Web.UI;
using Glass.Configuracoes;

namespace Glass.UI.Web.Listas
{
    public partial class LstSaidaEstoque : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                Label4.Visible = PedidoConfig.LiberarPedido;
                txtNumLiberacao.Visible = PedidoConfig.LiberarPedido;
                imgPesqLiberacao.Visible = PedidoConfig.LiberarPedido;
    
                lblVolume.Visible = OrdemCargaConfig.UsarControleOrdemCarga;
                txtNumVolume.Visible = OrdemCargaConfig.UsarControleOrdemCarga;
                imbPesqVolume.Visible = OrdemCargaConfig.UsarControleOrdemCarga;
            }
        }
    
        protected void imgPesq_Click(object sender, ImageClickEventArgs e)
        {
            grdSaidas.PageIndex = 0;
            grdSaidas.DataBind();
        }
    }
}
