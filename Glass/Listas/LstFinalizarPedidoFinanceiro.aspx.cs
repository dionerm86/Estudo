using System;
using System.Web.UI;
using System.Web.UI.WebControls;
using Glass.Configuracoes;
using Glass.Data.DAL;

namespace Glass.UI.Web.Listas
{
    public partial class LstFinalizarPedidoFinanceiro : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            Ajax.Utility.RegisterTypeForAjax(typeof(MetodosAjax));
            Ajax.Utility.RegisterTypeForAjax(typeof(Listas.LstFinalizarPedidoFinanceiro));

            if (!FinanceiroConfig.PermitirFinalizacaoPedidoPeloFinanceiro && !FinanceiroConfig.PermitirConfirmacaoPedidoPeloFinanceiro)
                Response.Redirect("~/WebGlass/Main.aspx", true);
    
            if (!IsPostBack)
            {
                /* Chamado 23075. */
                lblCausasConfirmacaoFinanceiro.Text =
                    string.Format("<b>Confirma��o:</b> {0}{0}{1}{0}{2}{0}{3}",
                    "<br/>",
                    "Pedido com sinal a receber.",
                    "Pedido com pagamento antecipado a receber.",
                    "Limite dispon�vel do cliente menor que o valor do pedido.");

                lblCausasFinalizacaoFinanceiro.Text =
                    string.Format("<b>Finaliza��o:</b> {0}{0}{1}{0}{2}{0}{3}{4}{0}{5}{0}{6}{0}{7}{0}{8}",
                    "<br/>",
                    "Limite dispon�vel do cliente menor que o valor do pedido.",
                    "Saldo da obra menor que o valor do pedido.",
                    (PedidoConfig.NumeroDiasPedidoProntoAtrasado == 0 ? "Pedidos prontos que n�o foram liberados.<br/>" : ""),
                    "Cliente inativo.",
                    "Pedido � prazo com cliente que n�o pode efetuar compras � prazo.",
                    "Cliente bloqueado por contas a receber em atraso.",
                    "Percentual m�nimo de sinal n�o definido.",
                    "Valor do pagamento antecipado somado ao valor de entrada maior que o valor do pedido.");

                drpSituacao.Items.Add(new ListItem("Todas", "0"));
    
                int situacao = (int)Glass.Data.Model.Pedido.SituacaoPedido.AguardandoFinalizacaoFinanceiro;
                drpSituacao.Items.Add(new ListItem(PedidoDAO.Instance.GetSituacaoPedido(situacao), situacao.ToString()));
    
                situacao = (int)Glass.Data.Model.Pedido.SituacaoPedido.AguardandoConfirmacaoFinanceiro;
                drpSituacao.Items.Add(new ListItem(PedidoDAO.Instance.GetSituacaoPedido(situacao), situacao.ToString()));
            }
    
            // Se a coluna de loja n�o estiver sendo exibida, esconde filtro de loja
            if (grdPedidos.Columns[6].Visible == false)
            {
                lblLoja.Style.Add("display", "none");
                drpLoja.Style.Add("display", "none");
                imgPesqLoja.Style.Add("display", "none");
            }
        }
    
        protected void grdPedidos_RowDataBound(object sender, GridViewRowEventArgs e)
        {
            if (e.Row.RowType != DataControlRowType.DataRow || e.Row.DataItem == null)
                return;
    
            var pedido = e.Row.DataItem as WebGlass.Business.Pedido.Entidade.PedidoFinalizarFinanceiro;
    
            foreach (TableCell c in e.Row.Cells)
                c.ForeColor = pedido.CorLinhaLista;
        }
    
        protected void imgPesq_Click(object sender, ImageClickEventArgs e)
        {
            grdPedidos.PageIndex = 0;
        }
    
        protected void lnkPesquisar_Click(object sender, EventArgs e)
        {
    
        }

        #region M�todos AJAX
        /// <summary>
        /// Busca o cliente em tempo real
        /// </summary>
        /// <param name="idCli"></param>
        /// <returns></returns>
        [Ajax.AjaxMethod()]
        public string GetCli(string idCli)
        {
            if (!ClienteDAO.Instance.Exists(Glass.Conversoes.StrParaUint(idCli)))
                return "Erro;Cliente n�o encontrado.";
            else
                return "Ok;" + ClienteDAO.Instance.GetNome(Glass.Conversoes.StrParaUint(idCli));
        }
        #endregion
    }
}
