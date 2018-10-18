// <copyright file="SituacaoDto.cs" company="Sync Softwares">
// Copyright (c) Sync Softwares. Todos os direitos reservados.
// </copyright>

using Newtonsoft.Json;
using System.Runtime.Serialization;

namespace Glass.API.Backend.Models.Produtos.V1.CadastroAtualizacao
{
    /// <summary>
    /// Classe que encapsula os dados de situação do produto.
    /// </summary>
    [DataContract(Name = "Situacao")]
    public class SituacaoDto
    {
        /// <summary>
        /// Obtém ou define a situação do produto.
        /// </summary>
        [DataMember]
        [JsonProperty("situacao")]
        public Situacao? Situacao { get; set; }
    }
}
