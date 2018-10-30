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
        private readonly MonitorIndicadoresFinanceirosJob indicadoresFinanceirosJob;

        /// <summary>
        /// Inicia uma nova instância da classe <see cref="IntegradorScheculerRegistry"/>.
        /// </summary>
        /// <param name="integrador">Integrador que será configurador.</param>
        public IntegradorScheculerRegistry(IntegradorKhan integrador)
        {
            this.indicadoresFinanceirosJob = new MonitorIndicadoresFinanceirosJob(integrador.MonitorIndicadoresFinanceiros, integrador.Logger);
            this.Schedule(this.indicadoresFinanceirosJob).ToRunNow().AndEvery(5).Minutes();
        }

        private class MonitorIndicadoresFinanceirosJob : FluentScheduler.IJob
        {
            private readonly MonitorIndicadoresFinanceiros monitor;
            private readonly Colosoft.Logging.ILogger logger;

            public MonitorIndicadoresFinanceirosJob(MonitorIndicadoresFinanceiros monitor, Colosoft.Logging.ILogger logger)
            {
                this.monitor = monitor;
                this.logger = logger;
            }

            public void Execute()
            {
                try
                {
                    this.monitor.ImportarIndicadores();
                }
                catch (Exception ex)
                {
                    this.logger.Error("Falha ao importer indicadores financeiros da khan.".GetFormatter(), ex);
                }
            }
        }
    }
}
