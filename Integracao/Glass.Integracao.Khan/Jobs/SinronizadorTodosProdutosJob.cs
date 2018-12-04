// <copyright file="SinronizadorTodosProdutosJob.cs" company="Sync Softwares">
// Copyright (c) Sync Softwares. Todos os direitos reservados.
// </copyright>

using Colosoft;
using System;
using System.Threading.Tasks;

namespace Glass.Integracao.Khan.Jobs
{
    /// <summary>
    /// Representa o job para sincronizar todos os produtos.
    /// </summary>
    internal class SinronizadorTodosProdutosJob : IJobIntegracao
    {
        private readonly MonitorProdutos monitorProdutos;
        private readonly Colosoft.Logging.ILogger logger;

        /// <summary>
        /// Inicia uma nova instância da classe <see cref="SinronizadorTodosProdutosJob"/>.
        /// </summary>
        /// <param name="monitorProdutos">Monitor dos produtos.</param>
        /// <param name="logger">Logger.</param>
        public SinronizadorTodosProdutosJob(MonitorProdutos monitorProdutos, Colosoft.Logging.ILogger logger)
        {
            this.monitorProdutos = monitorProdutos;
            this.logger = logger;
        }

        /// <inheritdoc />
        public string Descricao => "Sincronizador de todos produtos";

        /// <inheritdoc />
        public string Nome => "KhanSincronizadorTodosProdutos";

        /// <inheritdoc />
        public DateTime ProximaExecucao => DateTime.Now.AddYears(1);

        /// <inheritdoc />
        public SituacaoJobIntegracao Situacao { get; set; }

        /// <inheritdoc />
        public DateTime? UltimaExecucao { get; set; }

        /// <inheritdoc />
        public DateTime? UltimaExecucaoComFalha { get; set; }

        /// <inheritdoc />
        public Exception UltimaFalha { get; set; }

        /// <inheritdoc />
        public Task Executar()
        {
            return Task.Run(() =>
            {
                try
                {
                    this.Situacao = SituacaoJobIntegracao.Executando;
                    this.monitorProdutos.SincronizarTodosProdutos();
                    this.Situacao = SituacaoJobIntegracao.Executado;
                }
                catch (Exception ex)
                {
                    this.Situacao = SituacaoJobIntegracao.Falha;
                    this.UltimaExecucaoComFalha = DateTime.Now;
                    this.UltimaFalha = ex;

                    this.logger.Error("Falha ao executar o sincronização de todos produtos.".GetFormatter(), ex);
                }
                finally
                {
                    this.UltimaExecucao = DateTime.Now;
                }
            });
        }
    }
}
