using System;
using System.Web.UI.WebControls;
using Glass.Data.DAL;
using Glass.Configuracoes;
using Glass.Data.Helper;

namespace Glass.UI.Web.Cadastros
{
    public partial class CadContaReceberComposto : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            Ajax.Utility.RegisterTypeForAjax(typeof(MetodosAjax));
            Ajax.Utility.RegisterTypeForAjax(typeof(Glass.UI.Web.Cadastros.CadContaReceberComposto));
    
            hdfCxDiario.Value = Request["cxDiario"];
    
            if (!IsPostBack)
                CarregaParcelasRenegociar();

            if (ComissaoDAO.Instance.VerificarComissaoContasRecebidas() && (Configuracoes.FinanceiroConfig.FinanceiroPagto.SubtrairICMSCalculoComissao
                || Configuracoes.ComissaoConfig.TotalParaComissao != Configuracoes.ComissaoConfig.TotalComissaoEnum.TotalSemImpostos))
                chkRenegociar.Style.Add("display", "none");
        }
    
        [Ajax.AjaxMethod()]
        public string GetContasRecFromPedido(string idCliente, string idPedido)
        {
            return WebGlass.Business.ContasReceber.Fluxo.BuscarEValidar.Ajax.
                GetContasRecFromPedido(idCliente, idPedido);
        }
    
        /// <summary>
        /// Recebe as contas
        /// </summary>
        [Ajax.AjaxMethod()]
        public string Receber(string idCliente, string contas, string dataRecebido, string totalASerPago, string fPagtos, string valores, string contasBanco, string depositoNaoIdentificado,
            string cartaoNaoIdentificado, string tpCartoes, string tpBoletos, string txAntecip, string juros, string parcial, string gerarCredito, string creditoUtilizado, string cxDiario,
            string numAutConstrucard, string parcCredito, string chequesPagto, string descontarComissao, string obs, string numAutCartao, string receberCappta)
        {
            return WebGlass.Business.Acerto.Fluxo.Receber.Ajax.ReceberAcerto(idCliente, contas, dataRecebido, totalASerPago, fPagtos, valores, contasBanco, depositoNaoIdentificado,
                cartaoNaoIdentificado, tpCartoes, tpBoletos, txAntecip, juros, parcial, gerarCredito, creditoUtilizado, cxDiario, numAutConstrucard, parcCredito, chequesPagto, descontarComissao, obs,
                numAutCartao, receberCappta);
        }
        
        [Ajax.AjaxMethod()]
        public string Renegociar(string idCliente, string idContasR, string idFormaPagto, string numParc, string parcelas, string multa, 
            string creditoUtilizado, string obs)
        {
            return WebGlass.Business.Acerto.Fluxo.Receber.Ajax.Renegociar(idCliente, idContasR, idFormaPagto, numParc,
                parcelas, multa, creditoUtilizado, obs);
        }
    
        [Ajax.AjaxMethod()]
        public string TemCnabGerado(string idsContaR)
        {
            foreach (var idContaR in idsContaR.Trim(',').Split(','))
            {
                if (ContasReceberDAO.Instance.TemCnabGerado(Glass.Conversoes.StrParaUint(idContaR)))
                    return "true";
            }
    
            return "false";
        }
    
        protected void drpParcCredito_Load(object sender, EventArgs e)
        {
            ((DropDownList)sender).Visible = FinanceiroConfig.Cartao.PedidoJurosCartao;
        }
    
        protected void ctrlFormaPagto1_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
                ctrlFormaPagto1.DataRecebimento = DateTime.Now;
    
            ctrlFormaPagto1.CampoCredito = hdfValorCredito;
            ctrlFormaPagto1.CampoValorConta = hdfTotalContas;
            ctrlFormaPagto1.CampoClienteID = txtNumCli;
        }
    
        protected void ctrlParcelas_Load(object sender, EventArgs e)
        {
            Glass.UI.Web.Controls.ctrlParcelas ctrlParcelas = (Glass.UI.Web.Controls.ctrlParcelas)sender;
    
            ctrlParcelas.CampoCalcularParcelas = hdfCalcularParcelas;
            ctrlParcelas.CampoParcelasVisiveis = drpNumParc;
            ctrlParcelas.CampoValorTotal = lblTotalContas;
            ctrlParcelas.CampoValorDescontoAtual = hdfDescontoParc;
        }
    
        private void CarregaParcelasRenegociar()
        {
            drpNumParc.Items.Clear();
            for (int i = 0; i < FinanceiroConfig.FormaPagamento.NumeroParcelasRenegociar; i++)
                drpNumParc.Items.Add(new ListItem((i + 1).ToString()));
    
            ctrlParcelas.NumParcelas = FinanceiroConfig.FormaPagamento.NumeroParcelasRenegociar;
        }
    
        protected bool AbrirRptFinalizar()
        {
            return true;
        }
    
        protected void btnBuscar_Click(object sender, EventArgs e)
        {
    
        }
    }
}
