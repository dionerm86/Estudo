// <copyright file="TotalCalculadoDto.cs" company="Sync Softwares">
// Copyright (c) Sync Softwares. Todos os direitos reservados.
// </copyright>

using Newtonsoft.Json;
using System.Runtime.Serialization;

namespace Glass.API.Backend.Models.Produtos.V1.CalculoTotal
{
    /// <summary>
    /// Classe que encapsula os dados do total calculado.
    /// </summary>
    [DataContract(Name = "TotalCalculado")]
    public class TotalCalculadoDto
    {
        /// <summary>
        /// Obtém ou define o valor total calculado.
        /// </summary>
        [DataMember]
        [JsonProperty("total")]
        public decimal Total { get; set; }
    }
}
