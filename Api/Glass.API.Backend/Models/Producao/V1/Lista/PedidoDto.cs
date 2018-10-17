// <copyright file="PedidoDto.cs" company="Sync Softwares">
// Copyright (c) Sync Softwares. Todos os direitos reservados.
// </copyright>

using Glass.API.Backend.Models.Genericas.V1;
using Newtonsoft.Json;
using System;
using System.Runtime.Serialization;

namespace Glass.API.Backend.Models.Producao.V1.Lista
{
    /// <summary>
    /// Classe que encapsula os dados de pedido para o retorno.
    /// </summary>
    [DataContract(Name = "Pedido")]
    public class PedidoDto : IdDto
    {
        /// <summary>
        /// Obtém ou define o identificador do pedido que será exibido.
        /// </summary>
        [DataMember]
        [JsonProperty("idExibir")]
        public string IdExibir { get; set; }

        /// <summary>
        /// Obtém ou define a sigla do tipo de pedido.
        /// </summary>
        [DataMember]
        [JsonProperty("siglaTipoPedido")]
        public string SiglaTipoPedido { get; set; }

        /// <summary>
        /// Obtém ou define um valor que indica se o pedido foi cancelado.
        /// </summary>
        [DataMember]
        [JsonProperty("cancelado")]
        public bool Cancelado { get; set; }

        /// <summary>
        /// Obtém ou define o código de pedido do cliente.
        /// </summary>
        [DataMember]
        [JsonProperty("codigoPedidoCliente")]
        public string CodigoPedidoCliente { get; set; }

        /// <summary>
        /// Obtém ou define a data de liberação do pedido.
        /// </summary>
        [DataMember]
        [JsonProperty("dataLiberacao")]
        public DateTime? DataLiberacao { get; set; }

        /// <summary>
        /// Obtém ou define os dados de cliente do pedido.
        /// </summary>
        [DataMember]
        [JsonProperty("cliente")]
        public IdNomeDto Cliente { get; set; }
    }
}
