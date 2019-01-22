// <copyright file="FormasPagamentoDto.cs" company="Sync Softwares">
// Copyright (c) Sync Softwares. Todos os direitos reservados.
// </copyright>

using Newtonsoft.Json;
using System.Runtime.Serialization;

namespace Glass.API.Backend.Models.Liberacoes.V1.Movimentacoes.Lista
{
    /// <summary>
    /// Classe que encapsula os dados das formas de pagamento para a tela de listagem de movimentações de liberações.
    /// </summary>
    [DataContract(Name = "FormasPagamento")]
    public class FormasPagamentoDto
    {
        /// <summary>
        /// Obtém ou define a forma de pagamento 'dinheiro' para um item da lista.
        /// </summary>
        [DataMember]
        [JsonProperty("dinheiro")]
        public decimal Dinheiro { get; set; }

        /// <summary>
        /// Obtém ou define a forma de pagamento 'cheque' para um item da lista.
        /// </summary>
        [DataMember]
        [JsonProperty("cheque")]
        public decimal Cheque { get; set; }

        /// <summary>
        /// Obtém ou define a forma de pagamento 'boleto' para um item da lista.
        /// </summary>
        [DataMember]
        [JsonProperty("boleto")]
        public decimal Boleto { get; set; }

        /// <summary>
        /// Obtém ou define a forma de pagamento 'depósito' para um item da lista.
        /// </summary>
        [DataMember]
        [JsonProperty("deposito")]
        public decimal Deposito { get; set; }

        /// <summary>
        /// Obtém ou define a forma de pagamento 'cartão' para um item da lista.
        /// </summary>
        [DataMember]
        [JsonProperty("cartao")]
        public decimal Cartao { get; set; }

        /// <summary>
        /// Obtém ou define a forma de pagamento 'prazo' para um item da lista.
        /// </summary>
        [DataMember]
        [JsonProperty("prazo")]
        public decimal Prazo { get; set; }

        /// <summary>
        /// Obtém ou define a forma de pagamento 'outros' para um item da lista.
        /// </summary>
        [DataMember]
        [JsonProperty("outros")]
        public decimal Outros { get; set; }

        /// <summary>
        /// Obtém ou define a forma de pagamento 'débito' para um item da lista.
        /// </summary>
        [DataMember]
        [JsonProperty("debito")]
        public decimal Debito { get; set; }

        /// <summary>
        /// Obtém ou define a forma de pagamento 'crédito' para um item da lista.
        /// </summary>
        [DataMember]
        [JsonProperty("credito")]
        public decimal Credito { get; set; }
    }
}
