// <copyright file="ComissionadoDto.cs" company="Sync Softwares">
// Copyright (c) Sync Softwares. Todos os direitos reservados.
// </copyright>

using Newtonsoft.Json;
using System.Runtime.Serialization;

namespace Glass.API.Backend.Models.Pedidos.V1.CadastroAtualizacao
{
    /// <summary>
    /// Classe que encapsula os dados de comissionado do pedido.
    /// </summary>
    [DataContract(Name = "Comissionado")]
    public class ComissionadoDto
    {
        /// <summary>
        /// Obtém ou define o identificador do comissionado.
        /// </summary>
        [DataMember]
        [JsonProperty("id")]
        public int? Id { get; set; }

        /// <summary>
        /// Obtém ou define o percentual de comissão do comissionado.
        /// </summary>
        [DataMember]
        [JsonProperty("percentualComissao")]
        public decimal PercentualComissao { get; set; }
    }
}
