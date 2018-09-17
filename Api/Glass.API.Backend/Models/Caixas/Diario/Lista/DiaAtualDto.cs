// <copyright file="DiaAtualDto.cs" company="Sync Softwares">
// Copyright (c) Sync Softwares. Todos os direitos reservados.
// </copyright>

using Newtonsoft.Json;
using System.Runtime.Serialization;

namespace Glass.API.Backend.Models.Caixas.Diario.Lista
{
    /// <summary>
    /// Classe que encapsula dados do dia atual para fechamento do caixa.
    /// </summary>
    [DataContract(Name = "DiaAtual")]
    public class DiaAtualDto
    {
        /// <summary>
        /// Obtém ou define um valor que indica se o caixa está fechado.
        /// </summary>
        [DataMember]
        [JsonProperty("caixaFechado")]
        public bool CaixaFechado { get; set; }

        /// <summary>
        /// Obtém ou define o saldo atual do caixa.
        /// </summary>
        [DataMember]
        [JsonProperty("saldo")]
        public decimal Saldo { get; set; }

        /// <summary>
        /// Obtém ou define o saldo atual em dinheiro do caixa.
        /// </summary>
        [DataMember]
        [JsonProperty("saldoDinheiro")]
        public decimal SaldoDinheiro { get; set; }

        /// <summary>
        /// Obtém ou define um valor que indica se existem movimentações no caixa no dia atual.
        /// </summary>
        [DataMember]
        [JsonProperty("existemMovimentacoes")]
        public bool ExistemMovimentacoes { get; set; }
    }
}
