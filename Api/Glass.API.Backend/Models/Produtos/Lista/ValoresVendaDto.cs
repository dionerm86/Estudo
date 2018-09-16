// <copyright file="ValoresVendaDto.cs" company="Sync Softwares">
// Copyright (c) Sync Softwares. Todos os direitos reservados.
// </copyright>

using Newtonsoft.Json;
using System.Runtime.Serialization;

namespace Glass.API.Backend.Models.Produtos.Lista
{
    /// <summary>
    /// Classe que encapsula os valores de venda do produto.
    /// </summary>
    [DataContract(Name = "ValoresVenda")]
    public class ValoresVendaDto
    {
        /// <summary>
        /// Obtém ou define o valor de atacado do produto.
        /// </summary>
        [DataMember]
        [JsonProperty("atacado")]
        public decimal Atacado { get; set; }

        /// <summary>
        /// Obtém ou define o valor de balcão do produto.
        /// </summary>
        [DataMember]
        [JsonProperty("balcao")]
        public decimal Balcao { get; set; }

        /// <summary>
        /// Obtém ou define o valor de obra do produto.
        /// </summary>
        [DataMember]
        [JsonProperty("obra")]
        public decimal Obra { get; set; }

        /// <summary>
        /// Obtém ou define o valor de reposição do produto.
        /// </summary>
        [DataMember]
        [JsonProperty("reposicao")]
        public decimal Reposicao { get; set; }
    }
}
