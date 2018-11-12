// <copyright file="MonitorIndicadoresFinanceirosJob.cs" company="Sync Softwares">
// Copyright (c) Sync Softwares. Todos os direitos reservados.
// </copyright>

using Colosoft;
using System;

namespace Glass.Integracao.Khan.Jobs
{
    /// <summary>
    /// Representa o Job do monitor de indicadores financeiros.
    /// </summary>
    internal sealed class MonitorIndicadoresFinanceirosJob : FluentScheduler.IJob, IJobIntegracao
    {
        private readonly MonitorIndicadoresFinanceiros monitor;
        private readonly Colosoft.Logging.ILogger logger;

        /// <summary>
        /// Inicia uma nova instância da classe <see cref="MonitorIndicadoresFinanceirosJob"/>.
        /// </summary>
        /// <param name="monitor">Monitor base.</param>
        /// <param name="logger">Logger.</param>
        public MonitorIndicadoresFinanceirosJob(MonitorIndicadoresFinanceiros monitor, Colosoft.Logging.ILogger logger)
        {
            this.monitor = monitor;
            this.logger = logger;
        }

        /// <summary>
        /// Obtém ou define o agendador.
        /// </summary>
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

        /// <inheritdoc />
        void FluentScheduler.IJob.Execute()
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
