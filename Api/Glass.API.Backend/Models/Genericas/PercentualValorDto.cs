// <copyright file="PercentualValorDto.cs" company="Sync Softwares">
// Copyright (c) Sync Softwares. Todos os direitos reservados.
// </copyright>

using Newtonsoft.Json;
using System.Runtime.Serialization;

namespace Glass.API.Backend.Models.Genericas
{
    /// <summary>
    /// Classe que encapsula dados de percentual e valor.
    /// </summary>
    [DataContract(Name = "PercentualValor")]
    public class PercentualValorDto
    {
        /// <summary>
        /// Obtém ou define o percentual.
        /// </summary>
        [DataMember]
        [JsonProperty("percentual")]
        public double? Percentual { get; set; }

        /// <summary>
        /// Obtém ou define o valor monetário.
        /// </summary>
        [DataMember]
        [JsonProperty("valor")]
        public decimal? Valor { get; set; }
    }
}
