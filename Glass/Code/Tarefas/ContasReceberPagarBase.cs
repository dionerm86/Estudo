using Glass.Data.DAL;
using Quartz;
using System;

namespace Glass.UI.Web.Code.Tarefas
{
    public class ContasReceberPagarBase : IJob
    {
        public void Execute(IJobExecutionContext context)
        {
            #region Contas a receber

            var i = 0;

            while (i < 3)
            {
                try
                {
                    ContasReceberDAO.Instance.InserirContasReceberNoHistoricoDeContasAReceber();
                    break;
                }
                catch (Exception ex)
                {
                    i++;

                    if (i == 3)
                        ErroDAO.Instance.InserirFromException("Falha ao copiar contas a receber para a tabela contas_receber_data_base.", ex);

                    Thread.Sleep(10 * 1000);
                }
            }

            #endregion

            #region Contas a pagar

            i = 0;

            while (i < 3)
            {
                try
                {
                    ContasPagarDAO.Instance.InserirContasPagarNoHistoricoDeContasAPagar();
                    break;
                }
                catch (Exception ex)
                {
                    i++;

                    if (i == 3)
                        ErroDAO.Instance.InserirFromException("Falha ao copiar contas a pagar para a tabela contas_pagar_data_base.", ex);

                    Thread.Sleep(10 * 1000);
                }
            }

            #endregion
        }
    }
}