// <copyright file="MedidasDto.cs" company="Sync Softwares">
// Copyright (c) Sync Softwares. Todos os direitos reservados.
// </copyright>

using Newtonsoft.Json;
using System.Runtime.Serialization;

namespace Glass.API.Backend.Models.Producao.V1.Retalhos.Lista
{
    /// <summary>
    /// Classe que encapsula os dados de medidas para a lista de retalhos de produção.
    /// </summary>
    [DataContract(Name = "Medidas")]
    public class MedidasDto
    {
        /// <summary>
        /// Obtém ou define a largura do retalho.
        /// </summary>
        [DataMember]
        [JsonProperty("largura")]
        public decimal? Largura { get; set; }

        /// <summary>
        /// Obtém ou define a altura do retalho.
        /// </summary>
        [DataMember]
        [JsonProperty("altura")]
        public decimal? Altura { get; set; }

        /// <summary>
        /// Obtém ou define os dados referentes ao metro quadrado do retalho.
        /// </summary>
        [DataMember]
        [JsonProperty("metroQuadrado")]
        public MetroQuadradoDto MetroQuadrado { get; set; }
    }
}