// <copyright file="EstoqueDto.cs" company="Sync Softwares">
// Copyright (c) Sync Softwares. Todos os direitos reservados.
// </copyright>

using Newtonsoft.Json;
using System.Runtime.Serialization;

namespace Glass.API.Backend.Models.Produtos.V1.Lista
{
    /// <summary>
    /// Classe que encapsula dados de estoque do produto.
    /// </summary>
    [DataContract(Name = "Estoque")]
    public class EstoqueDto
    {
        /// <summary>
        /// Obtém ou define a quantidade de estoque em "reserva" do produto.
        /// </summary>
        [DataMember]
        [JsonProperty("reserva")]
        public decimal Reserva { get; set; }

        /// <summary>
        /// Obtém ou define a quantidade de estoque em "liberação" do produto.
        /// </summary>
        [DataMember]
        [JsonProperty("liberacao")]
        public decimal Liberacao { get; set; }

        /// <summary>
        /// Obtém ou define o estoque real do produto.
        /// </summary>
        [DataMember]
        [JsonProperty("real")]
        public decimal Real { get; set; }

        /// <summary>
        /// Obtém ou define o estoque disponível do produto.
        /// </summary>
        [DataMember]
        [JsonProperty("disponivel")]
        public decimal Disponivel { get; set; }

        /// <summary>
        /// Obtém ou define a unidade do estoque.
        /// </summary>
        [DataMember]
        [JsonProperty("unidade")]
        public string Unidade { get; set; }
    }
}
