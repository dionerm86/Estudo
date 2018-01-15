using System;
using System.Web.UI;
using System.Web.UI.WebControls;
using Glass.Data.DAL;
using Glass.Configuracoes;

namespace Glass.UI.Web.Cadastros
{
    public partial class CadDescontoParcela : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            Ajax.Utility.RegisterTypeForAjax(typeof(Glass.UI.Web.Cadastros.CadDescontoParcela));
    
            if (!IsPostBack)
            {
                if (PedidoConfig.LiberarPedido)
                {
                    grdConta.Columns[3].Visible = false;
                    lblNumPedidoLiberacao.Text = "Número da Liberação: ";
                    btnBuscarPedido.Text = "Buscar Liberação";
                    rblTipo.Items[0].Text = "Liberação";
                    imbPesq.Visible = false;
                }
                else
                {
                    grdConta.Columns[4].Visible = false;
                    lblNumPedidoLiberacao.Text = "Número do Pedido: ";
                    btnBuscarPedido.Text = "Buscar Pedido";
                    rblTipo.Items[0].Text = "Pedido";
                }
    
                if (OrigemTrocaDescontoDAO.Instance.GetList().Length == 0)
                {
                    lblOrigem.Style.Add("display","none");
                    ddlOrigem.Style.Add("display", "none");
                }
            }
        }
    
        protected void btnBuscarPedido_Click(object sender, EventArgs e)
        {
            if (txtNumPedidoLiberacao.Text == String.Empty)
            {
                tbDesconto.Visible = false;
                return;
            }
    
            try
            {
                uint idPedidoOuLiberarPedido = Glass.Conversoes.StrParaUint(txtNumPedidoLiberacao.Text);
                WebGlass.Business.ContasReceber.Fluxo.DescontoAcrescimo.Instance.Validar(idPedidoOuLiberarPedido);
                tbDesconto.Visible = true;
    
                grdConta.DataBind();
                grdConta.Columns[0].Visible = true;
            }
            catch (Exception ex)
            {
                Glass.MensagemAlerta.ErrorMsg("Falha ao buscar " + (PedidoConfig.LiberarPedido ? "liberação de pedido." : "pedido."), ex, Page);
                tbDesconto.Visible = false;
            }
        }
    
        [Ajax.AjaxMethod()]
        public string AplicarDescontoAcrescimo(string idContaR, string valorString, string descontoString, string acrescimoString, string origem, string motivo)
        {
            return WebGlass.Business.ContasReceber.Fluxo.DescontoAcrescimo.Ajax.AplicarDescontoAcrescimoParcela(idContaR,
                valorString, descontoString, acrescimoString, origem, motivo);
        }
    
        protected void btnBuscarConta_Click(object sender, EventArgs e)
        {
            grdConta.DataBind();
            grdConta.Columns[0].Visible = false;
            tbDesconto.Visible = grdConta.Rows.Count > 0;
    
            string script = ((ImageButton)grdConta.Rows[0].Cells[0].FindControl("imgOk")).OnClientClick.Replace("return false", "");
            Page.ClientScript.RegisterStartupScript(GetType(), "selecionarConta", script + "\n", true);
        }
    }
}
