// <copyright file="TotalDto.cs" company="Sync Softwares">
// Copyright (c) Sync Softwares. Todos os direitos reservados.
// </copyright>

using Newtonsoft.Json;
using System.Runtime.Serialization;

namespace Glass.API.Backend.Models.Beneficiamentos.V1.Total
{
    /// <summary>
    /// Classe que encapsula a resposta do método de cálculo de totais do beneficiamento.
    /// </summary>
    [DataContract(Name = "Total")]
    public class TotalDto
    {
        /// <summary>
        /// Obtém ou define o identificador do item selecionado a qual pertence o valor calculado.
        /// </summary>
        [DataMember]
        [JsonProperty("idSelecionado")]
        public int IdItemSelecionado { get; set; }

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
