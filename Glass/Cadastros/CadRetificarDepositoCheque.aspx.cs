using System;
using Glass.Data.DAL;
using Glass.Data.Model;
using System.Text;

namespace Glass.UI.Web.Cadastros
{
    public partial class CadRetificarDepositoCheque : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            Ajax.Utility.RegisterTypeForAjax(typeof(Glass.UI.Web.Cadastros.CadRetificarDepositoCheque));
        }
    
        [Ajax.AjaxMethod()]
        public string GetCheques(string idDeposito, string noCache)
        {
            try
            {
                // Verifica se depósito passado existe
                if (!DepositoChequeDAO.Instance.DepositoExists(Glass.Conversoes.StrParaUint(idDeposito)))
                    return "Erro\tDepósito informado não existe.";
    
                var deposito = DepositoChequeDAO.Instance.GetElementByPrimaryKey(Glass.Conversoes.StrParaUint(idDeposito));
    
                // Busca os cheques deste depósito
                var lstCheques = ChequesDAO.Instance.GetByDeposito(Glass.Conversoes.StrParaUint(idDeposito));
    
                var str = new StringBuilder();
    
                // Inclui dados do depósito no retorno
                str.Append(deposito.IdContaBanco + ";");
                str.Append(deposito.DataDeposito.ToString("dd/MM/yyyy") + "\n");
    
                // Inclui dados dos cheques deste depósito no retorno
                foreach (Cheques c in lstCheques)
                {
                    str.Append(c.IdCheque + ";");
                    str.Append(c.Num + ";");
                    str.Append(c.Titular.Replace("'", "").Replace("|", "").Replace(";", "") + ";");
                    str.Append(c.Banco.Replace("'", "").Replace("|", "").Replace(";", "") + ";");
                    str.Append(c.Agencia.Replace("'", "").Replace("|", "").Replace(";", "") + ";");
                    str.Append(c.Conta.Replace("'", "").Replace("|", "").Replace(";", "") + ";");
                    str.Append(c.Valor.ToString("c") + ";");
                    str.Append(c.DataVenc != null ? c.DataVenc.Value.ToString("dd/MM/yyyy") : String.Empty);
                    str.Append("|");
                }
    
                return lstCheques.Count == 0 ? "Erro\tNenhum cheque encontrado para o depósito informado." : 
                    "ok\t" + str.ToString().TrimEnd('|');
            }
            catch (Exception ex)
            {
                return "Erro\t" + Glass.MensagemAlerta.FormatErrorMsg("Falha ao buscar cheques.", ex);
            }
        }
    
        [Ajax.AjaxMethod()]
        public string Retificar(string idDeposito, string idsChequeNovos, string idContaBanco, string dataDeposito, string taxaAntecip, string valorDeposito, string noCache)
        {
            try
            {
                decimal valorDepositoDec = decimal.Parse(valorDeposito.Replace("R$", "").Replace(" ", "").Replace(".", ""));
                decimal taxaAntecipDec = Glass.Conversoes.StrParaDecimal(taxaAntecip);
    
                DepositoChequeDAO.Instance.RetificarDeposito(Glass.Conversoes.StrParaUint(idDeposito), idsChequeNovos, Glass.Conversoes.StrParaUint(idContaBanco), 
                    DateTime.Parse(dataDeposito), valorDepositoDec - taxaAntecipDec, taxaAntecipDec);
    
                return "ok\tok";
            }
            catch (Exception ex)
            {
                return Glass.MensagemAlerta.FormatErrorMsg("Erro\t", ex);
            }
        }
    }
}
