// <copyright file="CadastroAtualizacaoRapidaFiscal.cs" company="Sync Softwares">
// Copyright (c) Sync Softwares. Todos os direitos reservados.
// </copyright>

using Newtonsoft.Json;
using System.Runtime.Serialization;

namespace Glass.API.Backend.Models.Estoques.CadastroAtualizacao
{
    /// <summary>
    /// Classe que encapsula os dados de atualização do estoque fiscal de um produto.
    /// </summary>
    [DataContract(Name = "CadastroAtualizacaoRapidaFiscal")]
    public class CadastroAtualizacaoRapidaFiscalDto
    {
        /// <summary>
        /// Obtém ou define a quantidade em estoque fiscal do produto.
        /// </summary>
        [DataMember]
        [JsonProperty("quantidadeEstoqueFiscal")]
        public decimal? QuantidadeEstoqueFiscal { get; set; }
    }
}