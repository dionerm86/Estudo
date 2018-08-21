// <copyright file="ItemBeneficiamentoDto.cs" company="Sync Softwares">
// Copyright (c) Sync Softwares. Todos os direitos reservados.
// </copyright>

using Newtonsoft.Json;
using System.Runtime.Serialization;

namespace Glass.Data.Beneficiamentos.Total.Dto
{
    /// <summary>
    /// Classe que encapsula os dados de um item de beneficiamento.
    /// </summary>
    [DataContract(Name = "ItemBeneficiamento")]
    public class ItemBeneficiamentoDto
    {
        /// <summary>
        /// Obtém ou define o identificador do beneficiamento.
        /// </summary>
        [DataMember]
        [JsonProperty("id")]
        public int Id { get; set; }

        /// <summary>
        /// Obtém ou define a altura do beneficiamento.
        /// </summary>
        [DataMember]
        [JsonProperty("altura")]
        public int? Altura { get; set; }

        /// <summary>
        /// Obtém ou define a largura do beneficiamento.
        /// </summary>
        [DataMember]
        [JsonProperty("largura")]
        public int? Largura { get; set; }

        /// <summary>
        /// Obtém ou define a espessura do beneficiamento.
        /// </summary>
        [DataMember]
        [JsonProperty("espessura")]
        public double? Espessura { get; set; }

        /// <summary>
        /// Obtém ou define a quantidade de beneficiamentos.
        /// </summary>
        [DataMember]
        [JsonProperty("quantidade")]
        public int? Quantidade { get; set; }

        /// <summary>
        /// Obtém ou define o valor unitário do beneficiamento.
        /// </summary>
        [DataMember]
        [JsonProperty("valorUnitario")]
        public decimal ValorUnitario { get; set; }

        /// <summary>
        /// Obtém ou define o valor total do beneficiamento.
        /// </summary>
        [DataMember]
        [JsonProperty("valorTotal")]
        public decimal ValorTotal { get; set; }

        /// <summary>
        /// Obtém ou define o custo total do beneficiamento.
        /// </summary>
        [DataMember]
        [JsonProperty("custoTotal")]
        public decimal CustoTotal { get; set; }
    }
}
