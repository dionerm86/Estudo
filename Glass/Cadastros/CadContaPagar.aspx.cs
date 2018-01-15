using System;
using System.Web.UI;
using System.Web.UI.WebControls;
using Glass.Configuracoes;

namespace Glass.UI.Web.Cadastros
{
    public partial class CadContaPagar : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                if (Request["idCompra"] != null)
                {
                    txtNumCompra.Text = Request["idCompra"];
                    Page.ClientScript.RegisterStartupScript(GetType(), "addCompra", "getContasByCompra(" + Request["idCompra"] + ", true);\n" +
                        "FindControl('Pagto1_txtValor', 'input').focus();\n", true);
                }
                else if (Request["idImpostoServ"] != null)
                {
                    Page.ClientScript.RegisterStartupScript(GetType(), "addImpostoServ", "getContasByImpostoServ(" + Request["idImpostoServ"] + ", true);\n" +
                        "FindControl('Pagto1_txtValor', 'input').focus();\n", true);
                }
    
                CarregaParcelasRenegociar();
    
                if (Request["retificar"] == "1")
                {
                    hdfRetificarPagto.Value = "true";
                    Page.Title = "Retificar Pagamento";
                    buscarContas.Visible = false;
                    buscarPagamento.Visible = true;
                    btnPagar.Text = "Retificar";
                    gerarCreditoCheque.Visible = FinanceiroConfig.FormaPagamento.CreditoFornecedor;
                }
            }
    
            Ajax.Utility.RegisterTypeForAjax(typeof(MetodosAjax));
            Ajax.Utility.RegisterTypeForAjax(typeof(Glass.UI.Web.Cadastros.CadContaPagar));
    
            hdfCredito.Value = FinanceiroConfig.FormaPagamento.CreditoFornecedor.ToString().ToLower();
        }
    
        #region Métodos Ajax
    
        [Ajax.AjaxMethod()]
        public string ValidarPagamento(string idPagtoStr)
        {
            return WebGlass.Business.Pagamento.Fluxo.BuscarEValidar.Ajax.ValidarPagamento(idPagtoStr);
        }
    
        [Ajax.AjaxMethod()]
        public string Pagar(string idPagtoStr, string idFornecStr, string contas, string chequesAssoc, string vetJurosMulta, string dataPagto, string datasFormasPagtoStr, 
            string valoresStr, string formasPagtoStr, string tiposCartaoStr, string numParcCartaoStr, string chequesPagto, string idContasBancoStr, string boletosStr, 
            string antecipFornecStr, string desconto, string obs, string gerarCredito, string creditoUtilizado, string pagtoParcial, string numAutConstrucard, 
            string retificarStr, string gerarCreditoRetificarStr, string idsContasRemovidas, string lixo)
        {
            return WebGlass.Business.ContasPagar.Fluxo.Pagar.Ajax.PagarContas(idPagtoStr, idFornecStr, contas, chequesAssoc, vetJurosMulta,
                dataPagto, datasFormasPagtoStr, valoresStr, formasPagtoStr, tiposCartaoStr, numParcCartaoStr, chequesPagto, idContasBancoStr,
                boletosStr, antecipFornecStr, desconto, obs, gerarCredito, creditoUtilizado, pagtoParcial, numAutConstrucard, retificarStr, 
                gerarCreditoRetificarStr, idsContasRemovidas, lixo);
        }
    
        [Ajax.AjaxMethod]
        public string Renegociar(string idPagtoStr, string idFornecStr, string contas, string numParcelasStr, string datasStr, string valoresStr,
            string vetJurosMulta, string retificarStr, string lixo)
        {
            return WebGlass.Business.ContasPagar.Fluxo.Pagar.Ajax.Renegociar(idPagtoStr, idFornecStr, contas, numParcelasStr,
                datasStr, valoresStr, vetJurosMulta, retificarStr, lixo);
        }
    
        [Ajax.AjaxMethod()]
        public string GetContasByCompra(string idCompra, string soAVistaStr)
        {
            return WebGlass.Business.ContasPagar.Fluxo.BuscarEValidar.Ajax.GetContasByCompra(idCompra, soAVistaStr);
        }
    
        [Ajax.AjaxMethod()]
        public string GetContasByCustoFixo(string idCustoFixo, string soAVistaStr)
        {
            return WebGlass.Business.ContasPagar.Fluxo.BuscarEValidar.Ajax.GetContasByCustoFixo(idCustoFixo, soAVistaStr);
        }
    
        [Ajax.AjaxMethod()]
        public string GetContasByImpostoServ(string idImpostoServ, string soAVistaStr)
        {
            return WebGlass.Business.ContasPagar.Fluxo.BuscarEValidar.Ajax.GetContasByImpostoServ(idImpostoServ, soAVistaStr);
        }
    
        [Ajax.AjaxMethod()]
        public string GetDadosPagto(string idPagto, string controleFormaPagto, string controleParcelas, 
            string callbackIncluir, string callbackExcluir)
        {
            return WebGlass.Business.ContasPagar.Fluxo.BuscarEValidar.Ajax.GetDadosPagto(idPagto, controleFormaPagto, controleParcelas,
                callbackIncluir, callbackExcluir);
        }

        [Ajax.AjaxMethod()]
        public string ObterFornecVinculado(string idFornec)
        {
            return Glass.Data.DAL.FornecedorVinculoDAO.Instance.ObterVinculo(null, idFornec.StrParaInt());
        }
    
        #endregion
    
        protected void imgPesq_Click(object sender, ImageClickEventArgs e)
        {
    
        }
    
        protected void ctrlFormaPagto1_Load(object sender, EventArgs e)
        {
            ctrlFormaPagto1.CampoCredito = hdfValorCredito;
            ctrlFormaPagto1.CampoValorConta = hdfTotalASerPago;
            ctrlFormaPagto1.ExibirCredito = FinanceiroConfig.FormaPagamento.CreditoFornecedor;
            ctrlFormaPagto1.ExibirGerarCredito = FinanceiroConfig.FormaPagamento.CreditoFornecedor;
            ctrlFormaPagto1.ExibirUsarCredito = FinanceiroConfig.FormaPagamento.CreditoFornecedor;
            ctrlFormaPagto1.CampoValorDesconto = txtDesconto;
            ctrlFormaPagto1.CampoValorAcrescimo = hdfAcrescimo;
            ctrlFormaPagto1.NumPossibilidadesPagto = FinanceiroConfig.FormaPagamento.NumeroFormasPagtoContasPagar;
            ctrlFormaPagto1.CampoFornecedorID = hdfIdFornec;
        }
    
        protected void ctrlFormaPagto1_PreRender(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                DateTime?[] datas = new DateTime?[ctrlFormaPagto1.NumPossibilidadesPagto];
                for (int i = 0; i < ctrlFormaPagto1.NumPossibilidadesPagto; i++)
                    datas[i] = DateTime.Now;
    
                ctrlFormaPagto1.DataRecebimento = DateTime.Now;
                ctrlFormaPagto1.DatasFormasPagamento = datas;
            }
        }
    
        protected void ctrlParcelas_Load(object sender, EventArgs e)
        {
            Glass.UI.Web.Controls.ctrlParcelas ctrlParcelas = (Glass.UI.Web.Controls.ctrlParcelas)sender;
    
            ctrlParcelas.CampoCalcularParcelas = hdfCalcularParcelas;
            ctrlParcelas.CampoParcelasVisiveis = drpNumParc;
            ctrlParcelas.CampoValorTotal = ctrlFormaPagto1.FindControl("lblValorASerPago");
        }
    
        private void CarregaParcelasRenegociar()
        {
            drpNumParc.Items.Clear();
            for (int i = 0; i < FinanceiroConfig.FormaPagamento.NumeroParcelasRenegociar; i++)
                drpNumParc.Items.Add(new ListItem((i + 1).ToString()));
    
            ctrlParcelas.NumParcelas = FinanceiroConfig.FormaPagamento.NumeroParcelasRenegociar;
        }
    }
}
