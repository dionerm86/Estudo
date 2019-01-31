// <copyright file="ProdutoDto.cs" company="Sync Softwares">
// Copyright (c) Sync Softwares. Todos os direitos reservados.
// </copyright>

using Newtonsoft.Json;
using System.Runtime.Serialization;

namespace Glass.API.Backend.Models.Producao.V1.Fornadas.PecasFornada
{
    /// <summary>
    /// Classe que encapsula os dados referentes ao produto da peça de fornada.
    /// </summary>
    [DataContract(Name = "Produto")]
    public class ProdutoDto
    {
        /// <summary>
        /// Obtém ou define o código do produto.
        /// </summary>
        [DataMember]
        [JsonProperty("codigo")]
        public string Codigo { get; set; }

        /// <summary>
        /// Obtém ou define o nome/descrição do produto.
        /// </summary>
        [DataMember]
        [JsonProperty("nome")]
        public string Nome { get; set; }
    }
}