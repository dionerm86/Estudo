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
        /// Obtém ou define a forma de pagamento como dinheiro.
        /// </summary>
        [DataMember]
        [JsonProperty("dinheiro")]
        public decimal Dinheiro { get; set; }

        /// <summary>
        /// Obtém ou define a forma de pagamento como cheque.
        /// </summary>
        [DataMember]
        [JsonProperty("cheque")]
        public decimal Cheque { get; set; }

        /// <summary>
        /// Obtém ou define a forma de pagamento como boleto.
        /// </summary>
        [DataMember]
        [JsonProperty("boleto")]
        public decimal Boleto { get; set; }

        /// <summary>
        /// Obtém ou define a forma de pagamento como depósito.
        /// </summary>
        [DataMember]
        [JsonProperty("deposito")]
        public decimal Deposito { get; set; }

        /// <summary>
        /// Obtém ou define a forma de pagamento como cartão.
        /// </summary>
        [DataMember]
        [JsonProperty("cartao")]
        public decimal Cartao { get; set; }

        /// <summary>
        /// Obtém ou define a forma de pagamento como prazo.
        /// </summary>
        [DataMember]
        [JsonProperty("prazo")]
        public decimal Prazo { get; set; }

        /// <summary>
        /// Obtém ou define a forma de pagamento como outros.
        /// </summary>
        [DataMember]
        [JsonProperty("outros")]
        public decimal Outros { get; set; }

        /// <summary>
        /// Obtém ou define a forma de pagamento como débito.
        /// </summary>
        [DataMember]
        [JsonProperty("debito")]
        public decimal Debito { get; set; }

        /// <summary>
        /// Obtém ou define a forma de pagamento como crédito.
        /// </summary>
        [DataMember]
        [JsonProperty("credito")]
        public decimal Credito { get; set; }
    }
}
