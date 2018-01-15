using System;
using System.Globalization;
using System.Web.UI.WebControls;
using Glass.Data.Helper;
using Glass.Data.DAL;
using Glass.Configuracoes;

namespace Glass.UI.Web.Cadastros
{
    public partial class CadObra : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (GerarCreditoObra())
            {
                if (!Config.PossuiPermissao(Config.FuncaoMenuFinanceiro.GerarCreditoAvulsoCliente) &&
                    !Config.PossuiPermissao(Config.FuncaoMenuCaixaDiario.GerarCreditoAvulsoCliente))
                    Response.Redirect("~/Listas/LstObra.aspx" + (Request["cxDiario"] == "1" ? "?cxDiario=1" : ""));
    
                Page.Title = "Cadastro de crédito cliente";
            }
            else if (PedidoConfig.DadosPedido.UsarControleNovoObra)
                Response.Redirect("~/Listas/LstObra.aspx" + (Request["cxDiario"] == "1" ? "?cxDiario=1" : ""));
    
            Ajax.Utility.RegisterTypeForAjax(typeof(MetodosAjax));
    
            if (!IsPostBack && Request["idObra"] != null)
                dtvObra.ChangeMode(DetailsViewMode.ReadOnly);
    
            if (dtvObra.CurrentMode == DetailsViewMode.Insert)
                ((HiddenField)dtvObra.FindControl("hdfSituacao")).Value = "1";
        }

        protected bool GerarCreditoObra()
        {
            return GerarCreditoObra(0);
        }

        protected bool GerarCreditoObra(uint idObra)
        {
            if (idObra == 0)
                idObra = Request["idObra"].StrParaUint();

            return (Request["gerarCredito"] == "1" ||
                ObraDAO.Instance.IsGerarCredito(idObra));
        }
    
        protected void drpParcCredito_Load(object sender, EventArgs e)
        {
            ((DropDownList)sender).Visible = FinanceiroConfig.Cartao.PedidoJurosCartao;
        }
    
        protected void odsObra_Inserted(object sender, Colosoft.WebControls.VirtualObjectDataSourceStatusEventArgs e)
        {
            if (e.Exception == null)
            {
                var idObra = e.ReturnValue.ToString().StrParaUint();

                if (!GerarCreditoObra(idObra) && ObraDAO.Instance.IsGerarCredito(idObra))
                {
                    MensagemAlerta.ShowMsg("Faça logout e login no sistema e refaça esta operação.", Page);
                    return;
                }

                if (GerarCreditoObra() || !PedidoConfig.DadosPedido.UsarControleNovoObra)
                    Response.Redirect("~/Cadastros/CadObra.aspx?idObra=" + e.ReturnValue + (GerarCreditoObra() ? "&gerarCredito=1" : "") +
                        (Request["cxDiario"] == "1" ? "&cxDiario=1" : ""));
                else
                    Response.Redirect("~/Listas/LstObra.aspx" + (GerarCreditoObra() ? "?gerarCredito=1" : "") +
                        (Request["cxDiario"] == "1" ? (GerarCreditoObra() ? "&" : "?") + "cxDiario=1" : ""));
            }
            else
            {
                MensagemAlerta.ErrorMsg(string.Format("Erro ao inserir {0}.", DescrTipoObra()),
                    e.Exception.InnerException == null ? e.Exception : e.Exception.InnerException, this);
                e.ExceptionHandled = true;
            }
        }
    
        protected void odsObra_Updated(object sender, Colosoft.WebControls.VirtualObjectDataSourceStatusEventArgs e)
        {
            if (e.Exception == null)
            {
                if (GerarCreditoObra() || !PedidoConfig.DadosPedido.UsarControleNovoObra)
                    Response.Redirect(Request.Url.ToString());
                else
                    Response.Redirect("~/Listas/LstObra.aspx" + (GerarCreditoObra() ? "?gerarCredito=1" : "") +
                        (Request["cxDiario"] == "1" ? (GerarCreditoObra() ? "&" : "?") + "cxDiario=1" : ""));
            }
            else
            {
                Glass.MensagemAlerta.ErrorMsg("Erro ao atualizar " + DescrTipoObra() + ".", e.Exception.InnerException ?? e.Exception, this);
                e.ExceptionHandled = true;
            }
        }
    
        protected void btnCancelar_Click(object sender, EventArgs e)
        {
            Response.Redirect("~/Listas/LstObra.aspx" + (GerarCreditoObra() ? "?gerarCredito=1" : "") +
                (Request["cxDiario"] == "1" ? (GerarCreditoObra() ? "&" : "?") + "cxDiario=1" : ""));
        }
    
        protected void ctrlParcelas1_Load(object sender, EventArgs e)
        {
            Glass.UI.Web.Controls.ctrlParcelas ctrlParcelas = (Glass.UI.Web.Controls.ctrlParcelas)sender;
    
            ctrlParcelas.CampoCalcularParcelas = (HiddenField)dtvObra.FindControl("hdfCalcularParcelas");
            ctrlParcelas.CampoParcelasVisiveis = (DropDownList)dtvObra.FindControl("drpNumParcelas");
            ctrlParcelas.CampoValorTotal = dtvObra.FindControl("hdfTotalObra");
        }
    
        protected void hdfGerarCredito_Load(object sender, EventArgs e)
        {
            if (dtvObra.CurrentMode == DetailsViewMode.Insert)
                ((HiddenField)sender).Value = GerarCreditoObra().ToString();
        }
    
        protected void ctrlFormaPagto1_Load(object sender, EventArgs e)
        {
            Glass.UI.Web.Controls.ctrlFormaPagto ctrlFormaPagto = (Glass.UI.Web.Controls.ctrlFormaPagto)sender;
    
            ctrlFormaPagto.CampoClienteID = dtvObra.FindControl("hdfIdCliente");
            ctrlFormaPagto.CampoCredito = dtvObra.FindControl("hdfCreditoCliente");
            ctrlFormaPagto.CampoValorConta = dtvObra.FindControl("hdfTotalObra");
            
            if (!IsPostBack)
                ctrlFormaPagto.DataRecebimento = DateTime.Now;
        }
    
        protected void grdProdutoObra_DataBound(object sender, EventArgs e)
        {
            GridView grdProdutoObra = (GridView)sender;
            if (grdProdutoObra.Rows.Count != 1)
                return;
            
            uint idObra = Request["idObra"].StrParaUint();
            if (ProdutoObraDAO.Instance.GetCountReal(idObra) == 0)
                grdProdutoObra.Rows[0].Visible = false;
        }
    
        private void ExibirRecebimento(bool exibir)
        {
            var valorObra = ObraDAO.Instance.GetValorObra(null, Request["idObra"].StrParaUint());
            if (exibir && valorObra == 0)
            {
                MensagemAlerta.ShowMsg("O valor do " + DescrTipoObra() + " não pode ser zero.", Page);
                return;
            }
    
            var btnEditar = dtvObra.FindControl("btnEditar") as Button;
            var btnFinalizar = dtvObra.FindControl("btnFinalizar") as Button;
            var btnVoltar = dtvObra.FindControl("btnVoltar") as Button;
            var btnReceber = dtvObra.FindControl("btnReceber") as Button;
            var btnCancelar = dtvObra.FindControl("btnCancelar") as Button;
            var panReceber = dtvObra.FindControl("panReceber") as Panel;

            if (btnEditar != null) btnEditar.Visible = !exibir;
            if (btnFinalizar != null) btnFinalizar.Visible = !exibir;
            if (btnVoltar != null) btnVoltar.Visible = !exibir;
            if (btnReceber != null) btnReceber.Visible = exibir;
            if (btnCancelar != null) btnCancelar.Visible = exibir;
            if (panReceber != null) panReceber.Visible = exibir;

            if (exibir)
                ((HiddenField)dtvObra.FindControl("hdfTotalObra")).Value = valorObra.ToString(CultureInfo.InvariantCulture);
        }
    
        protected void btnFinalizar_Click(object sender, EventArgs e)
        {
            if (UserInfo.GetUserInfo.IsFinanceiroReceb || (Request["cxDiario"] == "1" && UserInfo.GetUserInfo.IsCaixaDiario))
                ExibirRecebimento(true);
            else
            {
                ObraDAO.Instance.FinalizaFuncionario(Request["idObra"].StrParaUint());
                Response.Redirect("~/Listas/LstObra.aspx" + (GerarCreditoObra() ? "?gerarCredito=1" : "") +
                    (Request["cxDiario"] == "1" ? (GerarCreditoObra() ? "&" : "?") + "cxDiario=1" : ""));
            }
        }
    
        protected void btnCancelarReceb_Click(object sender, EventArgs e)
        {
            ExibirRecebimento(false);
        }
    
        protected string DescrTipoObra()
        {
            return GerarCreditoObra() ? "crédito gerado" : "pagamento antecipado";
        }
    
        protected void btnReceber_Click(object sender, EventArgs e)
        {
            try
            {
                FilaOperacoes.ReceberObra.AguardarVez();
                var drpTipoPagto = dtvObra.FindControl("drpTipoPagto") as DropDownList;
                var obra = ObraDAO.Instance.GetElementByPrimaryKey(Request["idObra"].StrParaUint());
                string retorno;

                if (drpTipoPagto != null && drpTipoPagto.SelectedValue == "1")
                {
                    // À vista
                    Controls.ctrlFormaPagto ctrlFormaPagto1 =
                        dtvObra.FindControl("ctrlFormaPagto1") as Controls.ctrlFormaPagto;
                    if (ctrlFormaPagto1 != null)
                    {
                        obra.ValoresPagto = ctrlFormaPagto1.Valores;
                        obra.FormasPagto = ctrlFormaPagto1.FormasPagto;
                        obra.TiposCartaoPagto = ctrlFormaPagto1.TiposCartao;
                        obra.ParcelasCartaoPagto = ctrlFormaPagto1.ParcelasCartao;
                        obra.ContasBancoPagto = ctrlFormaPagto1.ContasBanco;
                        obra.ChequesPagto = ctrlFormaPagto1.ChequesString;
                        obra.CreditoUtilizado = ctrlFormaPagto1.CreditoUtilizado;
                        obra.DataRecebimento = ctrlFormaPagto1.DataRecebimento;
                        obra.DepositoNaoIdentificado = ctrlFormaPagto1.DepositosNaoIdentificados;
                        obra.NumAutCartao = ctrlFormaPagto1.NumAutCartao;
                        obra.CartaoNaoIdentificado = ctrlFormaPagto1.CartoesNaoIdentificados;
                    }

                    retorno = ObraDAO.Instance.PagamentoVista(obra, Request["cxDiario"] == "1", 0, false);
                }
                else
                {
                    // À prazo
                    var drpNumParcelas = dtvObra.FindControl("drpNumParcelas") as DropDownList;
                    if (drpNumParcelas != null) obra.NumParcelas = drpNumParcelas.SelectedValue.StrParaInt();

                    var ctrlParcelas1 =
                        dtvObra.FindControl("ctrlParcelas1") as Controls.ctrlParcelas;
                    if (ctrlParcelas1 != null)
                    {
                        obra.DatasParcelas = ctrlParcelas1.Datas;
                        obra.ValoresParcelas = ctrlParcelas1.Valores;
                    }

                    retorno = ObraDAO.Instance.PagamentoPrazo(obra, Request["cxDiario"] == "1");
                }

                Page.ClientScript.RegisterClientScriptBlock(GetType(), "recebido",
                    "alert('" + retorno + "'); redirectUrl('../Listas/LstObra.aspx" +
                    (GerarCreditoObra() ? "?gerarCredito=1" : "") +
                    (Request["cxDiario"] == "1" ? (GerarCreditoObra() ? "&" : "?") + "cxDiario=1" : "") + "');\n", true);
            }
            catch (Exception ex)
            {
                MensagemAlerta.ErrorMsg("Falha ao receber " + DescrTipoObra() + ".", ex, Page);
            }
            finally
            {
                FilaOperacoes.ReceberObra.ProximoFila();
            }
        }
    
        protected void panReceber_Load(object sender, EventArgs e)
        {
            Panel p = (Panel)sender;
            p.Visible = PedidoConfig.DadosPedido.UsarControleNovoObra;
        }
    
        protected void btnFinalizar_Load(object sender, EventArgs e)
        {
            var finalizar = sender as Button;
            if (finalizar != null)
                finalizar.OnClientClick = "if (!confirm('Deseja finalizar o " + DescrTipoObra() + "?')) return false";
        }
    }
}
