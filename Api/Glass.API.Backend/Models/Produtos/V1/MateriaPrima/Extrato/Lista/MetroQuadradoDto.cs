// <copyright file="MetroQuadradoDto.cs" company="Sync Softwares">
// Copyright (c) Sync Softwares. Todos os direitos reservados.
// </copyright>

using Newtonsoft.Json;
using System.Collections.Generic;
using System.Runtime.Serialization;

namespace Glass.API.Backend.Models.Produtos.V1.MateriaPrima.Extrato.Lista
{
    /// <summary>
    /// Classe que encapsula os dados referentes ao metro quadrado para um item da lista de movimentações do extrato de movimentações de chapa.
    /// </summary>
    [DataContract(Name = "MetroQuadrado")]
    public class MetroQuadradoDto
    {
        /// <summary>
        /// Obtém ou define a quantidade em metros quadrados utilizada.
        /// </summary>
        [DataMember]
        [JsonProperty("utilizado")]
        public decimal? Utilizado { get; set; }

        /// <summary>
        /// Obtém ou define a quantidade em metros quadrados lida.
        /// </summary>
        [DataMember]
        [JsonProperty("lido")]
        public decimal? Lido { get; set; }

        /// <summary>
        /// Obtém ou define a quantidade em metros quadrados restante a ser lida.
        /// </summary>
        [DataMember]
        [JsonProperty("sobra")]
        public decimal? Sobra { get; set; }
    }
}