using System;
using Glass.Data.DAL;

namespace Glass.UI.Web.Cadastros
{
    public partial class CadRecebChequeDevolvido : System.Web.UI.Page
    {       
        protected void Page_Load(object sender, EventArgs e)
        {
            Ajax.Utility.RegisterTypeForAjax(typeof(MetodosAjax));
            Ajax.Utility.RegisterTypeForAjax(typeof(CadRecebChequeDevolvido));     
    
            if(Request["caixaDiario"] != "true")
                btnBuscarCheques.OnClientClick = btnBuscarCheques.OnClientClick.Replace("&caixaDiario=true", "");

            if (IsQuitarReapresentados())
            {
                Page.Title = "Quitar Cheque Reapresentado";
                btnBuscarCheques.OnClientClick = btnBuscarCheques.OnClientClick.Replace("tipo=5", "tipo=5&reapresentados=1");
    
                ctrlFormaPagto1.ExibirCredito = false;
                ctrlFormaPagto1.ExibirGerarCredito = false;
                ctrlFormaPagto1.ExibirRecebParcial = false;
                ctrlFormaPagto1.ExibirUsarCredito = false;
                ctrlFormaPagto1.ExibirValorRestante = false;
                Page.ClientScript.RegisterStartupScript(GetType(), "esconderFormaPagto", "reapresentar();", true);
            }
    
            if (IsFinanceiroPagto())
            {
                if (!IsPostBack)
                    Page.Title = Page.Title.Replace("Cheque", "Cheque Próprio");
    
                ctrlFormaPagto1.ExibirCliente = false;
                ctrlFormaPagto1.ExibirCredito = false;
                ctrlFormaPagto1.ExibirGerarCredito = false;
                ctrlFormaPagto1.ExibirUsarCredito = false;
                if (btnBuscarCheques.OnClientClick.IndexOf("&pagto=1", StringComparison.Ordinal) == -1)
                    btnBuscarCheques.OnClientClick = btnBuscarCheques.OnClientClick.Replace("tipo=5", "tipo=5&pagto=1");
            }
        }
    
        [Ajax.AjaxMethod()]
        public string Receber(string idsCheque, string dataRecebido, string fPagtos, string valores, string tpCartoes,
            string contas, string depositoNaoIdentificado, string cartaoNaoIdentificado, string juros, string numAutConstrucard, string parcial, string parcCredito, string chequesPagto, string gerarCredito,
            string creditoUtilizado, string idClienteStr, string descontoStr, string isChequeProprio, string obs, string chequesCaixaDiario, string numAutCartao)
        {
            uint idAcertoCheque = 0;

            try
            {
                FilaOperacoes.Recebimento.AguardarVez();
                string[] sIdCheque = idsCheque.Trim(',').Split(',');
                uint[] idCheque = new uint[sIdCheque.Length];

                for (int i = 0; i < sIdCheque.Length; i++)
                    idCheque[i] = !string.IsNullOrEmpty(sIdCheque[i]) ? Convert.ToUInt32(sIdCheque[i]) : 0;

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
                uint[] cartNaoIdentificado = new uint[sCartaoNaoIdentificado.Length];

                for (int i = 0; i < sFormasPagto.Length; i++)
                {
                    formasPagto[i] = !string.IsNullOrEmpty(sFormasPagto[i]) ? Convert.ToUInt32(sFormasPagto[i]) : 0;
                    valoresReceb[i] = !string.IsNullOrEmpty(sValoresReceb[i])
                        ? Convert.ToDecimal(sValoresReceb[i].Replace('.', ','))
                        : 0;
                    idContasBanco[i] = !string.IsNullOrEmpty(sIdContasBanco[i])
                        ? Convert.ToUInt32(sIdContasBanco[i])
                        : 0;
                    tiposCartao[i] = !string.IsNullOrEmpty(sTiposCartao[i]) ? Convert.ToUInt32(sTiposCartao[i]) : 0;
                    parcCartoes[i] = !string.IsNullOrEmpty(sParcCartoes[i]) ? Convert.ToUInt32(sParcCartoes[i]) : 0;
                    depNaoIdentificado[i] = !string.IsNullOrEmpty(sDepositoNaoIdentificado[i])
                        ? Convert.ToUInt32(sDepositoNaoIdentificado[i])
                        : 0;                    
                }

                for (int i = 0; i < sCartaoNaoIdentificado.Length; i++)
                {
                    cartNaoIdentificado[i] = !string.IsNullOrEmpty(sCartaoNaoIdentificado[i]) ? Convert.ToUInt32(sCartaoNaoIdentificado[i]) : 0;
                }

                decimal jurosReceb = juros.StrParaDecimal();
                decimal creditoUtil = creditoUtilizado.StrParaDecimal();
                uint idCliente = !string.IsNullOrEmpty(idClienteStr) ? idClienteStr.StrParaUint() : 0;
                decimal desconto = descontoStr.StrParaDecimal();

                // Quita Cheque Devolvido
                idAcertoCheque = ChequesDAO.Instance.QuitarChequeDevolvido(idCheque, DateTime.Parse(dataRecebido),
                    formasPagto, valoresReceb, tiposCartao,
                    idContasBanco, depNaoIdentificado, cartNaoIdentificado, jurosReceb, numAutConstrucard, parcial == "true", parcCartoes,
                    chequesPagto, gerarCredito == "true", creditoUtil,
                    idCliente, desconto, isChequeProprio == "true", obs, chequesCaixaDiario == "true", sNumAutCartao);

                return "ok\t" + idAcertoCheque;
            }
            catch (Exception ex)
            {
                if (ex.Message == "Valor recebido.")
                    return "ok\t" + idAcertoCheque;

                return "Erro\t" + ex.Message;
            }
            finally
            {
                Glass.FilaOperacoes.Recebimento.ProximoFila();
            }
        }
    
