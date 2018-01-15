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
        public string Retirar(string idFornec, string idConta, string valor, string formaSaidaParam, string idCheque, string obs)
        {
            uint idCxGeral = 0;
    
            try
            {
                uint tipoFunc = UserInfo.GetUserInfo.TipoUsuario;
    
                if (!Config.PossuiPermissao(Config.FuncaoMenuFinanceiro.ControleFinanceiroRecebimento))
                    throw new Exception("Apenas funcionário Financeiro pode efetuar retiradas do Caixa Geral.");
    
                // Busca o saldo do caixa geral da forma de pagamento selecionada
                decimal valorMov = decimal.Parse(valor);
                int formaSaida = Glass.Conversoes.StrParaInt(formaSaidaParam);
                decimal saldo = CaixaGeralDAO.Instance.GetSaldoByFormaPagto(formaSaida == 1 ? Glass.Data.Model.Pagto.FormaPagto.Dinheiro : 
                    Glass.Data.Model.Pagto.FormaPagto.ChequeProprio, 0, null, null, 1, 0);
    
                // Verifica se o caixa possui saldo para realizar esta retirada
                if (saldo - valorMov < 0 && formaSaida == (int)CaixaGeral.FormaSaidaEnum.Dinheiro)
                    return "Erro\tNão há saldo suficiente em " + (formaSaida == 1 ? "dinheiro" : "cheque") + " para realizar esta retirada.";
    
                idCxGeral = CaixaGeralDAO.Instance.MovCxDebito(null, Glass.Conversoes.StrParaUintNullable(idFornec), formaSaida == 2 ? Glass.Conversoes.StrParaUintNullable(idCheque) : null, 
                    Glass.Conversoes.StrParaUint(idConta), 2, formaSaida, valorMov, true, obs, null);
    
                if (Glass.Conversoes.StrParaUint(idCheque) > 0)
                    ChequesDAO.Instance.UpdateSituacao(null, idCheque, Cheques.SituacaoCheque.Compensado);
    
                return "Ok\tRetirada efetuada com sucesso.";
            }
            catch (Exception ex)
            {
                if (idCxGeral > 0)
                    CaixaGeralDAO.Instance.DeleteByPrimaryKey(idCxGeral);
    
                return Glass.MensagemAlerta.FormatErrorMsg("Erro\tFalha ao efetuar retirada.", ex);
            }
        }
    }
}
