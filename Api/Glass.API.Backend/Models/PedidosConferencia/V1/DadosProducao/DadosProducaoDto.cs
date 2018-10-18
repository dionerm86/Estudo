// <copyright file="DadosProducaoDto.cs" company="Sync Softwares">
// Copyright (c) Sync Softwares. Todos os direitos reservados.
// </copyright>

using Newtonsoft.Json;
using System.Runtime.Serialization;

namespace Glass.API.Backend.Models.PedidosConferencia.V1.DadosProducao
{
    /// <summary>
    /// Classe que encapsula os dados de produção de um pedido.
    /// </summary>
    [DataContract(Name = "DadosProducao")]
    public class DadosProducaoDto
    {
        /// <summary>
        /// Obtém ou define um valor que indica se o pedido é de produção.
        /// </summary>
        [DataMember]
        [JsonProperty("pedidoProducao")]
        public bool PedidoProducao { get; set; }

        /// <summary>
        /// Obtém ou define a quantidade de peças para estoque do pedido.
        /// </summary>
        [DataMember]
        [JsonProperty("quantidadePecasVidroParaEstoque")]
        public long QuantidadePecasVidroParaEstoque { get; set; }

        /// <summary>
        /// Obtém ou define um valor que indica se o pedido possui etiquetas não impressas.
        /// </summary>
        [DataMember]
        [JsonProperty("possuiEtiquetasNaoImpressas")]
        public bool PossuiEtiquetasNaoImpressas { get; set; }
    }
}
