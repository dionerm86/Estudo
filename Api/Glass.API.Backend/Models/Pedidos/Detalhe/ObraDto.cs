// <copyright file="ObraDto.cs" company="Sync Softwares">
// Copyright (c) Sync Softwares. Todos os direitos reservados.
// </copyright>

using Newtonsoft.Json;
using System.Runtime.Serialization;

namespace Glass.API.Backend.Models.Pedidos.Detalhe
{
    /// <summary>
    /// Classe que encapsula os dados de obra do pedido.
    /// </summary>
    [DataContract(Name = "Obra")]
    public class ObraDto
    {
        /// <summary>
        /// Obtém ou define o identificador da obra.
        /// </summary>
        [DataMember]
        [JsonProperty("id")]
        public int Id { get; set; }

        /// <summary>
        /// Obtém ou define o identificador do cliente da obra.
        /// </summary>
        [DataMember]
        [JsonProperty("idCliente")]
        public int IdCliente { get; set; }

        /// <summary>
        /// Obtém ou define a descrição da obra.
        /// </summary>
        [DataMember]
        [JsonProperty("descricao")]
        public string Descricao { get; set; }

        /// <summary>
        /// Obtém ou define o saldo atual da obra.
        /// </summary>
        [DataMember]
        [JsonProperty("saldo")]
        public decimal Saldo { get; set; }

        /// <summary>
        /// Obtém ou define o total de pedidos em aberto da obra.
        /// </summary>
        [DataMember]
        [JsonProperty("totalPedidosEmAberto")]
        public decimal TotalPedidosEmAberto { get; set; }

        /// <summary>
        /// Obtém ou define o endereço da obra.
        /// </summary>
        [DataMember]
        [JsonProperty("endereco")]
        public string Endereco { get; set; }
    }
}
