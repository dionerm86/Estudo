// <copyright file="EstoqueDto.cs" company="Sync Softwares">
// Copyright (c) Sync Softwares. Todos os direitos reservados.
// </copyright>

using Newtonsoft.Json;
using System.Runtime.Serialization;

namespace Glass.API.Backend.Models.Produtos.GruposProduto.Lista
{
    /// <summary>
    /// Classe que encapsula os custos do produto.
    /// </summary>
    [DataContract(Name = "Estoque")]
    public class EstoqueDto
    {
        /// <summary>
        /// Obtém ou define um valor que indica se irá bloquear venda de produto deste grupo caso esteja sem estoque.
        /// </summary>
        [DataMember]
        [JsonProperty("bloquearEstoque")]
        public bool BloquearEstoque { get; set; }

        /// <summary>
        /// Obtém ou define um valor que indica se os produtos deste grupo irão alterar o estoque.
        /// </summary>
        [DataMember]
        [JsonProperty("alterarEstoque")]
        public bool AlterarEstoque { get; set; }

        /// <summary>
        /// Obtém ou define um valor que indica se os produtos deste grupo irão alterar o estoque fiscal.
        /// </summary>
        [DataMember]
        [JsonProperty("alterarEstoqueFiscal")]
        public bool AlterarEstoqueFiscal { get; set; }

        /// <summary>
        /// Obtém ou define um valor que indica se será exibida mensagem de falta de estoque dos produtos deste grupo caso estejam sem estoque.
        /// </summary>
        [DataMember]
        [JsonProperty("exibirMensagemEstoque")]
        public bool ExibirMensagemEstoque { get; set; }

        /// <summary>
        /// Obtém ou define um valor que indica se será gerado volume dos produtos deste grupo.
        /// </summary>
        [DataMember]
        [JsonProperty("geraVolume")]
        public bool GeraVolume { get; set; }
    }
}
