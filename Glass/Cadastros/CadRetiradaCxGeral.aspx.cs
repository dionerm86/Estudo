using System;
using Glass.Data.Helper;
using Glass.Data.DAL;
using Glass.Data.Model;
using Glass.Configuracoes;

namespace Glass.UI.Web.Cadastros
{
    public partial class CadRetiradaCxGeral : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            Ajax.Utility.RegisterTypeForAjax(typeof(MetodosAjax));
            Ajax.Utility.RegisterTypeForAjax(typeof(Glass.UI.Web.Cadastros.CadRetiradaCxGeral));
        }
    
        [Ajax.AjaxMethod()]
        public string Retirar(string idFornec, string idConta, string valor, string formaSaida, string idCheque, string obs)
        {
            try
            {
                if (!Config.PossuiPermissao(Config.FuncaoMenuFinanceiro.ControleFinanceiroRecebimento))
                    throw new Exception("Apenas funcionário Financeiro pode efetuar retiradas do Caixa Geral.");

                CaixaGeralDAO.Instance.RetirarValorCaixaGeral(idFornec.StrParaIntNullable(), idConta.StrParaInt(), idCheque.StrParaIntNullable(), valor.StrParaDecimalNullable().GetValueOrDefault(),
                    formaSaida.StrParaInt(), obs);
    
                return "Ok\tRetirada efetuada com sucesso.";
            }
            catch (Exception ex)
            {
                return Glass.MensagemAlerta.FormatErrorMsg("Erro\tFalha ao efetuar retirada.", ex);
            }
        }
    }
}
