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
            Ajax.Utility.RegisterTypeForAjax(typeof(CadObraNovo));

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

        [Ajax.AjaxMethod]
        public string ObterIdLojaPeloCliente(string idCliente)
        {
            return ClienteDAO.Instance.ObtemIdLoja(idCliente.StrParaUint()).ToString();
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
            try
            {
                var grdProdutos = dtvObra.FindControl("grdProdutoObra") as GridView;
                if (grdProdutos != null)
                {
                    var idProd = ((HiddenField)grdProdutos.FooterRow.FindControl("hdfIdProd")).Value;
                    var valorUnit = ((TextBox)grdProdutos.FooterRow.FindControl("txtValorUnit")).Text;
                    var tamanhoMax = ((TextBox)grdProdutos.FooterRow.FindControl("txtTamanhoMax")).Text;

                    ProdutoObraDAO.Instance.VerificaProdutoComposicao(uint.Parse(idProd));

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
            catch (Exception ex)
            {
                MensagemAlerta.ErrorMsg("", ex, Page);
            }

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

        [Ajax.AjaxMethod]
        public string ReceberAVista(string idObra, string valores, string fPagtos, string tpCartoes, string parcCredito, string contas, string chequesPagto, string creditoUtilizado,
            string dataRecebido, string depositoNaoIdentificado, string numAutCartao, string cartaoNaoIdentificado, string isGerarCredito, string cxDiario, string receberCappta)
        {
            string[] sFormasPagto = fPagtos.Split(';');
            string[] sValoresReceb = valores.Split(';');
            string[] sIdContasBanco = contas.Split(';');
            string[] sTiposCartao = tpCartoes.Split(';');
            string[] sParcCartoes = parcCredito.Split(';');
            string[] sDepositoNaoIdentificado = depositoNaoIdentificado.Split(';');
            string[] sCartaoNaoIdentificado = cartaoNaoIdentificado.Split(';');
            string[] sNumAutCartao = numAutCartao.Split(';');

            uint[] formasPagto = new uint[sFormasPagto.Length];
            decimal[] valoresReceb = new decimal[sValoresReceb.Length];
            uint[] idContasBanco = new uint[sIdContasBanco.Length];
            uint[] tiposCartao = new uint[sTiposCartao.Length];
            uint[] parcCartoes = new uint[sParcCartoes.Length];
            uint[] depNaoIdentificado = new uint[sDepositoNaoIdentificado.Length];
            var cartNaoIdentificado = new uint[sCartaoNaoIdentificado.Length];

            for (var i = 0; i < sFormasPagto.Length; i++)
            {
                formasPagto[i] = !string.IsNullOrEmpty(sFormasPagto[i]) ? sFormasPagto[i].StrParaUint() : 0;
                valoresReceb[i] = !string.IsNullOrEmpty(sValoresReceb[i]) ? sValoresReceb[i].Replace('.', ',').StrParaDecimal() : 0;
                idContasBanco[i] = !string.IsNullOrEmpty(sIdContasBanco[i]) ? sIdContasBanco[i].StrParaUint() : 0;
                tiposCartao[i] = !string.IsNullOrEmpty(sTiposCartao[i]) ? sTiposCartao[i].StrParaUint() : 0;
                parcCartoes[i] = !string.IsNullOrEmpty(sParcCartoes[i]) ? sParcCartoes[i].StrParaUint() : 0;
                depNaoIdentificado[i] = !string.IsNullOrEmpty(sDepositoNaoIdentificado[i]) ? sDepositoNaoIdentificado[i].StrParaUint() : 0;
            }

            for (var i = 0; i < sCartaoNaoIdentificado.Length; i++)
            {
                cartNaoIdentificado[i] = !string.IsNullOrEmpty(sCartaoNaoIdentificado[i]) ? sCartaoNaoIdentificado[i].StrParaUint() : 0;
            }

            var obra = ObraDAO.Instance.GetElementByPrimaryKey(idObra.StrParaUint());

            obra.ValoresPagto = valoresReceb;
            obra.FormasPagto = formasPagto;
            obra.TiposCartaoPagto = tiposCartao;
            obra.ParcelasCartaoPagto = parcCartoes;
            obra.ContasBancoPagto = idContasBanco;
            obra.ChequesPagto = chequesPagto;
            obra.CreditoUtilizado = creditoUtilizado.StrParaDecimal();
            obra.DataRecebimento = dataRecebido.StrParaDate();
            obra.DepositoNaoIdentificado = depNaoIdentificado;
            obra.NumAutCartao = sNumAutCartao;
            obra.CartaoNaoIdentificado = cartNaoIdentificado;

            if (receberCappta == "true")
            {
                return ObraDAO.Instance.CriarPrePagamentoVistaComTransacao(cxDiario.ToLower() == "true", 0, obra, isGerarCredito.ToLower() == "true");
            }

            var retorno = ObraDAO.Instance.PagamentoVista(cxDiario.ToLower() == "true", 0, obra, isGerarCredito.ToLower() == "true");

            ObraDAO.Instance.AtualizaSaldoComTransacao(obra.IdObra, cxDiario.ToLower() == "true", true);

            return retorno;
        }

        [Ajax.AjaxMethod]
        public string ReceberAPrazo(string idObra, string formaPagto, string numParcelas, string valores, string datas, string cxDiario)
        {
            var obra = ObraDAO.Instance.GetElementByPrimaryKey(idObra.StrParaUint());

            var sValoresReceb = valores.Split(';');
            var sDatas = datas.Split(';');

            var valoresReceb = new decimal[sValoresReceb.Length];
            var datasReceb = new DateTime[sDatas.Length];

            for (int i = 0; i < valoresReceb.Length; i++)
            {
                valoresReceb[i] = !string.IsNullOrEmpty(sValoresReceb[i]) ? Convert.ToDecimal(sValoresReceb[i].Replace('.', ',')) : 0;
                datasReceb[i] = !string.IsNullOrEmpty(sDatas[i]) ? sDatas[i].StrParaDate().GetValueOrDefault() : new DateTime();
            }

            if (numParcelas.StrParaIntNullable().GetValueOrDefault(0) > 0)
                obra.NumParcelas = numParcelas.StrParaInt();

            obra.DatasParcelas = datasReceb;
            obra.ValoresParcelas = valoresReceb;
            obra.FormasPagto = new uint[] { formaPagto.StrParaUint() };

            var retorno = ObraDAO.Instance.PagamentoPrazo(obra, cxDiario.ToLower() == "true");

            ObraDAO.Instance.AtualizaSaldoComTransacao(obra.IdObra, cxDiario.ToLower() == "true", true);

            return retorno;
        }

        protected void panReceber_Load(object sender, EventArgs e)
        {
            Panel p = (Panel)sender;
            p.Visible = !PedidoConfig.DadosPedido.UsarControleNovoObra;
        }

        protected void drpLoja_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
                ((DropDownList)sender).SelectedIndex = (int)UserInfo.GetUserInfo.IdLoja;
        }

        protected void ctrlFormaPagto1_Init(object sender, EventArgs e)
        {
            Glass.UI.Web.Controls.ctrlFormaPagto ctrlFormaPagto = (Glass.UI.Web.Controls.ctrlFormaPagto)sender;

            ctrlFormaPagto.ExibirApenasCartaoDebito = FinanceiroConfig.FinanceiroRec.ConsiderarApenasDebitoComoPagtoAvista;
        }
    }
}
