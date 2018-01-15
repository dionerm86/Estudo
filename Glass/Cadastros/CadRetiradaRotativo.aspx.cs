using System;
using Glass.Data.Model;
using Glass.Data.Helper;
using Glass.Data.DAL;
using Glass.Configuracoes;

namespace Glass.UI.Web.Cadastros
{
    public partial class CadRetiradaRotativo : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!FinanceiroConfig.EfetuarRetiradaCaixaDiario)
                Response.Redirect("~/Webglass/Main.aspx");
    
            Ajax.Utility.RegisterTypeForAjax(typeof(Glass.UI.Web.Cadastros.CadRetiradaRotativo));
        }
    
        [Ajax.AjaxMethod()]
        public string Retirar(string idConta, string valor, string formaSaida, string idCheque, string obs)
        {
            try
            {
                CaixaDiarioDAO.Instance.RetirarValor((int)UserInfo.GetUserInfo.IdLoja, valor.StrParaDecimal(), idCheque.StrParaIntNullable(),
                    idConta.StrParaInt(), formaSaida.StrParaInt(), obs);

                return "Ok\tRetirada do caixa efetuada com sucesso.";
            }
            catch (Exception ex)
            {
                return Glass.MensagemAlerta.FormatErrorMsg("Erro\tFalha ao efetuar retirada.", ex);
            }
        }
    }
}
