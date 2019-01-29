// <copyright file="SituacoesDto.cs" company="Sync Softwares">
// Copyright (c) Sync Softwares. Todos os direitos reservados.
// </copyright>

using Newtonsoft.Json;
using System.Runtime.Serialization;

namespace Glass.API.Backend.Models.Pedidos.V1.Exportacao.Lista
{
    /// <summary>
    /// Classe que encapsula os dados das situações de um item da lista de pedidos para exportação.
    /// </summary>
    [DataContract(Name = "Situacoes")]
    public class SituacoesDto
    {
        /// <summary>
        /// Obtém ou define a situação do pedido.
        /// </summary>
        [DataMember]
        [JsonProperty("pedido")]
        public string Pedido { get; set; }

        /// <summary>
        /// Obtém ou define a situação da exportação.
        /// </summary>
        [DataMember]
        [JsonProperty("exportacao")]
        public string Exportacao { get; set; }
    }
}