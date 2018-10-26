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
            this.indicadoresFinanceirosJob.Schedule = this
                .Schedule(this.indicadoresFinanceirosJob)
                .WithName("KhanMonitorIndicadoresFinanceiros");
            this.indicadoresFinanceirosJob.Schedule.ToRunNow().AndEvery(5).Minutes();
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

        private sealed class MonitorIndicadoresFinanceirosJob : FluentScheduler.IJob, IJobIntegracao
        {
            private readonly MonitorIndicadoresFinanceiros monitor;
            private readonly Colosoft.Logging.ILogger logger;

            public MonitorIndicadoresFinanceirosJob(MonitorIndicadoresFinanceiros monitor, Colosoft.Logging.ILogger logger)
            {
                this.monitor = monitor;
                this.logger = logger;
            }

            public FluentScheduler.Schedule Schedule { get; set; }

            /// <inheritdoc />
            public string Nome => Schedule.Name;

            /// <inheritdoc />
            public string Descricao => "Monitor dos indicadores financeiros";

            /// <inheritdoc />
            public Exception UltimaFalha { get; private set; }

            /// <inheritdoc />
            public DateTime? UltimaExecucaoComFalha { get; private set; }

            /// <inheritdoc />
            public SituacaoJobIntegracao Situacao { get; private set; } = SituacaoJobIntegracao.NaoIniciado;

            /// <inheritdoc />
            public DateTime? UltimaExecucao { get; private set; }

            /// <inheritdoc />
            public DateTime ProximaExecucao => this.Schedule.NextRun;

            /// <inheritdoc />
            public void Executar() => this.Schedule.Execute();

            public void Execute()
            {
                this.Situacao = SituacaoJobIntegracao.Executando;
                try
                {
                    this.monitor.ImportarIndicadores();
                    this.Situacao = SituacaoJobIntegracao.Executado;
                }
                catch (Exception ex)
                {
                    this.UltimaExecucaoComFalha = DateTime.Now;
                    this.UltimaFalha = ex;
                    this.logger.Error("Falha ao importer indicadores financeiros da khan.".GetFormatter(), ex);
                }
                finally
                {
                    this.UltimaExecucao = DateTime.Now;
                }
            }
        }
    }
}
