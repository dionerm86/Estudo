// <copyright file="SinalDto.cs" company="Sync Softwares">
// Copyright (c) Sync Softwares. Todos os direitos reservados.
// </copyright>

using Newtonsoft.Json;
using System.Runtime.Serialization;

namespace Glass.API.Backend.Models.Pedidos.Detalhe
{
    /// <summary>
    /// Classe que encapsula dados de sinal do pedido.
    /// </summary>
    [DataContract(Name = "Sinal")]
    public class SinalDto
    {
        /// <summary>
        /// Obtém ou define o identificador do sinal do pedido.
        /// </summary>
        [DataMember]
        [JsonProperty("id")]
        public int Id { get; set; }

        /// <summary>
        /// Obtém ou define o valor pago como sinal do pedido.
        /// </summary>
        [DataMember]
        [JsonProperty("valor")]
        public decimal Valor { get; set; }
    }
}
