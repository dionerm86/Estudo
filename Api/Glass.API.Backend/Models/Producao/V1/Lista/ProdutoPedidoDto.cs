// <copyright file="ProdutoPedidoDto.cs" company="Sync Softwares">
// Copyright (c) Sync Softwares. Todos os direitos reservados.
// </copyright>

using Glass.API.Backend.Models.Genericas;
using Newtonsoft.Json;
using System.Runtime.Serialization;

namespace Glass.API.Backend.Models.Producao.V1.Lista
{
    /// <summary>
    /// Classe que encapsula os dados do produto de pedido da peça.
    /// </summary>
    [DataContract(Name = "ProdutoPedido")]
    public class ProdutoPedidoDto : IdDto
    {
        /// <summary>
        /// Obtém ou define um valor que indica se o produto é composto.
        /// </summary>
        [DataMember]
        [JsonProperty("composto")]
        public bool Composto { get; set; }

        /// <summary>
        /// Obtém ou define a descrição do produto.
        /// </summary>
        [DataMember]
        [JsonProperty("descricao")]
        public string Descricao { get; set; }

        /// <summary>
        /// Obtém ou define a descrição dos beneficiamentos do produto.
        /// </summary>
        [DataMember]
        [JsonProperty("descricaoBeneficiamentos")]
        public string DescricaoBeneficiamentos { get; set; }
    }
}
