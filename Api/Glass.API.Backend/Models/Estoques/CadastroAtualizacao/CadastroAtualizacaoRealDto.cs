// <copyright file="CadastroAtualizacaoRealDto.cs" company="Sync Softwares">
// Copyright (c) Sync Softwares. Todos os direitos reservados.
// </copyright>

using Newtonsoft.Json;
using System.Runtime.Serialization;

namespace Glass.API.Backend.Models.Estoques.CadastroAtualizacao
{
    /// <summary>
    /// Classe que encapsula os dados de atualização do estoque real de um produto.
    /// </summary>
    [DataContract(Name = "CadastroAtualizacaoReal")]
    public class CadastroAtualizacaoRealDto
    {
        /// <summary>
        /// Obtém ou define o estoque mínimo do produto.
        /// </summary>
        [DataMember]
        [JsonProperty("estoqueMinimo")]
        public decimal EstoqueMinimo { get; set; }

        /// <summary>
        /// Obtém ou define o estoque em m² do produto.
        /// </summary>
        [DataMember]
        [JsonProperty("estoqueM2")]
        public decimal EstoqueM2 { get; set; }

        /// <summary>
        /// Obtém ou define a quantidade em estoque do produto.
        /// </summary>
        [DataMember]
        [JsonProperty("quantidadeEstoque")]
        public decimal QuantidadeEstoque { get; set; }

        /// <summary>
        /// Obtém ou define a quantidade com defeito do produto.
        /// </summary>
        [DataMember]
        [JsonProperty("quantidadeDefeito")]
        public decimal QuantidadeDefeito { get; set; }
    }
}