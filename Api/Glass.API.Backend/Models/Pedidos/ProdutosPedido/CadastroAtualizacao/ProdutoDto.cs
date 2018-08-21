// <copyright file="ProdutoDto.cs" company="Sync Softwares">
// Copyright (c) Sync Softwares. Todos os direitos reservados.
// </copyright>

using Newtonsoft.Json;
using System.Runtime.Serialization;

namespace Glass.API.Backend.Models.Pedidos.ProdutosPedido.CadastroAtualizacao
{
    /// <summary>
    /// Classe que encapsula os dados de produto.
    /// </summary>
    [DataContract(Name = "Produto")]
    public class ProdutoDto
    {
        /// <summary>
        /// Obtém ou define o identificador do produto.
        /// </summary>
        [DataMember]
        [JsonProperty("id")]
        public int? Id { get; set; }

        /// <summary>
        /// Obtém ou define a espessura do produto.
        /// </summary>
        [DataMember]
        [JsonProperty("espessura")]
        public double? Espessura { get; set; }
    }
}
