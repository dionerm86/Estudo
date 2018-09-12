// <copyright file="CadastroAtualizacaoDto.cs" company="Sync Softwares">
// Copyright (c) Sync Softwares. Todos os direitos reservados.
// </copyright>

using Newtonsoft.Json;
using System.Runtime.Serialization;

namespace Glass.API.Backend.Models.Estoques.CadastroAtualizacao
{
    /// <summary>
    /// Classe que encapsula os dados de atualização ddo estoque de um produto.
    /// </summary>
    [DataContract(Name = "CadastroAtualizacao")]
    public class CadastroAtualizacaoDto
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
        /// Obtém ou define a quantidade em estoque fiscal do produto.
        /// </summary>
        [DataMember]
        [JsonProperty("quantidadeEstoqueFiscal")]
        public decimal QuantidadeEstoqueFiscal { get; set; }

        /// <summary>
        /// Obtém ou define a quantidade com defeito do produto.
        /// </summary>
        [DataMember]
        [JsonProperty("quantidadeDefeito")]
        public decimal QuantidadeDefeito { get; set; }

        /// <summary>
        /// Obtém ou define a quantidade do produto em posse de terceiros.
        /// </summary>
        [DataMember]
        [JsonProperty("quantidadePosseTerceiros")]
        public decimal QuantidadePosseTerceiros { get; set; }

        /// <summary>
        /// Obtém ou define o identificador do cliente em posse de estoque do produto.
        /// </summary>
        [DataMember]
        [JsonProperty("idCliente")]
        public int? IdCliente { get; set; }

        /// <summary>
        /// Obtém ou define o identificador do fornecedor em posse de estoque do produto.
        /// </summary>
        [DataMember]
        [JsonProperty("idFornecedor")]
        public int? IdFornecedor { get; set; }

        /// <summary>
        /// Obtém ou define o identificador da loja em posse de estoque do produto.
        /// </summary>
        [DataMember]
        [JsonProperty("idLojaTerceiros")]
        public int? IdLojaTerceiros { get; set; }

        /// <summary>
        /// Obtém ou define o identificador do transportador em posse de estoque do produto.
        /// </summary>
        [DataMember]
        [JsonProperty("idTransportador")]
        public int? IdTransportador { get; set; }

        /// <summary>
        /// Obtém ou define o identificador da administradora de cartão em posse de estoque do produto.
        /// </summary>
        [DataMember]
        [JsonProperty("idAdministradoraCartao")]
        public int? IdAdministradoraCartao { get; set; }
    }
}