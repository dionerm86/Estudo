// <copyright file="SinalEPagamentoAntecipadoDto.cs" company="Sync Softwares">
// Copyright (c) Sync Softwares. Todos os direitos reservados.
// </copyright>

using Newtonsoft.Json;
using System.Runtime.Serialization;

namespace Glass.API.Backend.Models.Pedidos.Lista
{
    /// <summary>
    /// Classe que encapsula os dados de sinal e pagamento antecipado do pedido.
    /// </summary>
    [DataContract(Name = "SinalEPagamentoAntecipado")]
    public class SinalEPagamentoAntecipadoDto
    {
        /// <summary>
        /// Obtém ou define o identificador do sinal do pedido.
        /// </summary>
        [DataMember]
        [JsonProperty("idSinal")]
        public int? IdSinal { get; set; }

        /// <summary>
        /// Obtém ou define um valor que indica se o pedido possui pagamento antecipado.
        /// </summary>
        [DataMember]
        [JsonProperty("temPagamentoAntecipado")]
        public bool TemPagamentoAntecipado { get; set; }

        /// <summary>
        /// Obtém ou define o valor pago como sinal do pedido.
        /// </summary>
        [DataMember]
        [JsonProperty("valorSinal")]
        public decimal ValorSinal { get; set; }

        /// <summary>
        /// Obtém ou define o valor pago antecipadamente pelo pedido.
        /// </summary>
        [DataMember]
        [JsonProperty("valorPagamentoAntecipado")]
        public decimal ValorPagamentoAntecipado { get; set; }
    }
}
