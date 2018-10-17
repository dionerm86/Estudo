// <copyright file="PermissoesDto.cs" company="Sync Softwares">
// Copyright (c) Sync Softwares. Todos os direitos reservados.
// </copyright>

using Newtonsoft.Json;
using System.Runtime.Serialization;

namespace Glass.API.Backend.Models.Acertos.V1.Lista
{
    /// <summary>
    /// Classe que encapsula os dados de permissão da tela de listagem de acertos.
    /// </summary>
    [DataContract(Name = "Permissoes")]
    public class PermissoesDto
    {
        /// <summary>
        /// Obtém ou define um valor que indica se será permitido cancelar o acerto.
        /// </summary>
        [DataMember]
        [JsonProperty("cancelar")]
        public bool Cancelar { get; set; }

        /// <summary>
        /// Obtém ou define um valor que indica se será permitido imprimir a nota promissória do acerto.
        /// </summary>
        [DataMember]
        [JsonProperty("exibirNotaPromissoria")]
        public bool ExibirNotaPromissoria { get; set; }

        /// <summary>
        /// Obtém ou define um valor que indica se o item possui log de cancelamento.
        /// </summary>
        [DataMember]
        [JsonProperty("logCancelamento")]
        public bool LogCancelamento { get; set; }
    }
}
