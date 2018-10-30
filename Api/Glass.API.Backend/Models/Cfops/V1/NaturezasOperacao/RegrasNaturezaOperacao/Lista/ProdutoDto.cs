// <copyright file="ProdutoDto.cs" company="Sync Softwares">
// Copyright (c) Sync Softwares. Todos os direitos reservados.
// </copyright>

using Glass.API.Backend.Models.Genericas.V1;
using Newtonsoft.Json;
using System.Runtime.Serialization;

namespace Glass.API.Backend.Models.Cfops.V1.NaturezasOperacao.RegrasNaturezaOperacao.Lista
{
    /// <summary>
    /// Classe que encapsula dados do produto da regra de natureza de operação.
    /// </summary>
    [DataContract(Name = "Produto")]
    public class ProdutoDto
    {
        /// <summary>
        /// Obtém ou define o grupo de produto da regra de natureza de operação.
        /// </summary>
        [DataMember]
        [JsonProperty("grupoProduto")]
        public IdNomeDto GrupoProduto { get; set; }

        /// <summary>
        /// Obtém ou define o subgrupo de produto da regra de natureza de operação.
        /// </summary>
        [DataMember]
        [JsonProperty("subgrupoProduto")]
        public IdNomeDto SubgrupoProduto { get; set; }

        /// <summary>
        /// Obtém ou define as cores da regra de natureza de operação.
        /// </summary>
        [DataMember]
        [JsonProperty("cores")]
        public CoresDto Cores { get; set; }
    }
}
