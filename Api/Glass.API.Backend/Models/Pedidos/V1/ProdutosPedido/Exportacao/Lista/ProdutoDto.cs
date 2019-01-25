// <copyright file="ProdutoDto.cs" company="Sync Softwares">
// Copyright (c) Sync Softwares. Todos os direitos reservados.
// </copyright>

using Newtonsoft.Json;
using System.Runtime.Serialization;

namespace Glass.API.Backend.Models.Pedidos.V1.ProdutosPedido.Exportacao.Lista
{
    /// <summary>
    /// Classe que encapsula os dados de um produto para a lista de produtos de pedido de exportação.
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
        /// Obtém ou define a descrição do produto.
        /// </summary>
        [DataMember]
        [JsonProperty("descricao")]
        public string Descricao { get; set; }
    }
}