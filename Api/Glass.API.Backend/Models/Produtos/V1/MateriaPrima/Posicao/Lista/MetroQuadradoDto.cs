// <copyright file="MetroQuadradoDto.cs" company="Sync Softwares">
// Copyright (c) Sync Softwares. Todos os direitos reservados.
// </copyright>

using Newtonsoft.Json;
using System.Runtime.Serialization;

namespace Glass.API.Backend.Models.Produtos.V1.MateriaPrima.Posicao.Lista
{
    /// <summary>
    /// Classe que encapsula os dados referêntes ao metro quadrado da posição de matéria prima.
    /// </summary>
    [DataContract(Name = "MetroQuadrado")]
    public class MetroQuadradoDto
    {
        /// <summary>
        /// Obtém ou define o total em metros quadrados referente a posição de matéria prima.
        /// </summary>
        [DataMember]
        [JsonProperty("total")]
        public decimal? Total { get; set; }

        /// <summary>
        /// Obtém ou define o total com etiqueta impressa em metros quadrados referente a posição de matéria prima.
        /// </summary>
        [DataMember]
        [JsonProperty("comEtiquetaImpressa")]
        public decimal? ComEtiquetaImpressa { get; set; }

        /// <summary>
        /// Obtém ou define o total sem etiqueta impressa em metros quadrados referente a posição de matéria prima.
        /// </summary>
        [DataMember]
        [JsonProperty("semEtiquetaImpressa")]
        public decimal? SemEtiquetaImpressa { get; set; }

        /// <summary>
        /// Obtém ou define o total em metros quadrados expedido em pedidos de venda referente a posição de matéria prima.
        /// </summary>
        [DataMember]
        [JsonProperty("pedidoDeVenda")]
        public decimal? PedidoDeVenda { get; set; }

        /// <summary>
        /// Obtém ou define o total em metros quadrados expedido em pedidos de produção referente a posição de matéria prima.
        /// </summary>
        [DataMember]
        [JsonProperty("pedidoDeProducao")]
        public decimal? PedidoDeProducao { get; set; }

        /// <summary>
        /// Obtém ou define o total em metros quadrados presente no estoque referente a posição da matéria prima.
        /// </summary>
        [DataMember]
        [JsonProperty("emEstoque")]
        public decimal? EmEstoque { get; set; }

        /// <summary>
        /// Obtém ou define o total em metros quadrados disponível referente a posição da matéria prima.
        /// </summary>
        [DataMember]
        [JsonProperty("disponivel")]
        public decimal? Disponivel { get; set; }
    }
}
