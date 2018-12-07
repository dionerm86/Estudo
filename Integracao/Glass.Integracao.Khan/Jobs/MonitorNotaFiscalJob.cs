// <copyright file="MonitorNotaFiscalJob.cs" company="Sync Softwares">
// Copyright (c) Sync Softwares. Todos os direitos reservados.
// </copyright>

using Colosoft;
using System;
using System.Threading.Tasks;

namespace Glass.Integracao.Khan.Jobs
{
    /// <summary>
    /// Representa o Job do monitor das notas fiscais.
    /// </summary>
    internal class MonitorNotaFiscalJob : FluentScheduler.IJob, IJobIntegracao
    {
        private readonly MonitorNotaFiscal monitor;
        private readonly Colosoft.Logging.ILogger logger;

        /// <summary>
        /// Inicia uma nova instância da classe <see cref="MonitorNotaFiscalJob"/>.
        /// </summary>
        /// <param name="monitor">Monitor base.</param>
        /// <param name="logger">Logger.</param>
        public MonitorNotaFiscalJob(MonitorNotaFiscal monitor, Colosoft.Logging.ILogger logger)
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
        public string Descricao => "Monitor das notas fiscais";

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
        public Task Executar() => Task.Run(new Action(((FluentScheduler.IJob)this).Execute));

        /// <inheritdoc />
        void FluentScheduler.IJob.Execute()
        {
            this.Situacao = SituacaoJobIntegracao.Executando;
            try
            {
                this.monitor.SincronizarNotasFiscaisIntegrando();
                this.Situacao = SituacaoJobIntegracao.Executado;
            }
            catch (Exception ex)
            {
                this.Situacao = SituacaoJobIntegracao.Falha;
                this.UltimaExecucaoComFalha = DateTime.Now;
                this.UltimaFalha = ex;
                this.logger.Error("Falha ao sincronizar as notas fiscais que estão integrando na khan.".GetFormatter(), ex);
            }
            finally
            {
                this.UltimaExecucao = DateTime.Now;
            }
        }
    }
}
