// <copyright file="ProdutoMaoDeObraDto.cs" company="Sync Softwares">
// Copyright (c) Sync Softwares. Todos os direitos reservados.
// </copyright>

using Glass.API.Backend.Models.Genericas;
using Newtonsoft.Json;
using System.Runtime.Serialization;

namespace Glass.API.Backend.Models.Pedidos.AmbientesPedido.CadastroAtualizacao
{
    /// <summary>
    /// Classe que encapsula os dados de produto mão-de-obra do ambiente.
    /// </summary>
    public class ProdutoMaoDeObraDto
    {
        /// <summary>
        /// Obtém ou define o identificador do produto.
        /// </summary>
        [DataMember]
        [JsonProperty("id")]
        public int? Id { get; set; }

        /// <summary>
        /// Obtém ou define o código interno do produto.
        /// </summary>
        [DataMember]
        [JsonProperty("codigoInterno")]
        public string CodigoInterno { get; set; }

        /// <summary>
        /// Obtém ou define a quantidade de produtos.
        /// </summary>
        [DataMember]
        [JsonProperty("quantidade")]
        public double? Quantidade { get; set; }

        /// <summary>
        /// Obtém ou define a altura do produto.
        /// </summary>
        [DataMember]
        [JsonProperty("altura")]
        public double? Altura { get; set; }

        /// <summary>
        /// Obtém ou define a largura do produto.
        /// </summary>
        [DataMember]
        [JsonProperty("largura")]
        public int? Largura { get; set; }

        /// <summary>
        /// Obtém ou define os dados de processo.
        /// </summary>
        [DataMember]
        [JsonProperty("processo")]
        public IdCodigoDto Processo { get; set; }

        /// <summary>
        /// Obtém ou define os dados de aplicação.
        /// </summary>
        [DataMember]
        [JsonProperty("aplicacao")]
        public IdCodigoDto Aplicacao { get; set; }

        /// <summary>
        /// Obtém ou define um valor que indica se o ambiente é redondo.
        /// </summary>
        [DataMember]
        [JsonProperty("redondo")]
        public bool? Redondo { get; set; }
    }
}
