// <copyright file="DatasEntregaDto.cs" company="Sync Softwares">
// Copyright (c) Sync Softwares. Todos os direitos reservados.
// </copyright>

using Newtonsoft.Json;
using System;
using System.Runtime.Serialization;

namespace Glass.API.Backend.Models.Pedidos.Lista
{
    /// <summary>
    /// Classe que encapsula as datas de entrega do pedido.
    /// </summary>
    [DataContract(Name = "DatasEntrega")]
    public class DatasEntregaDto
    {
        /// <summary>
        /// Obtém ou define a data de entrega atual do pedido.
        /// </summary>
        [DataMember]
        [JsonProperty("atual")]
        public DateTime? Atual { get; set; }

        /// <summary>
        /// Obtém ou define a data de entrega original do pedido.
        /// </summary>
        [DataMember]
        [JsonProperty("original")]
        public DateTime? Original { get; set; }
    }
}
