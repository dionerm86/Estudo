using System;
using System.Web.UI;
using System.Web.UI.WebControls;
using Glass.Data.DAL;
using Glass.Data.Model;
using Glass.Data.Helper;
using System.Drawing;
using Glass.Configuracoes;

namespace Glass.UI.Web.Listas
{
    public partial class LstPedidos : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            Ajax.Utility.RegisterTypeForAjax(typeof(Glass.UI.Web.Listas.LstPedidos));
    
            if (Request["maoObra"] == "1")
                Page.Title = "Pedidos Mão de Obra";
            else if (Request["producao"] == "1")
                Page.Title = "Pedidos para Produção";
            else
                Page.Title = "Pedidos";
    
            if (Request["byVend"] == "1")
                Page.Title = "Meus " + Page.Title;
    
            if (!IsPostBack)
            {
                // Esconde a coluna de loja se a empresa possuir apenas uma
                grdPedido.Columns[6].Visible = LojaDAO.Instance.GetCount() > 1;

                // Esconde data de confirmação
                grdPedido.Columns[13].Visible = !PedidoConfig.LiberarPedido;

                // Esconde data pedido pronto
                grdPedido.Columns[14].Visible = PCPConfig.ControlarProducao && PedidoConfig.NumeroDiasPedidoProntoAtrasado > 0;

                // Esconde data de liberação
                grdPedido.Columns[15].Visible = PedidoConfig.LiberarPedido;

                // Recupera o tipo de usuário
                uint tipoUsuario = UserInfo.GetUserInfo.TipoUsuario;

                if (PedidoConfig.DadosPedido.ListaApenasPedidosVendedor &&
                    tipoUsuario == (uint)Data.Helper.Utils.TipoFuncionario.Vendedor &&
                    Request["byVend"] != "1")
                {
                    var requestQuery = Request.Url.ToString().Contains("?") ?
                        Request.Url.ToString().Substring(Request.Url.ToString().IndexOf('?')) :
                        String.Empty;

                    requestQuery += String.IsNullOrEmpty(requestQuery) ?
                        "?byVend=1" :
                        (requestQuery.Contains("byVend") ?
                            requestQuery.Replace("byVend=" + Request["byVend"], "byVend=1") :
                            "&byVend=1");

                    Response.Redirect("LstPedidos.aspx" + requestQuery);
                }
    
                // Apenas Gerentes, Vendedores e Auxiliares administrativos podem inserir pedidos
                lnkInserir.Visible = Config.PossuiPermissao(Config.FuncaoMenuPedido.EmitirPedido);
    
                // Esconde coluna situação da produção
                if (!PCPConfig.ControlarProducao)
                    grdPedido.Columns[17].Visible = false;
    
                if (!FinanceiroConfig.UsarControleLiberarFinanc || Request["financ"] != "1")
                {
                    grdPedido.Columns[19].Visible = false; // Esconde a coluna "Liberar p/ Entrega"
                }

                // Se a coluna de loja não estiver sendo exibida, esconde filtro de loja
                if (grdPedido.Columns[6].Visible == false)
                {
                    lblLoja.Style.Add("display", "none");
                    drpLoja.Style.Add("display", "none");
                    imgPesqLoja.Style.Add("display", "none");
                }

                lblValorAte.Visible = lblValorDe.Visible = txtValorDe.Visible = txtValorAte.Visible = imgPesqValor.Visible = false;

                if (PedidoConfig.TelaListagem.ApenasAdminVisualizaTotais && !UserInfo.GetUserInfo.IsAdministrador)
                {
                    lnkTotais.Visible = false;
                    lnkGraficoDiario.Visible = false;
                }
            }
        }
    
        protected bool SugestoesVisible()
        {
            return Config.PossuiPermissao(Config.FuncaoMenuCadastro.CadastrarSugestoesClientes);
        }

        protected void lnkInserir_Click(object sender, EventArgs e)
        {
            string tipoPedido = !String.IsNullOrEmpty(Request["maoObra"]) ? "&maoObra=" + Request["maoObra"] :
                !String.IsNullOrEmpty(Request["maoObraEspecial"]) ? "&maoObraEspecial=" + Request["maoObraEspecial"] :
                !String.IsNullOrEmpty(Request["producao"]) ? "&producao=" + Request["producao"] : String.Empty;
    
            Response.Redirect("../Cadastros/CadPedido.aspx?ByVend=" + Request["ByVend"] + tipoPedido);
        }
    
        protected void odsPedido_Deleted(object sender, Colosoft.WebControls.VirtualObjectDataSourceStatusEventArgs e)
        {
            if (e.Exception != null)
            {
                Glass.MensagemAlerta.ErrorMsg(null, e.Exception, Page);
                e.ExceptionHandled = true;
            }
        }
    
        protected void imgPesq_Click(object sender, ImageClickEventArgs e)
        {
            grdPedido.PageIndex = 0;
        }
    
        protected void lnkPesquisar_Click(object sender, EventArgs e)
        {
            grdPedido.PageIndex = 0;
        }
    
        #region Métodos AJAX
    
        /// <summary>
        /// Busca o cliente em tempo real
        /// </summary>
        /// <param name="idCli"></param>
        /// <returns></returns>
        [Ajax.AjaxMethod()]
        public string GetCli(string idCli)
        {
            if (String.IsNullOrEmpty(idCli) || !ClienteDAO.Instance.Exists(Glass.Conversoes.StrParaUint(idCli)))
                return "Erro;Cliente não encontrado.";
            else
                return "Ok;" + ClienteDAO.Instance.GetNome(Glass.Conversoes.StrParaUint(idCli));
        }
    
        #endregion
        
        protected void lnkEditar_DataBinding(object sender, EventArgs e)
        {
            HyperLink link = ((HyperLink)sender);
            GridViewRow linha = link.Parent.Parent as GridViewRow;
            Glass.Data.Model.Pedido item = linha != null ? linha.DataItem as Glass.Data.Model.Pedido : null;
    
            if (item != null && item.UsarControleReposicao)
                link.NavigateUrl = "../Cadastros/CadPedidoReposicao.aspx?idPedido=" + item.IdPedido + "&ByVend=" + Request["ByVend"];
        }
    
        protected void grdPedido_RowCommand(object sender, GridViewCommandEventArgs e)
        {
            if (e.CommandName == "Reabrir")
            {
                uint idPedido = Glass.Conversoes.StrParaUint(e.CommandArgument.ToString());
    
                try { PedidoDAO.Instance.Reabrir(idPedido); }
                catch (Exception ex) { Glass.MensagemAlerta.ErrorMsg("", ex, Page); }
    
                grdPedido.DataBind();
            }
            else if (e.CommandName == "Producao")
            {
                Response.Redirect("~/Cadastros/Producao/LstProducao.aspx?idPedido=" + e.CommandArgument);
            }
            else if (e.CommandName == "Liberar")
            {
                string[] argument = e.CommandArgument.ToString().Split('#');
                
                uint idPedido = Glass.Conversoes.StrParaUint(argument[0]);
                bool liberar = bool.Parse(argument[1]);
    
                try
                {
                    PedidoDAO.Instance.AlteraLiberarFinanc(idPedido, liberar);
                }
                catch (Exception ex) { Glass.MensagemAlerta.ErrorMsg("", ex, Page); }
    
                grdPedido.DataBind();
            }
        }
    
        protected void grdPedido_RowDataBound(object sender, GridViewRowEventArgs e)
        {
            if (e.Row.RowType != DataControlRowType.DataRow)
                return;

            Glass.Data.Model.Pedido item = e.Row.DataItem as Glass.Data.Model.Pedido;
            if (item == null)
                return;
    
            if (item.CorLinhaLista != Color.Black)
                foreach (TableCell c in e.Row.Cells)
                {
                    c.ForeColor = item.CorLinhaLista;
    
                    foreach (Control c1 in c.Controls)
                        if (c1 is WebControl)
                            ((WebControl)c1).ForeColor = c.ForeColor;
                }
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
    
        protected void grdObservacaoFinanceiro_RowDataBound(object sender, GridViewRowEventArgs e)
        {
            if (e.Row.RowType != DataControlRowType.DataRow || e.Row.DataItem == null)
                return;
    
            var item = e.Row.DataItem as WebGlass.Business.Pedido.Entidade.MotivoFinalizacaoFinanceiro;
    
            if (item.CorLinhaLista != null)
                foreach (TableCell c in e.Row.Cells)
                    c.ForeColor = item.CorLinhaLista.Value;
        }
    
        protected string EstiloFiltroPronto()
        {
            return PedidoConfig.LiberarPedido && 
                PedidoConfig.NumeroDiasPedidoProntoAtrasado > 0 ? "" : "display: none";
        }
    
        protected bool UsarImpressaoProjetoPcp()
        {
            return Glass.Configuracoes.PedidoConfig.TelaListagem.UsarImpressaoProjetoPcp;
        }
    }
}
