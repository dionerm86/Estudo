// <copyright file="ParcelasDto.cs" company="Sync Softwares">
// Copyright (c) Sync Softwares. Todos os direitos reservados.
// </copyright>

using Newtonsoft.Json;
using System.Collections.Generic;
using System.Runtime.Serialization;

namespace Glass.API.Backend.Models.Pedidos.V1.Detalhe
{
    /// <summary>
    /// Classe que encapsula dados de parcelas do pedido.
    /// </summary>
    [DataContract(Name = "Parcelas")]
    public class ParcelasDto
    {
        /// <summary>
        /// Obtém ou define o identificador da parcela selecionada.
        /// </summary>
        [DataMember]
        [JsonProperty("id")]
        public int? Id { get; set; }

        /// <summary>
        /// Obtém ou define o número de parcelas do pedido.
        /// </summary>
        [DataMember]
        [JsonProperty("numeroParcelas")]
        public int NumeroParcelas { get; set; }

        /// <summary>
        /// Obtém ou define a quantidade de dias entre as parcelas (Ex.: 15,30,45).
        /// </summary>
        [DataMember]
        [JsonProperty("dias")]
        public string Dias { get; set; }

        /// <summary>
        /// Obtém ou define se a parcela é à vista.
        /// </summary>
        [DataMember]
        [JsonProperty("parcelaAVista")]
        public bool ParcelaAVista { get; set; }

        /// <summary>
        /// Obtém ou define os detalhes das parcelas do pedido.
        /// </summary>
        [DataMember]
        [JsonProperty("detalhes")]
        public IEnumerable<DetalheParcelaDto> Detalhes { get; set; }
    }
}
