using Glass.Data.DAL;
using System;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace Glass.UI.Web.Listas
{
    public partial class LstPedidosIgnoradosComissao : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {

        }

        protected override void OnInit(EventArgs e)
        {
            base.OnInit(e);
            grdPedidosIgnorados.Register(true, true);
            odsPedidosIgnorados.Register();
        }

        protected void grdPedidosIgnorados_DataBound(object sender, EventArgs e)
        {
            var grdPedidos = (GridView)sender;
            if (grdPedidos.Rows.Count != 1)
                return;

            if (PedidoDAO.Instance.ObterPedidosIgnorarComissaoCountReal(0, null) == 0)
                grdPedidos.Rows[0].Visible = false;
        }

        protected void grdPedidosIgnorados_RowCommand(object sender, GridViewCommandEventArgs e)
        {
            if(e.CommandName == "RemoverIgnorar")
            {
                try
                {
                    var idPedido = e.CommandArgument.ToString().StrParaUint();

                    PedidoDAO.Instance.IgnorarComissaoPedido(idPedido, null, false);
                    grdPedidosIgnorados.DataBind();

                }
                catch (Exception ex)
                {
                    MensagemAlerta.ErrorMsg("Falha ao ignorar pedido para geração de comissão.", ex, Page);
                }
            }
        }

        protected void lnkInserir_Click(object sender, EventArgs e)
        {
            try
            {
                var idPedido = ((TextBox)grdPedidosIgnorados.FooterRow.FindControl("txtIdPedido")).Text.StrParaUint();
                var motivo = ((TextBox)grdPedidosIgnorados.FooterRow.FindControl("txtMotivo")).Text;

                PedidoDAO.Instance.IgnorarComissaoPedido(idPedido, motivo, true);
                grdPedidosIgnorados.DataBind();

            }
            catch (Exception ex)
            {
                MensagemAlerta.ErrorMsg("Falha ao ignorar pedido para geração de comissão.", ex, Page);
            }
        }

        protected void imgPesq_Click(object sender, ImageClickEventArgs e)
        {
            grdPedidosIgnorados.PageIndex = 0;
        }
    }
}