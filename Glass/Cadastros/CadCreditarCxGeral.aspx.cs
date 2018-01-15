using System;
using Glass.Data.Helper;
using Glass.Data.DAL;

namespace Glass.UI.Web.Cadastros
{
    public partial class CadCreditarCxGeral : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            Ajax.Utility.RegisterTypeForAjax(typeof(Glass.UI.Web.Cadastros.CadCreditarCxGeral));
        }
    
        [Ajax.AjaxMethod()]
        public string Transferir(string idConta, string valor, string formaEntrada, string obs)
        {
            uint idCxGeral = 0;
    
            try
            {
                uint tipoFunc = UserInfo.GetUserInfo.TipoUsuario;
    
                if (!Config.PossuiPermissao(Config.FuncaoMenuFinanceiro.ControleFinanceiroRecebimento))
                    return "Erro\tApenas funcionário Financeiro pode efetuar créditos no Caixa Geral.";
    
                idCxGeral = CaixaGeralDAO.Instance.MovCxCredito(Glass.Conversoes.StrParaUint(idConta), 1, Glass.Conversoes.StrParaInt(formaEntrada), decimal.Parse(valor), true, obs, null);
    
                return "Ok\tCrédito efetuado com sucesso.";
            }
            catch (Exception ex)
            {
                if (idCxGeral > 0)
                    CaixaGeralDAO.Instance.DeleteByPrimaryKey(idCxGeral);
    
                return Glass.MensagemAlerta.FormatErrorMsg("Erro\tFalha ao efetuar crédito.", ex);
            }
        }
    }
}
