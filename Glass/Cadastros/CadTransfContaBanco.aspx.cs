using System;
using System.Web.UI.WebControls;
using Glass.Data.Helper;
using Glass.Data.DAL;

namespace Glass.UI.Web.Cadastros
{
    public partial class CadTransfContaBanco : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                ((TextBox)ctrlData.FindControl("txtData")).Text = DateTime.Now.ToString("dd/MM/yyyy");
                ((ImageButton)ctrlData.FindControl("imgData")).Visible = true;
            }
    
            Ajax.Utility.RegisterTypeForAjax(typeof(Glass.UI.Web.Cadastros.CadTransfContaBanco));
        }
    
        [Ajax.AjaxMethod()]
        public string Transferir(string idContaBancoOrigem, string idContaBancoDest, string valor, string data, string obs)
        {
            try
            {
                uint idContaOrigem = Glass.Conversoes.StrParaUint(idContaBancoOrigem);
                uint idContaDestino = Glass.Conversoes.StrParaUint(idContaBancoDest);
                decimal valorMov = decimal.Parse(valor);
                DateTime dataMov = DateTime.Parse(data);
    
                // Manter esta ordem, retira da conta origem e credita na conta destino, pois caso precise excluir, 
                // será feito também nesta ordem
    
                // Debita valor da conta origem
                ContaBancoDAO.Instance.MovContaTransfConta(idContaOrigem, UtilsPlanoConta.GetPlanoConta(UtilsPlanoConta.PlanoContas.TransfContaBancaria),
                    (int)UserInfo.GetUserInfo.IdLoja, idContaDestino, 2, valorMov, dataMov, obs);
    
                // Credita valor na conta destino
                ContaBancoDAO.Instance.MovContaTransfConta(idContaDestino, UtilsPlanoConta.GetPlanoConta(UtilsPlanoConta.PlanoContas.TransfContaBancaria),
                    (int)UserInfo.GetUserInfo.IdLoja, idContaOrigem, 1, valorMov, dataMov, obs);
    
                return "Ok\tTransferência efetuada.";
            }
            catch (Exception ex)
            {
                return Glass.MensagemAlerta.FormatErrorMsg("Erro\tFalha ao transferir valor entre contas bancárias.", ex);
            }
        }
    }
}
