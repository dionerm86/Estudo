// <copyright file="EntregaDto.cs" company="Sync Softwares">
// Copyright (c) Sync Softwares. Todos os direitos reservados.
// </copyright>

using Newtonsoft.Json;
using System;
using System.Runtime.Serialization;

namespace Glass.API.Backend.Models.Pedidos.V1.CadastroAtualizacao
{
    /// <summary>
    /// Classe que encapsula as duas datas de entrega que o pedido pode ter, dependendo se for ou não fast delivery.
    /// </summary>
    [DataContract(Name = "DataEntregaMinima")]
    public class DataEntregaMinimaDto
    {
        /// <summary>
        /// Obtém ou define a data de entrega caso o pedido seja fast delivery.
        /// </summary>
        [DataMember]
        [JsonProperty("dataFastDelivery")]
        public DateTime? DataFastDelivery { get; set; }

        /// <summary>
        /// Obtém ou define a data de entrega mínima do pedido.
        /// </summary>
        [DataMember]
        [JsonProperty("dataMinimaCalculada")]
        public DateTime? DataMinimaCalculada { get; set; }

        /// <summary>
        /// Obtém ou define a data de entrega mínima do pedido que o usuário poderá selecionar.
        /// </summary>
        [DataMember]
        [JsonProperty("dataMinimaPermitida")]
        public DateTime? DataMinimaPermitida { get; set; }

        /// <summary>
        /// Obtém ou define se o campo de data de entrega deverá ser desabilitado.
        /// </summary>
        [DataMember]
        [JsonProperty("desabilitarCampo")]
        public bool? DesabilitarCampo { get; set; }
    }
}
