using System;
using System.Web.UI.WebControls;
using Glass.Data.Helper;
using Glass.Data.DAL;

namespace Glass.UI.Web.Cadastros
{
    public partial class CadCreditoContaBanco : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                ((TextBox)ctrlData.FindControl("txtData")).Text = DateTime.Now.ToString("dd/MM/yyyy");
                ((ImageButton)ctrlData.FindControl("imgData")).Visible = true;
            }
    
            Ajax.Utility.RegisterTypeForAjax(typeof(Glass.UI.Web.Cadastros.CadCreditoContaBanco));
        }
    
        [Ajax.AjaxMethod()]
        public string CreditarValor(string idConta, string idContaBanco, string valor, string data, string obs)
        {
            try
            {
                ContaBancoDAO.Instance.MovContaCredito(Glass.Conversoes.StrParaUint(idContaBanco), Glass.Conversoes.StrParaUint(idConta),
                    (int)UserInfo.GetUserInfo.IdLoja, 1, decimal.Parse(valor), DateTime.Parse(data), obs);
    
                return "Ok\tValor creditado.";
            }
            catch (Exception ex)
            {
                return Glass.MensagemAlerta.FormatErrorMsg("Erro\tFalha ao creditar valor.", ex);
            }
        }
    }
}
