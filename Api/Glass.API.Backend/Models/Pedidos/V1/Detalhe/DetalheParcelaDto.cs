// <copyright file="DetalheParcelaDto.cs" company="Sync Softwares">
// Copyright (c) Sync Softwares. Todos os direitos reservados.
// </copyright>

using Newtonsoft.Json;
using System;
using System.Runtime.Serialization;

namespace Glass.API.Backend.Models.Pedidos.V1.Detalhe
{
    /// <summary>
    /// Classe que encapsula os detalhes de parcela do pedido.
    /// </summary>
    [DataContract(Name = "DetalheParcela")]
    public class DetalheParcelaDto
    {
        /// <summary>
        /// Obtém ou define a data da parcela.
        /// </summary>
        [DataMember]
        [JsonProperty("data")]
        public DateTime Data { get; set; }

        /// <summary>
        /// Obtém ou define o valor da parcela.
        /// </summary>
        [DataMember]
        [JsonProperty("valor")]
        public decimal Valor { get; set; }
    }
}
