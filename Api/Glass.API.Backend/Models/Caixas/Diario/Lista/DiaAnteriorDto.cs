// <copyright file="DiaAnteriorDto.cs" company="Sync Softwares">
// Copyright (c) Sync Softwares. Todos os direitos reservados.
// </copyright>

using Newtonsoft.Json;
using System;
using System.Runtime.Serialization;

namespace Glass.API.Backend.Models.Caixas.Diario.Lista
{
    /// <summary>
    /// Classe que encapsula dados de um dia anterior ao atual para fechamento do caixa.
    /// </summary>
    [DataContract(Name = "DiaAnterior")]
    public class DiaAnteriorDto
    {
        /// <summary>
        /// Obtém ou define um valor que indica se o caixa de um dia anterior ao atual está fechado.
        /// </summary>
        [DataMember]
        [JsonProperty("caixaFechado")]
        public bool CaixaFechado { get; set; }

        /// <summary>
        /// Obtém ou define o saldo de um dia anterior ao atual do caixa.
        /// </summary>
        [DataMember]
        [JsonProperty("saldo")]
        public decimal Saldo { get; set; }

        /// <summary>
        /// Obtém ou define o saldo de um dia anterior ao atual em dinheiro do caixa.
        /// </summary>
        [DataMember]
        [JsonProperty("saldoDinheiro")]
        public decimal SaldoDinheiro { get; set; }

        /// <summary>
        /// Obtém ou define a data que o caixa ficou aberto.
        /// </summary>
        [DataMember]
        [JsonProperty("dataCaixaAberto")]
        public DateTime? DataCaixaAberto { get; set; }
    }
}
