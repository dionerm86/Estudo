// <copyright file="PedidoDto.cs" company="Sync Softwares">
// Copyright (c) Sync Softwares. Todos os direitos reservados.
// </copyright>

using Newtonsoft.Json;
using System.Collections.Generic;
using System.Runtime.Serialization;

namespace Glass.API.Backend.Models.Pedidos.V1.Exportacao.PedidoExportar
{
    /// <summary>
    /// Classe que encapsula os dados de um pedido que será exportado.
    /// </summary>
    [DataContract(Name = "Pedido")]
    public class PedidoDto
    {
        /// <summary>
        /// Obtém ou define o identificador do pedido.
        /// </summary>
        [DataMember]
        [JsonProperty("idPedido")]
        public int? IdPedido { get; set; }

        /// <summary>
        /// Obtém ou define um valor que indica se os beneficiamentos serão exportados.
        /// </summary>
        [DataMember]
        [JsonProperty("exportarBeneficiamento")]
        public bool? ExportarBeneficiamento { get; set; }

        /// <summary>
        /// Obtém ou define os identificadores dos produtos do pedido.
        /// </summary>
        [DataMember]
        [JsonProperty("idsProdutoPedido")]
        public IEnumerable<int> IdsProdutoPedido { get; set; }
    }
}