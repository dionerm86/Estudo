using System;
using Glass.Data.Model;
using Glass.Data.DAL;
using Glass.Data.Helper;

namespace Glass.UI.Web.Utils
{
    public partial class LocalizacaoCheque : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            Cheques cheque = ChequesDAO.Instance.GetElementByPrimaryKey(Glass.Conversoes.StrParaUint(Request["IdCheque"]));
            string localizacao = String.Empty;
    
            if (cheque.Origem == (int)Cheques.OrigemCheque.FinanceiroPagto)
                localizacao += "Cheque avulso. ";
    
            localizacao += "Cadastrado em: " + cheque.DataCad + ", por " + FuncionarioDAO.Instance.GetNome(cheque.Usucad) + " ";
    
            // Verifica se este cheque foi utilizado em algum dep�sito
            if (cheque.IdDeposito > 0)
            {
                DepositoCheque deposito = DepositoChequeDAO.Instance.GetElement(cheque.IdDeposito.Value);

                if (deposito == null)
                    localizacao += "Dep�sito n�o localizado.";
                else
                    localizacao += "Este cheque foi utilizado no dep�sito n�mero " + cheque.IdDeposito +
                        ". O dep�sito foi efetuado na conta banc�ria: " + deposito.DescrContaBanco + ", no dia " +
                        deposito.DataDeposito.ToString("dd/MM/yyyy") + " por " + deposito.NomeFuncDeposito + ". ";
            }
            
            if (cheque.IdAcertoCheque > 0)
            {
                AcertoCheque acertoCheque = AcertoChequeDAO.Instance.GetElement(cheque.IdAcertoCheque.Value);
    
                localizacao += "Este cheque foi utilizado no acerto de cheque n�mero " + cheque.IdAcertoCheque +
                    ". O acerto de cheque foi feito no dia " + acertoCheque.DataAcerto.ToString("dd/MM/yyyy") + 
                    " por " + acertoCheque.NomeFunc + ". ";
            }
            else
            {
                // Verifica se o cheque foi utilizado em algum pagamento
                uint idPagto = PagtoChequeDAO.Instance.GetPagtoByCheque(cheque.IdCheque);
    
                if (idPagto > 0)
                {
                    Pagto pagto = PagtoDAO.Instance.GetPagto(idPagto);
    
                    localizacao += "Este cheque foi utilizado no pagamento n�mero " + idPagto +
                        ". O pagamento foi efetuado no dia " + pagto.DataPagto.ToString("dd/MM/yyyy") +
                        " por " + pagto.NomeFuncPagto + ". ";
    
                    if (pagto.IdFornec > 0)
                        localizacao += "Fornecedor pago: " + pagto.NomeFornec + ". ";
    
                    if (cheque.DataReceb != null)
                        localizacao += "Este cheque foi compensado no dia " + cheque.DataReceb.Value.ToString("dd/MM/yyyy") + ". ";
                }
                else if (cheque.IdDeposito == 0)
                    localizacao += "Localiza��o do cheque desconhecida. ";
            }
    
            if (cheque.Situacao == (int)Cheques.SituacaoCheque.Trocado)
            {
                // Verifica se h� alguma movimenta��o no caixa geral de quita��o deste cheque
                CaixaGeral[] lstCxGeral = CaixaGeralDAO.Instance.GetByCheque(cheque.IdCheque);
    
                localizacao += "Este cheque foi trocado";
    
                foreach (CaixaGeral c in lstCxGeral)
                {
                    if (c.IdConta == UtilsPlanoConta.GetPlanoConta(UtilsPlanoConta.PlanoContas.RecChequeDevDinheiro))
                        localizacao += " por " + c.ValorMov.ToString("C") + " em dinheiro,";
                    else if (c.IdConta == UtilsPlanoConta.GetPlanoConta(UtilsPlanoConta.PlanoContas.RecChequeDevCartao))
                        localizacao += " por " + c.ValorMov.ToString("C") + " pago no cart�o,";
                    else if (c.IdConta == UtilsPlanoConta.GetPlanoConta(UtilsPlanoConta.PlanoContas.RecChequeDevDeposito))
                        localizacao += " por " + c.ValorMov.ToString("C") + " pago por dep�sito em conta,";
                    else if (c.IdConta == UtilsPlanoConta.GetPlanoConta(UtilsPlanoConta.PlanoContas.RecChequeDevCheque))
                        localizacao += " por um cheque no valor de " + c.ValorMov.ToString("C") + ",";
                }
    
                localizacao = localizacao.TrimEnd(',') + ".";
            }
            else if (cheque.Situacao == (int)Cheques.SituacaoCheque.Quitado)
            {
                // Verifica se h� alguma movimenta��o no caixa geral de quita��o deste cheque
                CaixaGeral[] lstCxGeral = CaixaGeralDAO.Instance.GetByCheque(cheque.IdCheque);
    
                localizacao += "Este cheque foi quitado";
    
                var idsAcertoCheque = ItemAcertoChequeDAO.Instance.GetIdsAcertoByCheque(null, cheque.IdCheque, true);
                if (!String.IsNullOrEmpty(idsAcertoCheque))
                    localizacao += " no(s) acerto(s) de cheques " + idsAcertoCheque;
    
                foreach (CaixaGeral c in lstCxGeral)
                {
                    if (c.IdConta == UtilsPlanoConta.GetPlanoConta(UtilsPlanoConta.PlanoContas.RecChequeDevDinheiro))
                        localizacao += " por " + c.ValorMov.ToString("C") + " em dinheiro,";
                    else if (c.IdConta == UtilsPlanoConta.GetPlanoConta(UtilsPlanoConta.PlanoContas.RecChequeDevCartao))
                        localizacao += " por " + c.ValorMov.ToString("C") + " pago no cart�o,";
                    else if (c.IdConta == UtilsPlanoConta.GetPlanoConta(UtilsPlanoConta.PlanoContas.RecChequeDevDeposito))
                        localizacao += " por " + c.ValorMov.ToString("C") + " pago por dep�sito em conta,";
                    else if (c.IdConta == UtilsPlanoConta.GetPlanoConta(UtilsPlanoConta.PlanoContas.RecChequeDevCheque))
                        localizacao += " por um cheque no valor de " + c.ValorMov.ToString("C") + ",";
                }
    
                localizacao = localizacao.TrimEnd(',') + ".";
            }
    
            lblLocalizacao.Text = localizacao;
        }
    }
}
