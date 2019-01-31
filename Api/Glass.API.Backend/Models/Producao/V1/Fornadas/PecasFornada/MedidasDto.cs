// <copyright file="MedidasDto.cs" company="Sync Softwares">
// Copyright (c) Sync Softwares. Todos os direitos reservados.
// </copyright>

using Newtonsoft.Json;
using System.Runtime.Serialization;

namespace Glass.API.Backend.Models.Producao.V1.Fornadas.PecasFornada
{
    /// <summary>
    /// Classe que encapsula os dados das medidas para uma peça de fornada.
    /// </summary>
    [DataContract(Name = "Medidas")]
    public class MedidasDto
    {
        /// <summary>
        /// Obtém ou define a altura da peça de fornada.
        /// </summary>
        [DataMember]
        [JsonProperty("altura")]
        public decimal? Altura { get; set; }

        /// <summary>
        /// Obtém ou define a largura da peça de fornada.
        /// </summary>
        [DataMember]
        [JsonProperty("largura")]
        public decimal? Largura { get; set; }
    }
}