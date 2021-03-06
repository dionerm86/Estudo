﻿using Newtonsoft.Json;
using System.Runtime.Serialization;

namespace Glass.API.Backend.Models.Produtos.V1.CalculoAreaM2
{
    /// <summary>
    /// Classe com o resultado do cálculo de área em m².
    /// </summary>
    [DataContract(Name = "AreaCalculada")]
    public class AreaCalculadaDto
    {
        /// <summary>
        /// Obtém ou define a área em m².
        /// </summary>
        [DataMember]
        [JsonProperty("areaM2")]
        public decimal AreaM2 { get; set; }

        /// <summary>
        /// Obtém ou define a área (para cálculo) em m².
        /// </summary>
        [DataMember]
        [JsonProperty("areaM2Calculo")]
        public decimal AreaM2Calculo { get; set; }

        /// <summary>
        /// Obtém ou define a área (para cálculo, desconsiderando a chapa de vidro) em m².
        /// </summary>
        [DataMember]
        [JsonProperty("areaM2CalculoSemChapaDeVidro")]
        public decimal AreaM2CalculoSemChapaDeVidro { get; set; }
    }
}
