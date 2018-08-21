// <copyright file="PrecoBeneficiamentoDto.cs" company="Sync Softwares">
// Copyright (c) Sync Softwares. Todos os direitos reservados.
// </copyright>

using Newtonsoft.Json;
using System.Runtime.Serialization;

namespace Glass.Data.Beneficiamentos.Total.Dto
{
    /// <summary>
    /// Classe que encapsula os preços e custos dos beneficiamentos para o controle.
    /// </summary>
    [DataContract(Name = "Preco")]
    public class PrecoBeneficiamentoDto
    {
        /// <summary>
        /// Obtém ou define o identificador do subgrupo para o preço.
        /// </summary>
        [DataMember]
        [JsonProperty("idSubgrupo")]
        public int? IdSubgrupo { get; set; }

        /// <summary>
        /// Obtém ou define o identificador da cor para o preço.
        /// </summary>
        [DataMember]
        [JsonProperty("idCor")]
        public int? IdCor { get; set; }

        /// <summary>
        /// Obtém ou define a espessura para o preço.
        /// </summary>
        [DataMember]
        [JsonProperty("espessura")]
        public double? Espessura { get; set; }

        /// <summary>
        /// Obtém ou define o custo unitário do beneficiamento.
        /// </summary>
        [DataMember]
        [JsonProperty("custoUnitario")]
        public decimal CustoUnitario { get; set; }

        /// <summary>
        /// Obtém ou define o valor unitário (atacado) do beneficiamento.
        /// </summary>
        [DataMember]
        [JsonProperty("valorAtacadoUnitario")]
        public decimal ValorAtacadoUnitario { get; set; }

        /// <summary>
        /// Obtém ou define o valor unitário (balcão) do beneficiamento.
        /// </summary>
        [DataMember]
        [JsonProperty("valorBalcaoUnitario")]
        public decimal ValorBalcaoUnitario { get; set; }

        /// <summary>
        /// Obtém ou define o valor unitário (obra) do beneficiamento.
        /// </summary>
        [DataMember]
        [JsonProperty("valorObraUnitario")]
        public decimal ValorObraUnitario { get; set; }
    }
}
