// <copyright file="PedidoExternoDto.cs" company="Sync Softwares">
// Copyright (c) Sync Softwares. Todos os direitos reservados.
// </copyright>

using Glass.API.Backend.Models.Genericas.V1;
using Newtonsoft.Json;
using System.Runtime.Serialization;

namespace Glass.API.Backend.Models.Pedidos.V1.ListaVolumes
{
    /// <summary>
    /// Classe que encapsula dados do pedido externo.
    /// </summary>
    [DataContract(Name = "PedidoExterno")]
    public class PedidoExternoDto : IdDto
    {
        /// <summary>
        /// Obtém ou define a rota do pedido externo.
        /// </summary>
        [DataMember]
        [JsonProperty("rota")]
        public string Rota { get; set; }

        /// <summary>
        /// Obtém ou define o cliente do pedido externo.
        /// </summary>
        [DataMember]
        [JsonProperty("cliente")]
        public IdNomeDto Cliente { get; set; }
    }
}
