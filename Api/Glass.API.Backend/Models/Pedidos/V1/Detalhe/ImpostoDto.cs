// <copyright file="ImpostoDto.cs" company="Sync Softwares">
// Copyright (c) Sync Softwares. Todos os direitos reservados.
// </copyright>

using Newtonsoft.Json;
using System.Runtime.Serialization;

namespace Glass.API.Backend.Models.Pedidos.V1.Detalhe
{
    /// <summary>
    /// Obtém ou define os dados de impostos do pedido.
    /// </summary>
    [DataContract(Name = "Imposto")]
    public class ImpostoDto
    {
        /// <summary>
        /// Obtém ou define a alíquota do imposto.
        /// </summary>
        [DataMember]
        [JsonProperty("aliquota")]
        public float Aliquota { get; set; }

        /// <summary>
        /// Obtém ou define o valor do imposto.
        /// </summary>
        [DataMember]
        [JsonProperty("valor")]
        public decimal Valor { get; set; }
    }
}
