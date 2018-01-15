using System;
using System.Globalization;
using System.Web.UI;
using System.Web.UI.WebControls;
using Glass.Data.Helper;
using Glass.Data.Model;
using Glass.Data.DAL;
using Glass.Configuracoes;

namespace Glass.UI.Web.Cadastros
{
    public partial class CadObraNovo : Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!PedidoConfig.DadosPedido.UsarControleNovoObra)
                Response.Redirect("~/Listas/LstObra.aspx" + (Request["cxDiario"] == "1" ? "?cxDiario=1" : ""));
    
            Ajax.Utility.RegisterTypeForAjax(typeof(MetodosAjax));
            Ajax.Utility.RegisterTypeForAjax(typeof(CadObraNovo));
    
            if (!IsPostBack && Request["idObra"] != null)
                dtvObra.ChangeMode(DetailsViewMode.ReadOnly);
    
            if (dtvObra.CurrentMode == DetailsViewMode.Insert)
            {
                ((HiddenField)dtvObra.FindControl("hdfIdFunc")).Value = UserInfo.GetUserInfo.CodUser.ToString();
                ((HiddenField)dtvObra.FindControl("hdfSituacao")).Value = "1";
            }
        }
    
        [Ajax.AjaxMethod]
        public string GetProd(string codInterno, string idClienteStr)
        {
            return WebGlass.Business.Produto.Fluxo.BuscarEValidar.Ajax.GetProdObra(codInterno, idClienteStr);
        }
    
        [Ajax.AjaxMethod]
        public string IsVidro(string codInterno)
        {
            return ProdutoDAO.Instance.IsVidro(codInterno).ToString().ToLower();
        }
    
        [Ajax.AjaxMethod]
        public string ProdutoJaExiste(string idObra, string codInterno)
        {
            return (ProdutoObraDAO.Instance.GetByCodInterno(idObra.StrParaUint(), codInterno) != null).ToString().ToLower();
        }
    
        protected void drpParcCredito_Load(object sender, EventArgs e)
        {
            ((DropDownList)sender).Visible = FinanceiroConfig.Cartao.PedidoJurosCartao;
        }
    
        protected void odsObra_Inserted(object sender, Colosoft.WebControls.VirtualObjectDataSourceStatusEventArgs e)
        {
            if (e.Exception == null)
            {
                if (PedidoConfig.DadosPedido.UsarControleNovoObra)
                    Response.Redirect("~/Cadastros/CadObraNovo.aspx?idObra=" + e.ReturnValue +
                        (Request["cxDiario"] == "1" ? "&cxDiario=1" : ""));
                else
                    Response.Redirect("~/Listas/LstObra.aspx" + (Request["cxDiario"] == "1" ? "?cxDiario=1" : ""));
            }
            else
            {
                //Se existir InnerException, exibe a mensagem dele, se não existir, exibe a mensagem de Exception
                Exception ex = e.Exception.InnerException == null ? e.Exception : e.Exception.InnerException;

                MensagemAlerta.ErrorMsg("Erro ao inserir pagamento antecipado.", ex, this);
                e.ExceptionHandled = true;
            }
        }
    
        protected void odsObra_Updated(object sender, Colosoft.WebControls.VirtualObjectDataSourceStatusEventArgs e)
        {
            if (e.Exception == null)
            {
                if (PedidoConfig.DadosPedido.UsarControleNovoObra)
                    Response.Redirect(Request.Url.ToString());
                else
                    Response.Redirect("~/Listas/LstObra.aspx" + (Request["cxDiario"] == "1" ? "?cxDiario=1" : ""));
            }
            else
            {
                //Se existir InnerException, exibe a mensagem dele, se não existir, exibe a mensagem de Exception
                Exception ex = e.Exception.InnerException == null ? e.Exception : e.Exception.InnerException;

                MensagemAlerta.ErrorMsg("Erro ao atualizar pagamento antecipado.", ex, this);
                e.ExceptionHandled = true;
            }
        }
    
        protected void btnCancelar_Click(object sender, EventArgs e)
        {
            Response.Redirect("~/Listas/LstObra.aspx" + (Request["cxDiario"] == "1" ? "?cxDiario=1" : ""));
        }
    
        protected void ctrlParcelas1_Load(object sender, EventArgs e)
        {
            Controls.ctrlParcelas ctrlParcelas = (Controls.ctrlParcelas)sender;
    
            ctrlParcelas.CampoCalcularParcelas = (HiddenField)dtvObra.FindControl("hdfCalcularParcelas");
            ctrlParcelas.CampoParcelasVisiveis = (DropDownList)dtvObra.FindControl("drpNumParcelas");
            ctrlParcelas.CampoValorTotal = dtvObra.FindControl("hdfTotalObra");
        }
    
        protected void ctrlFormaPagto1_Load(object sender, EventArgs e)
        {
            Controls.ctrlFormaPagto ctrlFormaPagto = (Controls.ctrlFormaPagto)sender;
    
            ctrlFormaPagto.CampoClienteID = dtvObra.FindControl("lblIdCliente");
            ctrlFormaPagto.CampoCredito = dtvObra.FindControl("hdfCreditoCliente");
            ctrlFormaPagto.CampoValorConta = dtvObra.FindControl("hdfTotalObra");
        }
    
        protected void grdProdutoObra_DataBound(object sender, EventArgs e)
        {
            var produtoObra = (GridView)sender;
            if (produtoObra.Rows.Count != 1)
                return;
    
            var idObra = Request["idObra"].StrParaUint();
            if (ProdutoObraDAO.Instance.GetCountReal(idObra) == 0)
                produtoObra.Rows[0].Visible = false;
        }
    
        protected void imbAdd_Click(object sender, ImageClickEventArgs e)
        {
            var grdProdutos = dtvObra.FindControl("grdProdutoObra") as GridView;
            if (grdProdutos != null)
            {
                var idProd = ((HiddenField)grdProdutos.FooterRow.FindControl("hdfIdProd")).Value;
                var valorUnit = ((TextBox)grdProdutos.FooterRow.FindControl("txtValorUnit")).Text;
                var tamanhoMax = ((TextBox)grdProdutos.FooterRow.FindControl("txtTamanhoMax")).Text;

                var novo = new ProdutoObra
                {
                    IdObra = Request["idObra"].StrParaUint(),
                    IdProd = !string.IsNullOrEmpty(idProd) ? idProd.StrParaUint() : 0,
                    ValorUnitario = valorUnit.StrParaDecimal(),
                    TamanhoMaximo = tamanhoMax.StrParaFloat()
                };

                ProdutoObraDAO.Instance.Insert(novo);
            }

            if (grdProdutos != null) grdProdutos.DataBind();
        }
    
        private void ExibirRecebimento(bool exibir)
        {
            var valorObra = ObraDAO.Instance.GetValorObra(null, Request["idObra"].StrParaUint());
            if (exibir && valorObra == 0)
            {
                MensagemAlerta.ShowMsg("O valor do pagamento antecipado não pode ser zero.", Page);
                return;
            }
    
            var editar = dtvObra.FindControl("btnEditar") as Button;
            var finalizar = dtvObra.FindControl("btnFinalizar") as Button;
            var voltar = dtvObra.FindControl("btnVoltar") as Button;
            var receber = dtvObra.FindControl("btnReceber") as Button;
            var cancelar = dtvObra.FindControl("btnCancelar") as Button;
            var grdProdutos = dtvObra.FindControl("grdProdutoObra") as GridView;
            var panelReceber = dtvObra.FindControl("panReceber") as Panel;

            if (editar != null) editar.Visible = !exibir;
            if (finalizar != null) finalizar.Visible = !exibir;
            if (voltar != null) voltar.Visible = !exibir;
            if (receber != null) receber.Visible = exibir;
            if (cancelar != null) cancelar.Visible = exibir;
            if (panelReceber != null) panelReceber.Visible = exibir;

            if (grdProdutos != null)
            {
                grdProdutos.Columns[0].Visible = !exibir;
                grdProdutos.ShowFooter = !exibir;
                grdProdutos.AllowPaging = !exibir;
            }

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
                Response.Redirect("~/Listas/LstObra.aspx" + (Request["cxDiario"] == "1" ? "?cxDiario=1" : ""));
            }
        }
    
        protected void btnCancelarReceb_Click(object sender, EventArgs e)
        {
            ExibirRecebimento(false);
        }
    
        protected void btnReceber_Click(object sender, EventArgs e)
        {
            try
            {
                FilaOperacoes.ReceberObra.AguardarVez();
                var tipoPagto = dtvObra.FindControl("drpTipoPagto") as DropDownList;
                var obra = ObraDAO.Instance.GetElementByPrimaryKey(Request["idObra"].StrParaUint());
                string retorno;

                if (tipoPagto != null && tipoPagto.SelectedValue == "1")
                {
                    // À vista
                    var formaPagto1 =
                        dtvObra.FindControl("ctrlFormaPagto1") as Controls.ctrlFormaPagto;
                    if (formaPagto1 != null)
                    {
                        obra.ValoresPagto = formaPagto1.Valores;
                        obra.FormasPagto = formaPagto1.FormasPagto;
                        obra.TiposCartaoPagto = formaPagto1.TiposCartao;
                        obra.ParcelasCartaoPagto = formaPagto1.ParcelasCartao;
                        obra.ContasBancoPagto = formaPagto1.ContasBanco;
                        obra.ChequesPagto = formaPagto1.ChequesString;
                        obra.CreditoUtilizado = formaPagto1.CreditoUtilizado;
                        obra.DataRecebimento = formaPagto1.DataRecebimento;
                        obra.DepositoNaoIdentificado = formaPagto1.DepositosNaoIdentificados;
                        obra.NumAutCartao = formaPagto1.NumAutCartao;
                        obra.CartaoNaoIdentificado = formaPagto1.CartoesNaoIdentificados;
                    }

                    retorno = ObraDAO.Instance.PagamentoVista(obra, Request["cxDiario"] == "1", 0, formaPagto1.GerarCredito);
                }
                else
                {
                    // À prazo
                    var numParcelas = dtvObra.FindControl("drpNumParcelas") as DropDownList;
                    if (numParcelas != null) obra.NumParcelas = numParcelas.SelectedValue.StrParaInt();

                    var parcelas1 =
                        dtvObra.FindControl("ctrlParcelas1") as Controls.ctrlParcelas;
                    if (parcelas1 != null)
                    {
                        obra.DatasParcelas = parcelas1.Datas;
                        obra.ValoresParcelas = parcelas1.Valores;
                    }

                    retorno = ObraDAO.Instance.PagamentoPrazo(obra, Request["cxDiario"] == "1");
                }

                ObraDAO.Instance.AtualizaSaldoComTransacao(obra.IdObra, Request["cxDiario"] == "1", true);
                Page.ClientScript.RegisterClientScriptBlock(GetType(), "recebido",
                    "alert('" + retorno + "'); redirectUrl('../Listas/LstObra.aspx" +
                    (Request["cxDiario"] == "1" ? "?cxDiario=1" : "") + "');\n", true);
            }
            catch (Exception ex)
            {
                Glass.MensagemAlerta.ErrorMsg("Falha ao receber pagamento antecipado.", ex, Page);
            }
            finally
            {
                Glass.FilaOperacoes.ReceberObra.ProximoFila();
            }
        }
    
        protected void panReceber_Load(object sender, EventArgs e)
        {
            Panel p = (Panel)sender;
            p.Visible = !PedidoConfig.DadosPedido.UsarControleNovoObra;
        }
        
    }
}
