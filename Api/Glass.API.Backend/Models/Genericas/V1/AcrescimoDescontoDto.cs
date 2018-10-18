// <copyright file="AcrescimoDescontoDto.cs" company="Sync Softwares">
// Copyright (c) Sync Softwares. Todos os direitos reservados.
// </copyright>

using Newtonsoft.Json;
using System.Runtime.Serialization;

namespace Glass.API.Backend.Models.Genericas.V1
{
    /// <summary>
    /// Classe que encapsula os dados de acréscimo ou desconto.
    /// </summary>
    [DataContract(Name = "AcrescimoDesconto")]
    public class AcrescimoDescontoDto
    {
        /// <summary>
        /// Obtém ou define o tipo de acréscimo ou desconto.
        /// </summary>
        [DataMember]
        [JsonProperty("tipo")]
        public int? Tipo { get; set; }

        /// <summary>
        /// Obtém ou define o valor ou percentual de acréscimo ou desconto, pelo tipo.
        /// </summary>
        [DataMember]
        [JsonProperty("valor")]
        public decimal? Valor { get; set; }
    }
}
