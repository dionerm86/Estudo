// <copyright file="QuantidadeDto.cs" company="Sync Softwares">
// Copyright (c) Sync Softwares. Todos os direitos reservados.
// </copyright>

using Newtonsoft.Json;
using System.Runtime.Serialization;

namespace Glass.API.Backend.Models.Produtos.Filtro
{
    /// <summary>
    /// Classe que encapsula as informações sobre a quantidade do produto.
    /// </summary>
    [DataContract(Name = "Quantidade")]
    public class QuantidadeDto
    {
        /// <summary>
        /// Obtém ou define um valor que indica se o campo pode ser decimal.
        /// </summary>
        [DataMember]
        [JsonProperty("permiteDecimal")]
        public bool PermiteDecimal { get; set; }
    }
}
