using System;
using System.Web.UI;
using System.Web.UI.WebControls;
using Glass.Configuracoes;

namespace Glass.UI.Web.Relatorios
{
    public partial class ListaPedidosSimples : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
    
        }
        protected void lblSitProd_Load(object sender, EventArgs e)
        {
            // Mostra o link para visualizar as peças deste pedido na produção se a situação não for "-"
            // e se a empresa controla produção
            // TODO: O texto está vindo vazio
            if (((WebControl)sender).ID == "lnkSitProd")
                ((WebControl)sender).Visible = ((LinkButton)sender).Text != "-" && PCPConfig.ControlarProducao;
            else
                ((WebControl)sender).Visible = ((Label)sender).Text == "-" || !PCPConfig.ControlarProducao;
        }
    
        protected void imgPesq_Click(object sender, ImageClickEventArgs e)
        {
            grdPedido.PageIndex = 0;
        }
    
        protected void grdPedido_RowCommand(object sender, GridViewCommandEventArgs e)
        {
            if (e.CommandName == "Producao")
            {
                Response.Redirect("~/Cadastros/Producao/LstProducao.aspx?idPedido=" + e.CommandArgument);
            }
        }
    }
}
