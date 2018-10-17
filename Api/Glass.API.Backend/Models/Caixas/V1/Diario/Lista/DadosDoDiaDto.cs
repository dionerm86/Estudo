// <copyright file="DadosDoDiaDto.cs" company="Sync Softwares">
// Copyright (c) Sync Softwares. Todos os direitos reservados.
// </copyright>

using Newtonsoft.Json;
using System.Runtime.Serialization;

namespace Glass.API.Backend.Models.Caixas.V1.Diario.Lista
{
    /// <summary>
    /// Classe que encapsula dados do dia anterior e atual para fechamento do caixa.
    /// </summary>
    [DataContract(Name = "DadosDoDia")]
    public class DadosDoDiaDto
    {
        /// <summary>
        /// Obtém ou define um valor que indica se o caixa foi fechado.
        /// </summary>
        [DataMember]
        [JsonProperty("caixaFechado")]
        public bool CaixaFechado { get; set; }

        /// <summary>
        /// Obtém ou define o saldo do caixa.
        /// </summary>
        [DataMember]
        [JsonProperty("saldo")]
        public decimal Saldo { get; set; }

        /// <summary>
        /// Obtém ou define o saldo em dinheiro do caixa.
        /// </summary>
        [DataMember]
        [JsonProperty("saldoDinheiro")]
        public decimal SaldoDinheiro { get; set; }
    }
}
