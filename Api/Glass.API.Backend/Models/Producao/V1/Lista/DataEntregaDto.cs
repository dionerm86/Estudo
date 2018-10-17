// <copyright file="DataEntregaDto.cs" company="Sync Softwares">
// Copyright (c) Sync Softwares. Todos os direitos reservados.
// </copyright>

using Newtonsoft.Json;
using System;
using System.Runtime.Serialization;

namespace Glass.API.Backend.Models.Producao.V1.Lista
{
    /// <summary>
    /// Classe que encapsula os dados de data de entrega da peça.
    /// </summary>
    [DataContract(Name = "DataEntrega")]
    public class DataEntregaDto
    {
        /// <summary>
        /// Obtém ou define a data de fábrica da peça.
        /// </summary>
        [DataMember]
        [JsonProperty("fabrica")]
        public DateTime? Fabrica { get; set; }

        /// <summary>
        /// Obtém ou define a data de entrega que será exibida da peça.
        /// </summary>
        [DataMember]
        [JsonProperty("exibicao")]
        public string Exibicao { get; set; }
    }
}
