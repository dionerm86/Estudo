// <copyright file="EntradaDto.cs" company="Sync Softwares">
// Copyright (c) Sync Softwares. Todos os direitos reservados.
// </copyright>

using Newtonsoft.Json;
using System.Runtime.Serialization;

namespace Glass.API.Backend.Models.Pedidos.V1.AlterarLiberacaoFinanceiro
{
    /// <summary>
    /// Classe que encapsula os dados de entrada do método de alteração de liberação do financeiro.
    /// </summary>
    [DataContract]
    public class EntradaDto
    {
        /// <summary>
        /// Obtém ou define se o pedido deverá ser liberado ou não.
        /// </summary>
        [DataMember]
        [JsonProperty("liberar")]
        public bool? Liberar { get; set; }
    }
}
