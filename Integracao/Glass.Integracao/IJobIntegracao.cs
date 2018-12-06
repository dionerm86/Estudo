// <copyright file="IJobIntegracao.cs" company="Sync Softwares">
// Copyright (c) Sync Softwares. Todos os direitos reservados.
// </copyright>

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Glass.Integracao
{
    /// <summary>
    /// Assinatura de um Join de integração.
    /// </summary>
    public interface IJobIntegracao
    {
        /// <summary>
        /// Obtém o nome do Job.
        /// </summary>
        string Nome { get; }

        /// <summary>
        /// Obtém a descrição do Job.
        /// </summary>
        string Descricao { get; }

        /// <summary>
        /// Obtém a situação da última execução do Job.
        /// </summary>
        SituacaoJobIntegracao Situacao { get; }

        /// <summary>
        /// Obtém última falha.
        /// </summary>
        Exception UltimaFalha { get; }

        /// <summary>
        /// Obtém a data da última execução com falha.
        /// </summary>
        DateTime? UltimaExecucaoComFalha { get; }

        /// <summary>
        /// Obtém a data da última execução.
        /// </summary>
        DateTime? UltimaExecucao { get; }

        /// <summary>
        /// Obtém a data da próxima execução.
        /// </summary>
        DateTime ProximaExecucao { get; }

        /// <summary>
        /// Força a execução do job.
        /// </summary>
        /// <returns>Tarefa.</returns>
        Task Executar();
    }
}
