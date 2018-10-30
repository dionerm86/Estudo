// <copyright file="PerdaDto.cs" company="Sync Softwares">
// Copyright (c) Sync Softwares. Todos os direitos reservados.
// </copyright>

using Newtonsoft.Json;
using System;
using System.Runtime.Serialization;

namespace Glass.API.Backend.Models.Producao.V1.Lista
{
    /// <summary>
    /// Classe que encapsula os dados de perda da peça.
    /// </summary>
    [DataContract(Name = "Perda")]
    public class PerdaDto
    {
        /// <summary>
        /// Obtém ou define o tipo de perda da peça.
        /// </summary>
        [DataMember]
        [JsonProperty("tipo")]
        public string Tipo { get; set; }

        /// <summary>
        /// Obtém ou define a data de perda da peça.
        /// </summary>
        [DataMember]
        [JsonProperty("data")]
        public DateTime Data { get; set; }
    }
}
