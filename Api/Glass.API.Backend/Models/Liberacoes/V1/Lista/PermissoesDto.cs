// <copyright file="PermissoesDto.cs" company="Sync Softwares">
// Copyright (c) Sync Softwares. Todos os direitos reservados.
// </copyright>

using Newtonsoft.Json;
using System.Runtime.Serialization;

namespace Glass.API.Backend.Models.Liberacoes.V1.Lista
{
    /// <summary>
    /// Classe que encapsula os dados de permissão da liberação.
    /// </summary>
    [DataContract(Name = "Permissoes")]
    public class PermissoesDto
    {
        /// <summary>
        /// Obtém ou define um valor que indica se a liberação poderá ser impressa.
        /// </summary>
        [DataMember]
        [JsonProperty("imprimir")]
        public bool Imprimir { get; set; }

        /// <summary>
        /// Obtém ou define um valor que indica se deverá exibir a nota promissória.
        /// </summary>
        [DataMember]
        [JsonProperty("exibirNotaPromissoria")]
        public bool ExibirNotaPromissoria { get; set; }

        /// <summary>
        /// Obtém ou define um valor que indica se a liberação poderá ser cancelada.
        /// </summary>
        [DataMember]
        [JsonProperty("cancelar")]
        public bool Cancelar { get; set; }

        /// <summary>
        /// Obtém ou define um valor que indica se deverá exibir o boleto.
        /// </summary>
        [DataMember]
        [JsonProperty("exibirBoleto")]
        public bool ExibirBoleto { get; set; }

        /// <summary>
        /// Obtém ou define um valor que indica se deverá exibir a opção de reenviar email.
        /// </summary>
        [DataMember]
        [JsonProperty("exibirReenvioEmail")]
        public bool ExibirReenvioEmail { get; set; }

        /// <summary>
        /// Obtém ou define um valor que indica se deverá exibir as notas fiscais geradas a partir desta liberação.
        /// </summary>
        [DataMember]
        [JsonProperty("exibirNfeGerada")]
        public bool ExibirNfeGerada { get; set; }

        /// <summary>
        /// Obtém ou define um valor que indica se a liberação possui log de alterações.
        /// </summary>
        [DataMember]
        [JsonProperty("logAlteracoes")]
        public bool LogAlteracoes { get; set; }
    }
}