        [Ajax.AjaxMethod]
        public string QuitarReapresentados(string idsCheque, string idClienteStr, string idContaBancoStr, string dataRecebido, 
            string juros, string descontoStr, string isChequeProprio, string obs)
        {
            try
            {
                FilaOperacoes.QuitarChequeDevolvidoEmAberto.AguardarVez();

                string[] sIdCheque = idsCheque.Trim(',').Split(',');
                uint[] idCheque = new uint[sIdCheque.Length];

                for (int i = 0; i < sIdCheque.Length; i++)
                    idCheque[i] = !String.IsNullOrEmpty(sIdCheque[i]) ? Convert.ToUInt32(sIdCheque[i]) : 0;

                decimal jurosReceb = juros.StrParaDecimal();
                uint idCliente = !String.IsNullOrEmpty(idClienteStr) ? idClienteStr.StrParaUint() : 0;
                uint idContaBanco = !String.IsNullOrEmpty(idContaBancoStr)
                    ? idContaBancoStr.StrParaUint()
                    : 0;
                decimal desconto = descontoStr.StrParaDecimal();

                // Quita Cheque Reapresentado
                uint idAcertoCheque = ChequesDAO.Instance.QuitarChequeReapresentado(idCheque, idCliente, idContaBanco,
                    DateTime.Parse(dataRecebido), jurosReceb,
                    desconto, isChequeProprio == "true", obs);

                return "ok\t" + idAcertoCheque;
            }
            catch (Exception ex)
            {
                if (ex.Message == "Valor recebido.")
                    return "ok\tValor recebido.";

                return "Erro\t" + ex.Message;
            }
            finally
            {
                Glass.FilaOperacoes.QuitarChequeDevolvidoEmAberto.ProximoFila();
            }
        }
    
        protected void ctrlFormaPagto1_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
                ctrlFormaPagto1.DataRecebimento = DateTime.Now;
    
            ctrlFormaPagto1.CampoValorConta = lblTotal;
            ctrlFormaPagto1.CampoValorDesconto = txtDesconto;

            if (IsFinanceiroPagto())
                ctrlFormaPagto1.MetodoFormasPagto = "ObtemParaQuitarChequeProprioDevolvido";
            else
                ctrlFormaPagto1.MetodoFormasPagto = "GetForQuitarChequeDev";
        }
    
        protected bool IsQuitarReapresentados()
        {
            return Request["reapresentados"] == "1";
        }
    
        protected bool IsFinanceiroPagto()
        {
            return Request["pagto"] == "1";
        }

        /// <summary>
        /// Atualiza os pagamentos feitos com o cappta tef
        /// </summary>
        /// <param name="id"></param>
        /// <param name="checkoutGuid"></param>
        /// <param name="admCodes"></param>
        /// <param name="customerReceipt"></param>
        /// <param name="merchantReceipt"></param>
        /// <param name="formasPagto"></param>
        [Ajax.AjaxMethod]
        public void AtualizaPagamentos(string id, string checkoutGuid, string admCodes, string customerReceipt, string merchantReceipt, string formasPagto)
        {
            TransacaoCapptaTefDAO.Instance.AtualizaPagamentosCappta(Data.Helper.UtilsFinanceiro.TipoReceb.ChequeDevolvido, id.StrParaInt(),
                checkoutGuid, admCodes, customerReceipt, merchantReceipt, formasPagto);
        }

        /// <summary>
        /// Cancela o pagto que foi pago com TEF porem deu algum erro
        /// </summary>
        /// <param name="id"></param>
        /// <param name="motivo"></param>
        [Ajax.AjaxMethod]
        public void CancelarAcertoChequeErroTef(string id, string motivo)
        {
            AcertoChequeDAO.Instance.CancelarAcertoCheque(id.StrParaUint(), "Falha no recebimento TEF. Motivo: " + motivo, DateTime.Now, true, false);
        }
    }
}
