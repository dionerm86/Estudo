using System;
using System.Web.UI;
using System.Web.UI.WebControls;
using Glass.Data.DAL;

namespace Glass.UI.Web.Relatorios
{
    public partial class ListaProdutoInterno : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
    
        }
    
        protected void imgPesq_Click(object sender, ImageClickEventArgs e)
        {
            grdPedidos.DataBind();
        }
    
        protected void grdPedidos_RowCommand(object sender, GridViewCommandEventArgs e)
        {
            if (e.CommandName == "Reabrir")
            {
                try
                {
                    uint idPedidoInterno = Glass.Conversoes.StrParaUint(e.CommandArgument.ToString());
                    PedidoInternoDAO.Instance.Reabrir(idPedidoInterno);
                    Glass.MensagemAlerta.ShowMsg("Pedido reaberto!", Page);
                    grdPedidos.DataBind();
                }
                catch (Exception ex)
                {
                    Glass.MensagemAlerta.ErrorMsg("Falha ao reabrir o pedido interno.", ex, Page);
                }
            }
        }
    }
}
