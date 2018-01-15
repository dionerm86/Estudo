using System;
using System.Web.UI.WebControls;
using Glass.Data.Helper;
using Glass.Data.DAL;

namespace Glass.UI.Web.Cadastros
{
    public partial class CadTransfContaBancariaParaCxGeral : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                ((TextBox)ctrlData.FindControl("txtData")).Text = DateTime.Now.ToString("dd/MM/yyyy");
                ((ImageButton)ctrlData.FindControl("imgData")).Visible = true;
            }
    
            Ajax.Utility.RegisterTypeForAjax(typeof(Glass.UI.Web.Cadastros.CadTransfContaBancariaParaCxGeral));
        }
    
        [Ajax.AjaxMethod()]
        public string Transferir(string idContaBanco, string valor, string data, string obs)
        {
            try
            {
                decimal valorMov = decimal.Parse(valor);
                DateTime dataMov = DateTime.Parse(data);

                MovBancoDAO.Instance.TransferirCxGeral(idContaBanco.StrParaInt(), valorMov, dataMov, 2, obs);
        
                return "Ok\tTransferência efetuada.";
            }
            catch (Exception ex)
            {
                return Glass.MensagemAlerta.FormatErrorMsg("Erro\tFalha ao transferir valor.", ex);
            }
        }
    }
}
