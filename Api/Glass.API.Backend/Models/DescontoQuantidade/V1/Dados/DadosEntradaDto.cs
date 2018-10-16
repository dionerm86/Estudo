// <copyright file="DadosEntradaDto.cs" company="Sync Softwares">
// Copyright (c) Sync Softwares. Todos os direitos reservados.
// </copyright>

using Newtonsoft.Json;
using System.Runtime.Serialization;

namespace Glass.API.Backend.Models.DescontoQuantidade.V1.Dados
{
    /// <summary>
    /// Classe que encapsula os dados de entrada para o controle de desconto por quantidade.
    /// </summary>
    [DataContract(Name = "DadosEntrada")]
    public class DadosEntradaDto
    {
        /// <summary>
        /// Obtém ou define o identificador do produto.
        /// </summary>
        [DataMember]
        [JsonProperty("idProduto")]
        public int IdProduto { get; set; }

        /// <summary>
        /// Obtém ou define o identificador do grupo do produto.
        /// </summary>
        [DataMember]
        [JsonProperty("idGrupoProduto")]
        public int? IdGrupoProduto { get; set; }

        /// <summary>
        /// Obtém ou define o identificador do subgrupo do produto.
        /// </summary>
        [DataMember]
        [JsonProperty("idSubgrupoProduto")]
        public int? IdSubgrupoProduto { get; set; }

        /// <summary>
        /// Obtém ou define o identificador do cliente.
        /// </summary>
        [DataMember]
        [JsonProperty("idCliente")]
        public int IdCliente { get; set; }

        /// <summary>
        /// Obtém ou define a quantidade de produtos.
        /// </summary>
        [DataMember]
        [JsonProperty("quantidade")]
        public double Quantidade { get; set; }
    }
}
