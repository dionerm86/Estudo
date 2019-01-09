// <copyright file="ValoresPagamentoDto.cs" company="Sync Softwares">
// Copyright (c) Sync Softwares. Todos os direitos reservados.
// </copyright>

using Newtonsoft.Json;
using System.Runtime.Serialization;

namespace Glass.API.Backend.Models.ContasPagar.V1.Pagas.Lista
{
    /// <summary>
    /// Classe que encapsula os dados referentes aos valores de pagamento para a tela de listagem de contas pagas.
    /// </summary>
    [DataContract(Name = "ValoresPagamento")]
    public class ValoresPagamentoDto
    {
        /// <summary>
        /// Obtém ou define o valor pago.
        /// </summary>
        [DataMember]
        [JsonProperty("valorPago")]
        public decimal ValorPago { get; set; }

        /// <summary>
        /// Obtém ou define o montante de juros.
        /// </summary>
        [DataMember]
        [JsonProperty("juros")]
        public decimal Juros { get; set; }

        /// <summary>
        /// Obtém ou define o montante de multa.
        /// </summary>
        [DataMember]
        [JsonProperty("multa")]
        public decimal Multa { get; set; }
    }
}