// <copyright file="ProdutoDto.cs" company="Sync Softwares">
// Copyright (c) Sync Softwares. Todos os direitos reservados.
// </copyright>

using Newtonsoft.Json;
using System.Runtime.Serialization;

namespace Glass.API.Backend.Models.Genericas.Venda
{
    /// <summary>
    /// Classe que encapsula os dados de produto de um produto de pedido.
    /// </summary>
    [DataContract(Name = "Produto")]
    public class ProdutoDto : IdCodigoDto
    {
        /// <summary>
        /// Obtém ou define a descrição do produto.
        /// </summary>
        [DataMember]
        [JsonProperty("descricao")]
        public string Descricao { get; set; }

        /// <summary>
        /// Obtém ou define a descrição (com a descrição do beneficiamento) do produto.
        /// </summary>
        [DataMember]
        [JsonProperty("descricaoComBeneficiamentos")]
        public string DescricaoComBeneficiamentos { get; set; }

        /// <summary>
        /// Obtém ou define a espessura do produto.
        /// </summary>
        [DataMember]
        [JsonProperty("espessura")]
        public double Espessura { get; set; }

        /// <summary>
        /// Obtém ou define um valor que indica se o produto é um vidro.
        /// </summary>
        [DataMember]
        [JsonProperty("vidro")]
        public bool Vidro { get; set; }
    }
}
