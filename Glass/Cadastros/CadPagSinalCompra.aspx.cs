using System;
using System.Web.UI;
using System.Web.UI.WebControls;
using Glass.Data.DAL;
using Glass.Configuracoes;

namespace Glass.UI.Web.Cadastros
{
    public partial class CadPagSinalCompra : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            Ajax.Utility.RegisterTypeForAjax(typeof(Glass.UI.Web.Cadastros.CadPagSinalCompra));
            Ajax.Utility.RegisterTypeForAjax(typeof(MetodosAjax));
    
            if (!IsPostBack && Request["idCompra"] != null)
            {
                hdfBuscarIdsCompras.Value = Request["idCompra"];
                grdCompra.DataBind();
    
                // Esconde os dados sobre o ICMS se a empresa não calcular
                grdCompra.Columns[6].Visible = PedidoConfig.Impostos.CalcularIcmsPedido;
            }
    
            if (hdfIdsComprasRem.Value != String.Empty)
            {
                lblComprasRem.Text = "Compras Removidas: " + hdfIdsComprasRem.Value.TrimEnd(',');
                imgLimparRemovidos.Visible = true;
            }       
        }
    
        #region Clicks
    
        protected void btnBuscarCompras_Click(object sender, EventArgs e)
        {
            grdCompra.DataBind();
        }
    
        protected void imgLimparRemovidos_Click(object sender, ImageClickEventArgs e)
        {
            hdfBuscarIdsCompras.Value += (!String.IsNullOrEmpty(hdfBuscarIdsCompras.Value) ? "," : "") + hdfIdsComprasRem.Value.Trim(',');
            hdfIdsComprasRem.Value = String.Empty;
            lblComprasRem.Text = "";
            imgLimparRemovidos.Visible = false;
            
            grdCompra.DataBind();
        }
    
        #endregion
    
        #region Métodos usados na pagina
    
        private bool corAlternada = true;
    
        protected string GetAlternateClass()
        {
            corAlternada = !corAlternada;
            return corAlternada ? "alt" : "";
        }
    
        #endregion
    
        #region Metodos AJAX
    
        [Ajax.AjaxMethod]
        public string ValidaCompra(string idCompraStr, string idFornecStr)
        {
            try
            {
                uint idCompra = Glass.Conversoes.StrParaUint(idCompraStr);
                uint idFornec = Glass.Conversoes.StrParaUint(idFornecStr);
    
                // Verifica se a compra existe
                if (!CompraDAO.Instance.CompraExists(idCompra))
                    return "false|Não existe compra com esse número.";
    
                if (!CompraDAO.Instance.TemSinalPagar(idCompra))
                    return "false|Não existe sinal a pagar para essa compra.";
    
                if (!String.IsNullOrEmpty(idFornecStr) && Glass.Conversoes.StrParaUint(idFornecStr) != CompraDAO.Instance.ObtemIdFornec(idCompra))
                    return "false|O fornecedor dessa compra é diferente do fornecedor da(s) compra(s) já selecionada(s).";
    
                return "true";
            }
            catch (Exception ex)
            {
                return "false|" + ex.Message;
            }
        }
    
        [Ajax.AjaxMethod]
        public string GetComprasByFornec(string idFornec, string nomeFornec, string idsComprasRem)
        {
            try
            {
                return CompraDAO.Instance.GetIdsComprasForPagarSinal(Glass.Conversoes.StrParaUint(idFornec), nomeFornec, idsComprasRem);
            }
            catch
            {
                return "0";
            }
        }
    
        [Ajax.AjaxMethod]
        public string GetTotalCompras(string idsCompras)
        {
            uint id = 0;
            decimal total = 0;
            foreach (string s in idsCompras.Split(','))
                if (uint.TryParse(s, out id))
                    total += CompraDAO.Instance.ObtemValorEntrada(id);
    
            return total.ToString();
        }
    
        [Ajax.AjaxMethod]
        public string PagarSinalCompra(string idsComprasStr, string valoresStr, string formasPagtoStr, string idContasBancoStr, string tiposCartaoStr,
            string numParcCartaoStr, string datasFormasPagtoStr, string obs, string boletosStr, string gerarCreditoStr, string creditoUtilizadoStr,
            string chequesPgtoStr, string idFornecStr)
        {
            try
            {
                string[] vIdsCompras = idsComprasStr.Split(';');
                string[] vPagto = valoresStr.Split(';');
                string[] fPagto = formasPagtoStr.Split(';');
                string[] cPagto = idContasBancoStr.Split(';');
                string[] tPagto = tiposCartaoStr.Split(';');
                string[] pPagto = numParcCartaoStr.Split(';');
                string[] dPagto = datasFormasPagtoStr.Split(';');
                string[] bPagto = boletosStr.Split(';');
    
                decimal[] valores = new decimal[vPagto.Length];
                uint[] formasPagto = new uint[fPagto.Length];
                uint[] contasBanco = new uint[cPagto.Length];
                uint[] tiposCartao = new uint[tPagto.Length];
                uint[] numParcCartao = new uint[pPagto.Length];
                DateTime[] datasFormasPagto = new DateTime[dPagto.Length];
    
                for (int i = 0; i < vPagto.Length; i++)
                {
                    valores[i] = Glass.Conversoes.StrParaDecimal(vPagto[i]);
                    formasPagto[i] = Glass.Conversoes.StrParaUint(fPagto[i]);
                    contasBanco[i] = Glass.Conversoes.StrParaUint(cPagto[i]);
                    tiposCartao[i] = Glass.Conversoes.StrParaUint(tPagto[i]);
                    numParcCartao[i] = Glass.Conversoes.StrParaUint(pPagto[i]);
                    datasFormasPagto[i] = !String.IsNullOrEmpty(dPagto[i]) ? Conversoes.ConverteDataNotNull(dPagto[i]) : DateTime.Now;
                }
    
                if (obs != null && obs.Length > 300)
                    return "Erro\t";
    
                uint idFornec = Glass.Conversoes.StrParaUint(idFornecStr);
                decimal creditoUtilizado = Glass.Conversoes.StrParaDecimal(creditoUtilizadoStr);
                bool gerarCredito = bool.Parse(gerarCreditoStr);
    
                //Cria os sianais
                uint idSinalCompra = SinalCompraDAO.Instance.Pagar(idsComprasStr, valores, formasPagto, contasBanco, tiposCartao,
                    numParcCartao, datasFormasPagto, obs, bPagto, gerarCredito, creditoUtilizado, chequesPgtoStr, idFornec);
    
                return "ok;" + idSinalCompra;
                
                
            }
            catch (Exception ex)
            {
                return "Erro;" + ex.Message;
            }
        }
    
        #endregion
    
        protected void grdCompra_DataBound(object sender, EventArgs e)
        {
            receber.Visible = grdCompra.Rows.Count > 0;
            decimal totalPagar = 0;
            decimal valorCredito = 0;
    
            if (grdCompra.Rows.Count > 0)
            {
                hdfIdFornec.Value = ((HiddenField)grdCompra.Rows[0].FindControl("hdfIdFornecedor")).Value;
    
                // Calcula o total a ser pago
                for (int i = 0; i < grdCompra.Rows.Count; i++)
                    totalPagar += Glass.Conversoes.StrParaDecimal(((HiddenField)grdCompra.Rows[i].FindControl("hdfValorEntrada")).Value);
    
                // Recupera o crédito do fornecedor
                valorCredito = FornecedorDAO.Instance.GetCredito(Glass.Conversoes.StrParaUint(hdfIdFornec.Value));
            }
    
            hdfTotalASerPago.Value = totalPagar.ToString("0.##");
            hdfValorCredito.Value = valorCredito.ToString("0.##");
        }
    
        protected void ctrlFormaPagto1_Load(object sender, EventArgs e)
        {
            ctrlFormaPagto1.CampoCredito = hdfValorCredito;
            ctrlFormaPagto1.CampoValorConta = hdfTotalASerPago;
            ctrlFormaPagto1.ExibirCredito = FinanceiroConfig.FormaPagamento.CreditoFornecedor;
            ctrlFormaPagto1.ExibirGerarCredito = FinanceiroConfig.FormaPagamento.CreditoFornecedor;
            ctrlFormaPagto1.NumPossibilidadesPagto = FinanceiroConfig.FormaPagamento.NumeroFormasPagtoContasPagar;
            ctrlFormaPagto1.DataRecebimento = DateTime.Now;
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
    }
}
