// <copyright file="JobIntegracaoDto.cs" company="Sync Softwares">
// Copyright (c) Sync Softwares. Todos os direitos reservados.
// </copyright>

using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Web;

namespace Glass.API.Backend.Models.Integracao.V1.Integradores.Lista
{
    /// <summary>
    /// Classe que encapsula os dados do Job de integração.
    /// </summary>
    [DataContract(Name = "JobIntegracao")]
    public class JobIntegracaoDto
    {
        /// <summary>
        /// Inicia uma nova instância da classe <see cref="JobIntegracaoDto"/>.
        /// </summary>
        /// <param name="job">Job que será encapsulado.</param>
        public JobIntegracaoDto(Glass.Integracao.IJobIntegracao job)
        {
            this.Nome = job.Nome;
            this.Descricao = job.Descricao;
            this.Situacao = job.Situacao.ToString();
            this.UltimaFalha = job.UltimaFalha?.Message;
            this.UltimaExecucaoComFalha = job.UltimaExecucaoComFalha;
            this.UltimaExecucao = job.UltimaExecucao;
            this.ProximaExecucao = job.ProximaExecucao;
        }

        /// <summary>
        /// Obtém o nome do Job.
        /// </summary>
        [DataMember]
        [JsonProperty("nome")]
        public string Nome { get; }

        /// <summary>
        /// Obtém a descrição do Job.
        /// </summary>
        [DataMember]
        [JsonProperty("descricao")]
        public string Descricao { get; }

        /// <summary>
        /// Obtém a situação da última execução do Job.
        /// </summary>
        [DataMember]
        [JsonProperty("situacao")]
        public string Situacao { get; }

        /// <summary>
        /// Obtém última falha.
        /// </summary>
        [DataMember]
        [JsonProperty("ultimaFalha")]
        public string UltimaFalha { get; }

        /// <summary>
        /// Obtém a data da última falha.
        /// </summary>
        [DataMember]
        [JsonProperty("ultimaExecucaoComFalha")]
        public DateTime? UltimaExecucaoComFalha { get; }

        /// <summary>
        /// Obtém a data da última execução.
        /// </summary>
        [DataMember]
        [JsonProperty("ultimaExecucao")]
        public DateTime? UltimaExecucao { get; }

        /// <summary>
        /// Obtém a data da próxima execução.
        /// </summary>
        [DataMember]
        [JsonProperty("proximaExecucao")]
        public DateTime ProximaExecucao { get; }
    }
}
