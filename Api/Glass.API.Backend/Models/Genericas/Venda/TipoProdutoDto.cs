// <copyright file="TipoProdutoDto.cs" company="Sync Softwares">
// Copyright (c) Sync Softwares. Todos os direitos reservados.
// </copyright>

using Newtonsoft.Json;
using System.Runtime.Serialization;

namespace Glass.API.Backend.Models.Genericas.Venda
{
    /// <summary>
    /// Classe que encapsula os dados do tipo de produto.
    /// </summary>
    [DataContract(Name = "TipoProduto")]
    public class TipoProdutoDto
    {
        /// <summary>
        /// Obtém ou define um valor que indica se o produto é laminado ou composto.
        /// </summary>
        [DataMember]
        [JsonProperty("laminadoOuComposicao")]
        public bool LaminadoOuComposicao { get; set; }

        /// <summary>
        /// Obtém ou define um valor que indica se o produto é um vidro.
        /// </summary>
        [DataMember]
        [JsonProperty("vidro")]
        public bool Vidro { get; set; }

        /// <summary>
        /// Obtém ou define um valor que indica se o produto é um alumínio.
        /// </summary>
        [DataMember]
        [JsonProperty("aluminio")]
        public bool Aluminio { get; set; }
    }
}
