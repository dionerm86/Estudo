// <copyright file="CustosDto.cs" company="Sync Softwares">
// Copyright (c) Sync Softwares. Todos os direitos reservados.
// </copyright>

using Newtonsoft.Json;
using System.Runtime.Serialization;

namespace Glass.API.Backend.Models.Produtos.V1.Lista
{
    /// <summary>
    /// Classe que encapsula os custos do produto.
    /// </summary>
    [DataContract(Name = "Custos")]
    public class CustosDto
    {
        /// <summary>
        /// Obtém ou define o custo de fornecedor do produto.
        /// </summary>
        [DataMember]
        [JsonProperty("fornecedor")]
        public decimal Fornecedor { get; set; }

        /// <summary>
        /// Obtém ou define o custo com impostos do produto.
        /// </summary>
        [DataMember]
        [JsonProperty("comImpostos")]
        public decimal ComImpostos { get; set; }
    }
}
