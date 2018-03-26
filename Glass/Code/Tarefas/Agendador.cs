using Quartz;
using Quartz.Impl;

namespace Glass.UI.Web.Code.Tarefas
{
    public class Agendador
    {
        public static void IniciarCopiaContasPagarReceber()
        {
            // Verifica se a cópia das contas a receber/pagar deverão ser copiadas
            // para as tabelas contas_receber_data_base e contas_pagar_data_base.
            /* Chamado 53735. */
            if (string.IsNullOrWhiteSpace(System.Configuration.ConfigurationManager.AppSettings["UtilizarPosicaoContasReceberPagarPorDataBase"]) ||
                System.Configuration.ConfigurationManager.AppSettings["UtilizarPosicaoContasReceberPagarPorDataBase"].ToLower() != "true")
                return;

            IScheduler scheduler = StdSchedulerFactory.GetDefaultScheduler();
            scheduler.Start();

            IJobDetail job = JobBuilder.Create<ContasReceberPagarBase>().Build();

            ITrigger trigger = TriggerBuilder.Create()
                .WithDailyTimeIntervalSchedule
                  (s =>
                     s.WithIntervalInHours(24)
                    .OnEveryDay()
                    .StartingDailyAt(TimeOfDay.HourAndMinuteOfDay(21, 0))
                  )
                .Build();

            scheduler.ScheduleJob(job, trigger);
        }
    }
}