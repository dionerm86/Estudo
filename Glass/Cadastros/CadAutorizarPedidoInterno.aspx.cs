using System;
using System.Web.UI.WebControls;
using Glass.Data.DAL;
using Glass.Data.Helper;

namespace Glass.UI.Web.Cadastros
{
    public partial class CadAutorizarPedidoInterno : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!Config.PossuiPermissao(Config.FuncaoMenuEstoque.AutorizarPedidoInterno))
                Response.Redirect("~/Listas/LstPedidoInterno.aspx", true);

            dtvPedidoInterno.Visible = selPedidoInterno.Valor != "";

            if (!Configuracoes.FiscalConfig.UsarControleCentroCusto)
                dtvPedidoInterno.Rows[3].Visible = false;
        }

        protected void btnAutorizar_Click(object sender, EventArgs e)
        {
            try
            {
                var idCentroCusto =
                    ((DropDownList) dtvPedidoInterno.Rows[3].FindControl("ddlCentroCusto")).SelectedValue.StrParaInt();

                PedidoInternoDAO.Instance.AutorizarPedidoInterno(Glass.Conversoes.StrParaUint(selPedidoInterno.Valor),
                    idCentroCusto);
                Response.Redirect("~/Listas/LstPedidoInterno.aspx");
            }
            catch (Exception ex)
            {
                Glass.MensagemAlerta.ErrorMsg("Falha ao autorizar pedido interno.", ex, Page);
            }
        }

        protected void ddlCentroCusto_DataBound(object sender, EventArgs e)
        {
            if (!Configuracoes.FiscalConfig.UsarControleCentroCusto &&
                dtvPedidoInterno.Rows.Count > 0)
                dtvPedidoInterno.Rows[3].Visible = false;
        }
    }
}