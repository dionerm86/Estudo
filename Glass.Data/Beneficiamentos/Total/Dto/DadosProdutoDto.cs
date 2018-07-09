// <copyright file="DadosProdutoDto.cs" company="Sync Softwares">
// Copyright (c) Sync Softwares. Todos os direitos reservados.
// </copyright>

using Newtonsoft.Json;
using System.Runtime.Serialization;

namespace Glass.Data.Beneficiamentos.Total.Dto
{
    /// <summary>
    /// Classe que encapsula os dados do produto para o cálculo de valor do beneficiamento.
    /// </summary>
    [DataContract(Name = "DadosProduto")]
    public class DadosProdutoDto
    {
        /// <summary>
        /// Obtém ou define o identificador do produto.
        /// </summary>
        [DataMember]
        [JsonProperty("id")]
        public int Id { get; set; }

        /// <summary>
        /// Obtém ou define a altura do produto.
        /// </summary>
        [DataMember]
        [JsonProperty("altura")]
        public double Altura { get; set; }

        /// <summary>
        /// Obtém ou define o identificador do produto.
        /// </summary>
        [DataMember]
        [JsonProperty("largura")]
        public int Largura { get; set; }

        /// <summary>
        /// Obtém ou define o identificador do produto.
        /// </summary>
        [DataMember]
        [JsonProperty("quantidade")]
        public int Quantidade { get; set; }

        /// <summary>
        /// Obtém ou define o identificador do produto.
        /// </summary>
        [DataMember]
        [JsonProperty("quantidadeAmbiente")]
        public int? QuantidadeAmbiente { get; set; }

        /// <summary>
        /// Obtém ou define a área, em m², do produto.
        /// </summary>
        [DataMember]
        [JsonProperty("areaM2")]
        public double AreaM2 { get; set; }

        /// <summary>
        /// Obtém ou define a área para cálculo, em m², do produto.
        /// </summary>
        [DataMember]
        [JsonProperty("areaCalculadaM2")]
        public double AreaCalculadaM2 { get; set; }

        /// <summary>
        /// Obtém ou define o valor unitário do produto.
        /// </summary>
        [DataMember]
        [JsonProperty("valorUnitario")]
        public decimal ValorUnitario { get; set; }

        /// <summary>
        /// Obtém ou define o custo unitário do produto.
        /// </summary>
        [DataMember]
        [JsonProperty("custoUnitario")]
        public decimal CustoUnitario { get; set; }
    }
}
