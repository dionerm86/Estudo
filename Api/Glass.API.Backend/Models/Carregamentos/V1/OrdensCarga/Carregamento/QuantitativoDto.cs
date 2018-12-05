// <copyright file="QuantitativoDto.cs" company="Sync Softwares">
// Copyright (c) Sync Softwares. Todos os direitos reservados.
// </copyright>

using Newtonsoft.Json;
using System.Runtime.Serialization;

namespace Glass.API.Backend.Models.Carregamentos.V1.OrdensCarga.Carregamento
{
    /// <summary>
    /// Classe que encapsula dados quantitativos.
    /// </summary>
    [DataContract(Name = "Quantitativo")]
    public class QuantitativoDto
    {
        /// <summary>
        /// Obtém ou define o valor total.
        /// </summary>
        [DataMember]
        [JsonProperty("total")]
        public decimal Total { get; set; }

        /// <summary>
        /// Obtém ou define o valor pendente.
        /// </summary>
        [DataMember]
        [JsonProperty("pendente")]
        public decimal Pendente { get; set; }
    }
}
