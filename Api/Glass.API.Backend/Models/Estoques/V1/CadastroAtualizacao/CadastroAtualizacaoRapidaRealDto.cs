// <copyright file="CadastroAtualizacaoRapidaRealDto.cs" company="Sync Softwares">
// Copyright (c) Sync Softwares. Todos os direitos reservados.
// </copyright>

using Newtonsoft.Json;
using System.Runtime.Serialization;

namespace Glass.API.Backend.Models.Estoques.V1.CadastroAtualizacao
{
    /// <summary>
    /// Classe que encapsula os dados de atualização do estoque de um produto.
    /// </summary>
    [DataContract(Name = "CadastroAtualizacaoRapidaReal")]
    public class CadastroAtualizacaoRapidaRealDto
    {
        /// <summary>
        /// Obtém ou define a quantidade em estoque do produto.
        /// </summary>
        [DataMember]
        [JsonProperty("quantidadeEstoque")]
        public decimal? QuantidadeEstoque { get; set; }
    }
}