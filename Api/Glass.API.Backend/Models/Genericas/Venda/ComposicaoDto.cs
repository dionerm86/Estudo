// <copyright file="ComposicaoDto.cs" company="Sync Softwares">
// Copyright (c) Sync Softwares. Todos os direitos reservados.
// </copyright>

using Newtonsoft.Json;
using System.Collections.Generic;
using System.Runtime.Serialization;

namespace Glass.API.Backend.Models.Genericas.Venda
{
    /// <summary>
    /// Classe que encapsula os dados de composição do produto.
    /// </summary>
    [DataContract(Name = "Composicao")]
    public class ComposicaoDto
    {
        /// <summary>
        /// Obtém ou define um valor que indica se o produto possui filhos (no caso de produto composição/laminado).
        /// </summary>
        [DataMember]
        [JsonProperty("possuiFilhos")]
        public bool? PossuiFilhos { get; set; }

        /// <summary>
        /// Obtém ou define um valor que indica se os produtos de composição podem ser inseridos.
        /// </summary>
        [DataMember]
        [JsonProperty("permitirInserirFilhos")]
        public bool? PermitirInserirFilhos { get; set; }

        /// <summary>
        /// Obtém ou define um valor que indica se os beneficiamentos serão aplicados na composição.
        /// </summary>
        [DataMember]
        [JsonProperty("aplicarBeneficiamentosProdutosFilhos")]
        public bool? AplicarBeneficiamentosProdutosFilhos { get; set; }

        /// <summary>
        /// Obtém ou define um valor que indica todos os identificadores de subgrupos dos produtos da composição.
        /// </summary>
        [DataMember]
        [JsonProperty("idsSubgruposProdutosFilhos")]
        public IEnumerable<int> IdsSubgruposProdutosFilhos { get; set; }
    }
}
