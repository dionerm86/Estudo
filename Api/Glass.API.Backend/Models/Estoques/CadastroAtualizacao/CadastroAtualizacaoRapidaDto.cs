// <copyright file="CadastroAtualizacaoRapidaDto.cs" company="Sync Softwares">
// Copyright (c) Sync Softwares. Todos os direitos reservados.
// </copyright>

using Newtonsoft.Json;
using System.Runtime.Serialization;

namespace Glass.API.Backend.Models.Estoques.CadastroAtualizacao
{
    /// <summary>
    /// Classe que encapsula os dados de atualização ddo estoque de um produto.
    /// </summary>
    [DataContract(Name = "CadastroAtualizacaoRapida")]
    public class CadastroAtualizacaoRapidaDto
    {
        /// <summary>
        /// Obtém ou define o identificador do produto.
        /// </summary>
        [DataMember]
        [JsonProperty("idProduto")]
        public int IdProduto { get; set; }

        /// <summary>
        /// Obtém ou define o identificador da loja de estoque.
        /// </summary>
        [DataMember]
        [JsonProperty("idLoja")]
        public int IdLoja{ get; set; }

        /// <summary>
        /// Obtém ou define a quantidade em estoque do produto.
        /// </summary>
        [DataMember]
        [JsonProperty("quantidadeEstoque")]
        public decimal? QuantidadeEstoque { get; set; }

        /// <summary>
        /// Obtém ou define a quantidade em estoque fiscal do produto.
        /// </summary>
        [DataMember]
        [JsonProperty("quantidadeEstoqueFiscal")]
        public decimal? QuantidadeEstoqueFiscal { get; set; }
    }
}