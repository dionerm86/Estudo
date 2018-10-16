// <copyright file="DadosProdutoDto.cs" company="Sync Softwares">
// Copyright (c) Sync Softwares. Todos os direitos reservados.
// </copyright>

using Newtonsoft.Json;
using System.Runtime.Serialization;

namespace Glass.API.Backend.Models.Produtos.V1.CalculoTotal
{
    /// <summary>
    /// Classe que encapsula os dados do produto para o serviço de cálculo de área.
    /// </summary>
    [DataContract(Name = "DadosProduto")]
    public class DadosProdutoDto : CalculoAreaM2.DadosProdutoDto
    {
        /// <summary>
        /// Obtém ou define a quantidade do ambiente.
        /// </summary>
        [DataMember]
        [JsonProperty("quantidadeAmbiente")]
        public int? QuantidadeAmbiente { get; set; }

        /// <summary>
        /// Obtém ou define a quantidade do ambiente.
        /// </summary>
        [DataMember]
        [JsonProperty("areaM2")]
        public decimal AreaM2 { get; set; }

        /// <summary>
        /// Obtém ou define a quantidade do ambiente.
        /// </summary>
        [DataMember]
        [JsonProperty("areaCalculadaM2")]
        public decimal AreaCalculadaM2 { get; set; }

        /// <summary>
        /// Obtém ou define o valor unitário para o produto.
        /// </summary>
        [DataMember]
        [JsonProperty("valorUnitario")]
        public decimal ValorUnitario { get; set; }

        /// <summary>
        /// Obtém ou define o percentual de desconto por quantidade.
        /// </summary>
        [DataMember]
        [JsonProperty("percentualDescontoPorQuantidade")]
        public decimal PercentualDescontoPorQuantidade { get; set; }
    }
}
