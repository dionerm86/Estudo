// <copyright file="PedidosExportarDto.cs" company="Sync Softwares">
// Copyright (c) Sync Softwares. Todos os direitos reservados.
// </copyright>

using Newtonsoft.Json;
using System.Collections.Generic;
using System.Runtime.Serialization;

namespace Glass.API.Backend.Models.Pedidos.V1.Exportacao.PedidoExportar
{
    /// <summary>
    /// Classe que encapsula os dados de uma exportação de pedidos.
    /// </summary>
    [DataContract(Name = "PedidoExportar")]
    public class PedidosExportarDto
    {
        /// <summary>
        /// Obtém ou define o identificador do fornecedor.
        /// </summary>
        [DataMember]
        [JsonProperty("idFornecedor")]
        public int? IdFornecedor { get; set; }

        /// <summary>
        /// Obtém ou define os pedidos que serão exportados.
        /// </summary>
        [DataMember]
        [JsonProperty("pedidos")]
        public IEnumerable<PedidoDto> Pedidos { get; set; }
    }
}