// <copyright file="FastDeliveryDto.cs" company="Sync Softwares">
// Copyright (c) Sync Softwares. Todos os direitos reservados.
// </copyright>

using Newtonsoft.Json;
using System.Runtime.Serialization;

namespace Glass.API.Backend.Models.Pedidos.V1.Detalhe
{
    /// <summary>
    /// Classe que encapsula os dados de "fast delivery" do pedido.
    /// </summary>
    [DataContract(Name = "FastDelivery")]
    public class FastDeliveryDto
    {
        /// <summary>
        /// Obtém ou define um valor que indica se o "fast delivery" foi aplicado.
        /// </summary>
        [DataMember]
        [JsonProperty("aplicado")]
        public bool Aplicado { get; set; }

        /// <summary>
        /// Obtém ou define a o percentual de taxa de "fast delivery".
        /// </summary>
        [DataMember]
        [JsonProperty("taxa")]
        public decimal Taxa { get; set; }
    }
}
