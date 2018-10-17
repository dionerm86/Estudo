// <copyright file="ProducaoDto.cs" company="Sync Softwares">
// Copyright (c) Sync Softwares. Todos os direitos reservados.
// </copyright>

using Newtonsoft.Json;
using System.Runtime.Serialization;

namespace Glass.API.Backend.Models.Pedidos.V1.Lista
{
    /// <summary>
    /// Classe que encapsula os dados de produção do pedido.
    /// </summary>
    [DataContract(Name = "Producao")]
    public class ProducaoDto
    {
        /// <summary>
        /// Obtém ou define a situação de produção do pedido.
        /// </summary>
        [DataMember]
        [JsonProperty("situacao")]
        public string Situacao { get; set; }

        /// <summary>
        /// Obtém ou define um valor que indica se o pedido está pronto.
        /// </summary>
        [DataMember]
        [JsonProperty("pronto")]
        public bool Pronto { get; set; }

        /// <summary>
        /// Obtém ou define um valor que indica se o pedido está pendente.
        /// </summary>
        [DataMember]
        [JsonProperty("pendente")]
        public bool Pendente { get; set; }
    }
}
