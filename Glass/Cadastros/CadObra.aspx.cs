using System;
using System.Globalization;
using System.Web.UI.WebControls;
using Glass.Data.Helper;
using Glass.Data.DAL;
using Glass.Configuracoes;
using System.Linq;

namespace Glass.UI.Web.Cadastros
{
    public partial class CadObra : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            Ajax.Utility.RegisterTypeForAjax(typeof(CadObra));

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

        protected void drpFuncionario_DataBound(object sender, EventArgs e)
        {
            // Preenche o campo vendedor com o usuário logado, caso exista na drop
            if (!IsPostBack && dtvObra.CurrentMode == DetailsViewMode.Insert && 
                ((DropDownList)dtvObra.FindControl("drpFuncionario")).Items.FindByValue(UserInfo.GetUserInfo.CodUser.ToString()) != null)
            { 
                ((DropDownList)dtvObra.FindControl("drpFuncionario")).SelectedValue = UserInfo.GetUserInfo.CodUser.ToString();
            }
        }

        [Ajax.AjaxMethod]
        public string ReceberAVista(string idObra, string valores, string fPagtos, string tpCartoes, string parcCredito, string contas, string chequesPagto, string creditoUtilizado,
            string dataRecebido, string depositoNaoIdentificado, string numAutCartao, string cartaoNaoIdentificado, string isGerarCredito, string cxDiario, string receberCappta)
        {
            var sFormasPagto = fPagtos.Split(';');
            var sValoresReceb = valores.Split(';');
            var sIdContasBanco = contas.Split(';');
            var sTiposCartao = tpCartoes.Split(';');
            var sParcCartoes = parcCredito.Split(';');
            var sDepositoNaoIdentificado = depositoNaoIdentificado.Split(';');
            var sCartaoNaoIdentificado = cartaoNaoIdentificado.Split(';');
            var sNumAutCartao = numAutCartao.Split(';');

            var formasPagto = new uint[sFormasPagto.Length];
            var valoresReceb = new decimal[sValoresReceb.Length];
            var idContasBanco = new uint[sIdContasBanco.Length];
            var tiposCartao = new uint[sTiposCartao.Length];
            var parcCartoes = new uint[sParcCartoes.Length];
            var depNaoIdentificado = new uint[sDepositoNaoIdentificado.Length];
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

            return receberCappta == "true" ?
                ObraDAO.Instance.CriarPrePagamentoVistaComTransacao(cxDiario.ToLower() == "true", 0, obra, false) :
                ObraDAO.Instance.PagamentoVista(cxDiario.ToLower() == "true", 0, obra, false);
        }

        [Ajax.AjaxMethod]
        public string ReceberAPrazo(string idObra, string numParcelas, string formaPagto, string valores, string datas, string cxDiario)
        {
            var obra = ObraDAO.Instance.GetElementByPrimaryKey(idObra.StrParaUint());
            
            var sValoresReceb = valores.Split(';');
            var sDatas = datas.Split(';');
            
            var valoresReceb = new decimal[sValoresReceb.Length];
            var datasReceb = new DateTime[sDatas.Length];

            for (int i = 0; i < valoresReceb.Length; i++)
            {
                valoresReceb[i] = !string.IsNullOrEmpty(sValoresReceb[i]) ? sValoresReceb[i].Replace('.', ',').StrParaDecimal() : 0;
                datasReceb[i] = !string.IsNullOrEmpty(sDatas[i]) ? sDatas[i].StrParaDate().GetValueOrDefault() : new DateTime();
            }

            if (numParcelas.StrParaIntNullable().GetValueOrDefault(0) > 0)
                obra.NumParcelas = numParcelas.StrParaInt();

            obra.FormasPagto = new uint[] { formaPagto.StrParaUint() };
            obra.DatasParcelas = datasReceb;
            obra.ValoresParcelas = valoresReceb;

            return ObraDAO.Instance.PagamentoPrazo(obra, cxDiario.ToLower() == "true");
        }

        protected void drpFuncionario_DataBinding(object sender, EventArgs e)
        {
            if (dtvObra.CurrentMode == DetailsViewMode.Edit)
            {
                var funcionario = FuncionarioDAO.Instance.GetVendedores();
                var idFunc = ObraDAO.Instance.ObtemIdFunc(null, Request["idObra"].StrParaUint());

                if (!funcionario.Any(f => f.IdFunc == idFunc))
                    ((DropDownList)dtvObra.FindControl("drpFuncionario")).Items.Add(new ListItem(FuncionarioDAO.Instance.GetNome(idFunc), idFunc.ToString()));
            }
        }

        protected void ctrlFormaPagto1_Init(object sender, EventArgs e)
        {
            Glass.UI.Web.Controls.ctrlFormaPagto ctrlFormaPagto = (Glass.UI.Web.Controls.ctrlFormaPagto)sender;

            ctrlFormaPagto.ExibirApenasCartaoDebito = FinanceiroConfig.FinanceiroRec.ConsiderarApenasDebitoComoPagtoAvista;
        }
    }
}
