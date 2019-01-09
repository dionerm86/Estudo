// <copyright file="MetroQuadradoDto.cs" company="Sync Softwares">
// Copyright (c) Sync Softwares. Todos os direitos reservados.
// </copyright>

using Newtonsoft.Json;
using System.Runtime.Serialization;

namespace Glass.API.Backend.Models.Producao.V1.Retalhos.Lista
{
    /// <summary>
    /// Classe que encapsula os dados referentes ao metro quadrado para as medidas de retalhos de produção.
    /// </summary>
    [DataContract(Name = "MetroQuadrado")]
    public class MetroQuadradoDto
    {
        /// <summary>
        /// Obtém ou define o total em metros quadrados do retalho.
        /// </summary>
        [DataMember]
        [JsonProperty("total")]
        public decimal? Total { get; set; }

        /// <summary>
        /// Obtém ou define o total em metros quadrados sendo utilizados do retalho.
        /// </summary>
        [DataMember]
        [JsonProperty("usando")]
        public decimal? Usando { get; set; }
    }
}