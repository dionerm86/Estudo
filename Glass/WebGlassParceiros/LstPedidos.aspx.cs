using System;
using System.Web.UI;
using System.Web.UI.WebControls;
using Glass.Data.Model;
using Glass.Data.DAL;
using System.Drawing;
using Glass.Configuracoes;

namespace Glass.UI.Web.WebGlassParceiros
{
    public partial class LstPedidos : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
    
        }
    
        protected void imgPesq_Click(object sender, ImageClickEventArgs e)
        {
            grdPedido.DataBind();
        }
    
        protected void grdPedido_RowDataBound(object sender, GridViewRowEventArgs e)
        {
            if (e.Row.RowType != DataControlRowType.DataRow)
                return;

            Glass.Data.Model.Pedido item = e.Row.DataItem as Glass.Data.Model.Pedido;
            if (item == null)
                return;
    
            if (item.IdPedido == 0)
            {
                foreach (TableCell c in e.Row.Cells)
                    c.ForeColor = System.Drawing.Color.Red;
    
                e.Row.Cells[7].Text = "Aberto";
            }
    
            if (item.CorLinhaLista != Color.Black)
                foreach (TableCell c in e.Row.Cells)
                {
                    c.ForeColor = item.CorLinhaLista;
    
                    // Alteração no pcp não deve ser vista pelo cliente
                    if (c.ForeColor == Color.Red)
                        continue;
    
                    foreach (Control c1 in c.Controls)
                        if (c1 is WebControl)
                            ((WebControl)c1).ForeColor = c.ForeColor;
                }
        }
    
        protected void grdPedido_RowCommand(object sender, GridViewCommandEventArgs e)
        {
            if (e.CommandName == "Reabrir")
            {
                uint idPedido = Glass.Conversoes.StrParaUint(e.CommandArgument.ToString());
                var pedido = PedidoDAO.Instance.GetElementByPrimaryKey(idPedido);
                if (!pedido.ExibirReabrir)
                {
                    Glass.MensagemAlerta.ShowMsg("Não é possível reabrir esse pedido.", Page);
                    grdPedido.DataBind();
                    return;
                }

                PedidoDAO.Instance.Reabrir(pedido.IdPedido);

                grdPedido.DataBind();
            }
        }
    
        protected bool UsarImpressaoProjetoPcp()
        {
            return PedidoConfig.TelaListagem.UsarImpressaoProjetoPcp;
        }
    }
}
