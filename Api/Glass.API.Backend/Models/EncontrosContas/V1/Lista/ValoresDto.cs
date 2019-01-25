// <copyright file="ValoresDto.cs" company="Sync Softwares">
// Copyright (c) Sync Softwares. Todos os direitos reservados.
// </copyright>

using Newtonsoft.Json;
using System.Runtime.Serialization;

namespace Glass.API.Backend.Models.Acertos.V1.Lista
{
    /// <summary>
    /// Classe que encapsula os dados de valores da lista de encontros de contas.
    /// </summary>
    public class ValoresDto
    {
        /// <summary>
        /// Obtém ou define os valores a pagar.
        /// </summary>
        [DataMember]
        [JsonProperty("pagar")]
        public decimal Pagar { get; set; }

        /// <summary>
        /// Obtém ou define os valores a receber.
        /// </summary>
        [DataMember]
        [JsonProperty("receber")]
        public decimal Receber { get; set; }

        /// <summary>
        /// Obtém ou define os saldos dos valores.
        /// </summary>
        [DataMember]
        [JsonProperty("saldo")]
        public decimal Saldo { get; set; }
    }
}