// <copyright file="PermissoesDto.cs" company="Sync Softwares">
// Copyright (c) Sync Softwares. Todos os direitos reservados.
// </copyright>

using Newtonsoft.Json;
using System.Runtime.Serialization;

namespace Glass.API.Backend.Models.Sinais.V1.Lista
{
    /// <summary>
    /// Classe que encapsula os dados de permissão da tela de sinal.
    /// </summary>
    [DataContract(Name = "Permissoes")]
    public class PermissoesDto
    {
        /// <summary>
        /// Obtém ou define um valor que indica se será permitido cancelar o sinal.
        /// </summary>
        [DataMember]
        [JsonProperty("cancelar")]
        public bool Cancelar { get; set; }

        /// <summary>
        /// Obtém ou define um valor que indica se o sinal possui log de alterações.
        /// </summary>
        [DataMember]
        [JsonProperty("logAlteracoes")]
        public bool LogAlteracoes { get; set; }

        /// <summary>
        /// Obtém ou define um valor que indica se o sinal possui log de cancelamento.
        /// </summary>
        [DataMember]
        [JsonProperty("logCancelamento")]
        public bool LogCancelamento { get; set; }
    }
}
