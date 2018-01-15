using System;
using Glass.Data.DAL;

namespace Glass.UI.Web.Cadastros
{
    public partial class CadTransfCxGeral : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            Ajax.Utility.RegisterTypeForAjax(typeof(Glass.UI.Web.Cadastros.CadTransfCxGeral));
        }
    
        [Ajax.AjaxMethod()]
        public string Transferir(string valorParam, string formaSaida, string obs)
        {
            try
            {
                CaixaDiarioDAO.Instance.TransferirCxGeral(Conversoes.StrParaDecimal(valorParam), Conversoes.StrParaInt(formaSaida), obs);
                
                return "Ok\tTransferência efetuada com sucesso.";
            }
            catch (Exception ex)
            {    
                return Glass.MensagemAlerta.FormatErrorMsg("Erro\tFalha ao efetuar retirada.", ex);
            }
        }
    }
}
