using System;
using System.Web.UI.WebControls;
using Glass.Data.DAL;

namespace Glass.UI.Web.Cadastros
{
    public partial class CadTransfCxGeralParaContaBancaria : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                ((TextBox)ctrlData.FindControl("txtData")).Text = DateTime.Now.ToString("dd/MM/yyyy");
                ((ImageButton)ctrlData.FindControl("imgData")).Visible = true;
            }

            Ajax.Utility.RegisterTypeForAjax(typeof(Glass.UI.Web.Cadastros.CadTransfCxGeralParaContaBancaria));
        }
    
        [Ajax.AjaxMethod()]
        public string Transferir(string idContaBancoStr, string valorStr, string dataStr, string obs)
        {
            try
            {
                var idContaBanco = Conversoes.StrParaInt(idContaBancoStr);
                var valor = Conversoes.StrParaDecimal(valorStr);
                var data = DateTime.Parse(dataStr);
                
                CaixaGeralDAO.Instance.TransferenciaParaContaBancaria(idContaBanco, valor, data, obs);

                return "Ok\tTransferência efetuada.";
            }
            catch (Exception ex)
            {
                return Glass.MensagemAlerta.FormatErrorMsg("Erro\tFalha ao transferir valor.", ex);
            }
        }
    }
}
