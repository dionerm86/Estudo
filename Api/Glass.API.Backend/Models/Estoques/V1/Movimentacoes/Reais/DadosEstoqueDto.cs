// <copyright file="DadosEstoqueDto.cs" company="Sync Softwares">
// Copyright (c) Sync Softwares. Todos os direitos reservados.
// </copyright>

using Newtonsoft.Json;
using System;
using System.Runtime.Serialization;

namespace Glass.API.Backend.Models.Genericas.V1
{
    /// <summary>
    /// Classe que encapsula data de cadastro e movimentação de estoque.
    /// </summary>
    [DataContract(Name = "DadosEstoque")]
    public class DadosEstoqueDto
    {
        /// <summary>
        /// Obtém ou define o tipo da movimentação.
        /// </summary>
        [DataMember]
        [JsonProperty("tipoMovimentacao")]
        public string TipoMovimentacao { get; set; }

        /// <summary>
        /// Obtém ou define a unidade de medida.
        /// </summary>
        [DataMember]
        [JsonProperty("unidade")]
        public string Unidade { get; set; }

        /// <summary>
        /// Obtém ou define a quantidade.
        /// </summary>
        [DataMember]
        [JsonProperty("quantidade")]
        public decimal Quantidade { get; set; }

        /// <summary>
        /// Obtém ou define o saldo de quantidade.
        /// </summary>
        [DataMember]
        [JsonProperty("saldoQuantidade")]
        public decimal SaldoQuantidade { get; set; }

        /// <summary>
        /// Obtém ou define o valor.
        /// </summary>
        [DataMember]
        [JsonProperty("valor")]
        public decimal Valor { get; set; }

        /// <summary>
        /// Obtém ou define o saldo valor.
        /// </summary>
        [DataMember]
        [JsonProperty("saldoValor")]
        public decimal SaldoValor { get; set; }
    }
}
