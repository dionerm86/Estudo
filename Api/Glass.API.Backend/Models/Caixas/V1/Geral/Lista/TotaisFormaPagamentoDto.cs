// <copyright file="TotaisFormaPagamentoDto.cs" company="Sync Softwares">
// Copyright (c) Sync Softwares. Todos os direitos reservados.
// </copyright>

using Newtonsoft.Json;
using System.Runtime.Serialization;

namespace Glass.API.Backend.Models.Caixas.V1.Geral.Lista
{
    /// <summary>
    /// Classe que encapsula dados dos valores totais por forma de pagamento.
    /// </summary>
    [DataContract(Name = "TotaisFormaPagamento")]
    public class TotaisFormaPagamentoDto
    {
        /// <summary>
        /// Obtém ou define o valor total em dinheiro.
        /// </summary>
        [DataMember]
        [JsonProperty("dinheiro")]
        public decimal Dinheiro { get; set; }

        /// <summary>
        /// Obtém ou define o valor total em cheque.
        /// </summary>
        [DataMember]
        [JsonProperty("cheque")]
        public decimal Cheque { get; set; }

        /// <summary>
        /// Obtém ou define o valor total em cartão.
        /// </summary>
        [DataMember]
        [JsonProperty("cartao")]
        public decimal Cartao { get; set; }

        /// <summary>
        /// Obtém ou define o valor total em construcard.
        /// </summary>
        [DataMember]
        [JsonProperty("construcard")]
        public decimal Construcard { get; set; }

        /// <summary>
        /// Obtém ou define o valor total em permuta.
        /// </summary>
        [DataMember]
        [JsonProperty("permuta")]
        public decimal Permuta { get; set; }
    }
}
