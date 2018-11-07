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
        private readonly Jobs.MonitorIndicadoresFinanceirosJob indicadoresFinanceirosJob;

        /// <summary>
        /// Inicia uma nova instância da classe <see cref="IntegradorScheculerRegistry"/>.
        /// </summary>
        /// <param name="integrador">Integrador que será configurador.</param>
        public IntegradorScheculerRegistry(IntegradorKhan integrador)
        {
            this.indicadoresFinanceirosJob = new Jobs.MonitorIndicadoresFinanceirosJob(integrador.MonitorIndicadoresFinanceiros, integrador.Logger);
            this.indicadoresFinanceirosJob.Schedule = this.Schedule(this.indicadoresFinanceirosJob)
                .WithName("KhanMonitorIndicadoresFinanceiros");
            this.indicadoresFinanceirosJob.Schedule.NonReentrant().ToRunNow().AndEvery(5).Minutes();
        }

        /// <summary>
        /// Obtém os jobs.
        /// </summary>
        public IEnumerable<IJobIntegracao> Jobs
        {
            get
            {
                yield return this.indicadoresFinanceirosJob;
            }
        }
    }
}
