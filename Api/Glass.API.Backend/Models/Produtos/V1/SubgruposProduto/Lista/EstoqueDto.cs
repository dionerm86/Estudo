// <copyright file="EstoqueDto.cs" company="Sync Softwares">
// Copyright (c) Sync Softwares. Todos os direitos reservados.
// </copyright>

using Newtonsoft.Json;
using System.Runtime.Serialization;

namespace Glass.API.Backend.Models.Produtos.V1.SubgruposProduto.Lista
{
    /// <summary>
    /// Classe que encapsula dados de estoque do subgrupo.
    /// </summary>
    [DataContract(Name = "Estoque")]
    public class EstoqueDto : GruposProduto.Lista.EstoqueDto
    {
        /// <summary>
        /// Obtém ou define um valor que indica se os produtos deste subgrupo serão para estoque.
        /// </summary>
        [DataMember]
        [JsonProperty("produtoParaEstoque")]
        public bool ProdutoParaEstoque { get; set; }

        /// <summary>
        /// Obtém ou define um valor que indica se será permitido vender os produtos deste subgrupo pelo eCommerce.
        /// </summary>
        [DataMember]
        [JsonProperty("bloquearVendaECommerce")]
        public bool BloquearVendaECommerce { get; set; }
    }
}
