// <copyright file="DadosVolumeDto.cs" company="Sync Softwares">
// Copyright (c) Sync Softwares. Todos os direitos reservados.
// </copyright>

using Newtonsoft.Json;
using System.Runtime.Serialization;

namespace Glass.API.Backend.Models.Pedidos.V1.ListaVolumes
{
    /// <summary>
    /// Classe que encapsula dados do volume.
    /// </summary>
    [DataContract(Name = "DadosVolume")]
    public class DadosVolumeDto
    {
        /// <summary>
        /// Obtém ou define a quantidade de peças do volume.
        /// </summary>
        [DataMember]
        [JsonProperty("quantidadePecas")]
        public decimal QuantidadePecas { get; set; }

        /// <summary>
        /// Obtém ou define a quantidade de peças pendentes do volume.
        /// </summary>
        [DataMember]
        [JsonProperty("quantidadePecasPendentes")]
        public decimal QuantidadePecasPendentes { get; set; }

        /// <summary>
        /// Obtém ou define a metragem quadrada total do volume.
        /// </summary>
        [DataMember]
        [JsonProperty("metroQuadrado")]
        public decimal MetroQuadrado { get; set; }

        /// <summary>
        /// Obtém ou define o peso do volume.
        /// </summary>
        [DataMember]
        [JsonProperty("peso")]
        public decimal Peso { get; set; }

        /// <summary>
        /// Obtém ou define a situação do volume.
        /// </summary>
        [DataMember]
        [JsonProperty("situacao")]
        public string Situacao { get; set; }
    }
}
