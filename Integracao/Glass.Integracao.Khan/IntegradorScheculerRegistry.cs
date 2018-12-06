// <copyright file="IntegradorScheculerRegistry.cs" company="Sync Softwares">
// Copyright (c) Sync Softwares. Todos os direitos reservados.
// </copyright>

using Colosoft;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Glass.Integracao.Khan
{
    /// <summary>
    /// Representa o registro do agendador de tarefas do integrador.
    /// </summary>
    internal class IntegradorScheculerRegistry : FluentScheduler.Registry
    {
        /// <summary>
        /// Inicia uma nova instância da classe <see cref="IntegradorScheculerRegistry"/>.
        /// </summary>
        /// <param name="integrador">Integrador que será configurador.</param>
        public IntegradorScheculerRegistry(IntegradorKhan integrador)
        {
            var indicadoresFinanceirosJob = new Jobs.MonitorIndicadoresFinanceirosJob(integrador.MonitorIndicadoresFinanceiros, integrador.Logger);
            indicadoresFinanceirosJob.Schedule = this.Schedule(indicadoresFinanceirosJob)
                .WithName("KhanMonitorIndicadoresFinanceiros");
            indicadoresFinanceirosJob.Schedule.NonReentrant().ToRunNow().AndEvery(5).Minutes();

            var notaFiscalJob = new Jobs.MonitorNotaFiscalJob(integrador.MonitorNotaFiscal, integrador.Logger);
            notaFiscalJob.Schedule = this.Schedule(notaFiscalJob)
                .WithName("KhanMonitorNotaFiscal");
            notaFiscalJob.Schedule.NonReentrant().ToRunNow().AndEvery(15).Seconds();

            var sincronizarTodosProdutosJob = new Jobs.SinronizadorTodosProdutosJob(integrador.MonitorProdutos, integrador.Logger);
            var monitorCondicoesPagamento = new Jobs.MonitorCondicoesPagamentoJob(integrador.MonitorCondicoesPagamento, integrador.Logger);
            monitorCondicoesPagamento.Schedule = this.Schedule(notaFiscalJob)
                .WithName("KhanMonitorCondicoesPagamento");
            notaFiscalJob.Schedule.NonReentrant().ToRunNow().AndEvery(5).Minutes();

            this.Jobs = new IJobIntegracao[]
            {
                indicadoresFinanceirosJob,
                notaFiscalJob,
                sincronizarTodosProdutosJob,
                monitorCondicoesPagamento,
            };
        }

        /// <summary>
        /// Obtém os jobs.
        /// </summary>
        public IEnumerable<IJobIntegracao> Jobs { get; }
    }
}
