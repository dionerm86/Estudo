// <copyright file="PermissoesDto.cs" company="Sync Softwares">
// Copyright (c) Sync Softwares. Todos os direitos reservados.
// </copyright>

using Newtonsoft.Json;
using System.Runtime.Serialization;

namespace Glass.API.Backend.Models.DevolucoesPagamento.V1.Lista
{
    /// <summary>
    /// Classe que encapsula os dados de permissão da lista de devoluções de pagamentos.
    /// </summary>
    [DataContract(Name = "Permissoes")]
    public class PermissoesDto
    {
        /// <summary>
        /// Obtém ou define um valor que indica se a devolução pode ser cancelada.
        /// </summary>
        [DataMember]
        [JsonProperty("cancelar")]
        public bool Cancelar { get; set; }

        /// <summary>
        /// Obtém ou define um valor que indica se a devolução possui log de cancelamento.
        /// </summary>
        [DataMember]
        [JsonProperty("logCancelamento")]
        public bool LogCancelamento { get; set; }
    }
}