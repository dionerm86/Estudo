// <copyright file="DescontoDto.cs" company="Sync Softwares">
// Copyright (c) Sync Softwares. Todos os direitos reservados.
// </copyright>

using Newtonsoft.Json;
using System.Runtime.Serialization;

namespace Glass.API.Backend.Models.Pedidos.V1.ValidarDescontoPedido
{
    /// <summary>
    /// Classe de resposta do desconto no pedido.
    /// </summary>
    [DataContract(Name = "Desconto")]
    public class DescontoDto
    {
        /// <summary>
        /// Obtém ou define o desconto permitido no pedido.
        /// </summary>
        [DataMember]
        [JsonProperty("descontoPermitido")]
        public decimal DescontoPermitido { get; set; }
    }
}
