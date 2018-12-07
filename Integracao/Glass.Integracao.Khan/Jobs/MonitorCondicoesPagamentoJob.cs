// <copyright file="MonitorCondicoesPagamentoJob.cs" company="Sync Softwares">
// Copyright (c) Sync Softwares. Todos os direitos reservados.
// </copyright>

using Colosoft;
using System;
using System.Threading.Tasks;

namespace Glass.Integracao.Khan.Jobs
{
    /// <summary>
    /// Repreesnta o job do monitor das condições de pagamento.
    /// </summary>
    internal sealed class MonitorCondicoesPagamentoJob : FluentScheduler.IJob, IJobIntegracao
    {
        private readonly MonitorCondicoesPagamento monitor;
        private readonly Colosoft.Logging.ILogger logger;

        /// <summary>
        /// Inicia uma nova instância da classe <see cref="MonitorCondicoesPagamentoJob"/>.
        /// </summary>
        /// <param name="monitor">Monitor das condições de pagamento.</param>
        /// <param name="logger">Logger.</param>
        public MonitorCondicoesPagamentoJob(MonitorCondicoesPagamento monitor, Colosoft.Logging.ILogger logger)
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
        public string Descricao => "Monitor das condições de pagamento";

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
                this.monitor.ImportarFormasPagamento();
                this.Situacao = SituacaoJobIntegracao.Executado;
            }
            catch (Exception ex)
            {
                this.Situacao = SituacaoJobIntegracao.Falha;
                this.UltimaExecucaoComFalha = DateTime.Now;
                this.UltimaFalha = ex;
                this.logger.Error("Falha ao importar as condições de pagamento da khan.".GetFormatter(), ex);
            }
            finally
            {
                this.UltimaExecucao = DateTime.Now;
            }
        }
    }
}
