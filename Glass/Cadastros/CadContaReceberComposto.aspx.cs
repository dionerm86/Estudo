using System;
using System.Web.UI.WebControls;
using Glass.Data.DAL;
using Glass.Configuracoes;

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
    
            if (Configuracoes.ComissaoConfig.ComissaoPorContasRecebidas)
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
        /// <param name="idAcerto"></param>
        /// <param name="contas"></param>
        /// <param name="dataRecebido"></param>
        /// <param name="totalASerPago"></param>
        /// <param name="fPagto1"></param>
        /// <param name="valor1"></param>
        /// <param name="fPagto2"></param>
        /// <param name="valor2"></param>
        /// <param name="conta1"></param>
        /// <param name="conta2"></param>
        /// <param name="tpCartao1"></param>
        /// <param name="tpCartao2"></param>
        /// <param name="tpBoleto1"></param>
        /// <param name="tpBoleto2"></param>
        /// <param name="juros"></param>
        /// <param name="parcial"></param>
        /// <param name="gerarCredito"></param>
        /// <param name="creditoUtilizado"></param>
        /// <param name="cxDiario"></param>
        /// <returns></returns>
        [Ajax.AjaxMethod()]
        public string Receber(string idCliente, string contas, string dataRecebido, string totalASerPago, string fPagtos, string valores,
            string contasBanco, string depositoNaoIdentificado, string cartaoNaoIdentificado, string tpCartoes, string tpBoletos, string txAntecip, string juros, string parcial, string gerarCredito, 
            string creditoUtilizado, string cxDiario, string numAutConstrucard, string parcCredito, string chequesPagto, 
            string descontarComissao, string obs, string numAutCartao)
        {
            return WebGlass.Business.Acerto.Fluxo.Receber.Ajax.ReceberAcerto(idCliente, contas, dataRecebido, totalASerPago,
                fPagtos, valores, contasBanco, depositoNaoIdentificado, cartaoNaoIdentificado, tpCartoes, tpBoletos, txAntecip, juros, parcial, gerarCredito, creditoUtilizado,
                cxDiario, numAutConstrucard, parcCredito, chequesPagto, descontarComissao, obs, numAutCartao);
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
    
        //protected bool ExibirCnab()
        //{
        //    return FinanceiroConfig.FinanceiroRec.ExibirCnab;
        //}
    
        protected void btnBuscar_Click(object sender, EventArgs e)
        {
    
        }
    }
}
