// <copyright file="PermissoesDto.cs" company="Sync Softwares">
// Copyright (c) Sync Softwares. Todos os direitos reservados.
// </copyright>

using Newtonsoft.Json;
using System.Runtime.Serialization;

namespace Glass.API.Backend.Models.Cheques.V1.Lista
{
    /// <summary>
    /// Classe que encapsula os dados de permissão da tela de cheques.
    /// </summary>
    [DataContract(Name = "Permissoes")]
    public class PermissoesDto
    {
        /// <summary>
        /// Obtém ou define um valor que indica se será permitido alterar dados do cheque.
        /// </summary>
        [DataMember]
        [JsonProperty("alterarDadosCheque")]
        public bool AlterarDadosCheque { get; set; }

        /// <summary>
        /// Obtém ou define um valor que indica se será permitido alterar a data de vencimento do cheque.
        /// </summary>
        [DataMember]
        [JsonProperty("alterarDataVencimento")]
        public bool AlterarDataVencimento { get; set; }

        /// <summary>
        /// Obtém ou define um valor que indica se será permitido cancelar a reapresentação do cheque.
        /// </summary>
        [DataMember]
        [JsonProperty("cancelarReapresentacao")]
        public bool CancelarReapresentacao { get; set; }

        /// <summary>
        /// Obtém ou define um valor que indica se será permitido cancelar a devolução do cheque.
        /// </summary>
        [DataMember]
        [JsonProperty("cancelarDevolucao")]
        public bool CancelarDevolucao { get; set; }

        /// <summary>
        /// Obtém ou define um valor que indica se será permitido cancelar o protesto do cheque.
        /// </summary>
        [DataMember]
        [JsonProperty("cancelarProtesto")]
        public bool CancelarProtesto { get; set; }

        /// <summary>
        /// Obtém ou define um valor que indica se o cheque possui log de alterações.
        /// </summary>
        [DataMember]
        [JsonProperty("logAlteracoes")]
        public bool LogAlteracoes { get; set; }
    }
}
