using System;
using System.Globalization;
using Glass.Data.DAL;

namespace Glass.UI.Web.Cadastros
{
    public partial class CadQuitarDebitoFunc : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            Ajax.Utility.RegisterTypeForAjax(typeof(CadQuitarDebitoFunc));
        }
    
        protected void ctrlFormaPagto1_Load(object sender, EventArgs e)
        {
            var fp = (Controls.ctrlFormaPagto)sender;
            fp.CampoValorConta = hdfValorDebito;
        }
    
        [Ajax.AjaxMethod]
        public string GetDadosFuncionario(string idFuncStr)
        {
            try
            {
                var idFunc = idFuncStr.StrParaUint();
                var debito = MovFuncDAO.Instance.GetSaldo(idFunc);
    
                return "Ok;" + Math.Abs(debito).ToString(CultureInfo.InvariantCulture);
            }
            catch (Exception ex)
            {
                return "Erro;" + MensagemAlerta.FormatErrorMsg("Falha ao recuperar os dados do débito.", ex);
            }
        }
    
        [Ajax.AjaxMethod]
        public string Confirmar(string idFuncStr, string valoresStr, string formasPagtoStr, string tiposCartaoStr,
            string parcelasCartaoStr, string contasBancoStr, string depositoNaoIdentificado, string cartaoNaoIdentificado, string tiposBoletoStr, string taxaAntecipStr, 
            string numAutConstrucard, string recebimentoParcial, string gerarCredito, string chequesPagto, string obs)
        {
            try
            {
                FilaOperacoes.QuitarDebitoFuncionario.AguardarVez();
                var idFunc = idFuncStr.StrParaUint();

                var v = valoresStr.Split(';');
                var fp = formasPagtoStr.Split(';');
                var tc = tiposCartaoStr.Split(';');
                var pc = parcelasCartaoStr.Split(';');
                var cb = contasBancoStr.Split(';');
                var tb = tiposBoletoStr.Split(';');
                var ta = taxaAntecipStr.Split(';');
                var sDepositoNaoIdentificado = depositoNaoIdentificado.Split(';');
                var sCartaoNaoIdentificado = cartaoNaoIdentificado.Split(';');

                var valores = new decimal[v.Length];
                var formasPagto = new uint[fp.Length];
                var tiposCartao = new uint[tc.Length];
                var parcelasCartao = new uint[pc.Length];
                var contasBanco = new uint[cb.Length];
                var tiposBoleto = new uint[tb.Length];
                var taxasAntecip = new decimal[ta.Length];
                var depNaoIdentificado = new uint[sDepositoNaoIdentificado.Length];
                var cartNaoIdentificado = new uint[sCartaoNaoIdentificado.Length];

                for (var i = 0; i < valores.Length; i++)
                {
                    valores[i] = !string.IsNullOrEmpty(v[i]) ? decimal.Parse(v[i]) : 0;
                    formasPagto[i] = !string.IsNullOrEmpty(fp[i]) ? fp[i].StrParaUint() : 0;
                    tiposCartao[i] = !string.IsNullOrEmpty(tc[i]) ? tc[i].StrParaUint() : 0;
                    parcelasCartao[i] = !string.IsNullOrEmpty(pc[i]) ? pc[i].StrParaUint() : 0;
                    contasBanco[i] = !string.IsNullOrEmpty(cb[i]) ? cb[i].StrParaUint() : 0;
                    tiposBoleto[i] = !string.IsNullOrEmpty(tb[i]) ? tb[i].StrParaUint() : 0;
                    taxasAntecip[i] = !string.IsNullOrEmpty(ta[i]) ? decimal.Parse(ta[i]) : 0;
                    depNaoIdentificado[i] = !string.IsNullOrEmpty(sDepositoNaoIdentificado[i])
                        ? Convert.ToUInt32(sDepositoNaoIdentificado[i])
                        : 0;                    
                }

                for (int i = 0; i < sCartaoNaoIdentificado.Length; i++)
                {
                    cartNaoIdentificado[i] = !string.IsNullOrEmpty(sCartaoNaoIdentificado[i]) ? Convert.ToUInt32(sCartaoNaoIdentificado[i]) : 0;
                }

                return MovFuncDAO.Instance.Quitar(idFunc, valores, formasPagto, tiposCartao, parcelasCartao, contasBanco,
                    depNaoIdentificado, cartNaoIdentificado, tiposBoleto, taxasAntecip,
                    numAutConstrucard, recebimentoParcial == "true", gerarCredito == "true", chequesPagto, obs);
            }
            finally
            {
                Glass.FilaOperacoes.QuitarDebitoFuncionario.ProximoFila();
            }
        }
    }
}
