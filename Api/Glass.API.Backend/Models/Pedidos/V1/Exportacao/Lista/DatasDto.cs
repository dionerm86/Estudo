// <copyright file="DatasDto.cs" company="Sync Softwares">
// Copyright (c) Sync Softwares. Todos os direitos reservados.
// </copyright>

using Glass.API.Backend.Models.Genericas.V1;
using Newtonsoft.Json;
using System;
using System.Runtime.Serialization;

namespace Glass.API.Backend.Models.Pedidos.V1.Exportacao.Lista
{
    /// <summary>
    /// Classe que encapsula os dados das datas para um item da lista de pedidos para exportação.
    /// </summary>
    [DataContract(Name = "Datas")]
    public class DatasDto
    {
        /// <summary>
        /// Obtém ou define a data do pedido.
        /// </summary>
        [DataMember]
        [JsonProperty("pedido")]
        public DateTime? Pedido { get; set; }

        /// <summary>
        /// Obtém ou define a data de entrega do pedido.
        /// </summary>
        [DataMember]
        [JsonProperty("entrega")]
        public DateTime? Entrega { get; set; }
    }
}