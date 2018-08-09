using System;
using Glass.Data.Helper;
using Glass.Data.DAL;
using GDA;

namespace Glass.UI.Web.Cadastros
{
    public partial class CadTrasnfCxGeralParaCxDiario : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            Ajax.Utility.RegisterTypeForAjax(typeof(Glass.UI.Web.Cadastros.CadTrasnfCxGeralParaCxDiario));
        }
    
        [Ajax.AjaxMethod()]
        public string Transferir(string idLoja, string valor, string obs)
        {
            if (!Config.PossuiPermissao(Config.FuncaoMenuFinanceiro.ControleFinanceiroRecebimento))
                throw new Exception("Apenas funcionário Financeiro pode efetuar transferência para o Caixa Diário.");

            using (var transacao = new GDATransaction())
            {
                transacao.BeginTransaction();

                try
                {
                    decimal valorTransf = decimal.Parse(valor);

                    CaixaDiarioDAO.Instance.MovCaixa(transacao, Glass.Conversoes.StrParaUint(idLoja), null, 1, valorTransf, 0,
                        UtilsPlanoConta.GetPlanoConta(UtilsPlanoConta.PlanoContas.TransfCaixaGeralParaDiario), null, 1, obs, true, false);

                    // Movimenta caixa geral
                    CaixaGeralDAO.Instance.MovCxGeral(transacao, null, null, null, UtilsPlanoConta.GetPlanoConta(UtilsPlanoConta.PlanoContas.TransfCaixaGeralParaDiario),
                        2, 1, valorTransf, 0, null, true, obs, DateTime.Now);

                    transacao.Commit();

                    return "Ok\tTransferência efetuada com sucesso.";
                }
                catch (Exception ex)
                {
                    transacao.Rollback();
                    transacao.Close();

                    return Glass.MensagemAlerta.FormatErrorMsg("Erro\tFalha ao efetuar retirada.", ex);
                }
            }
        }
    }
}
