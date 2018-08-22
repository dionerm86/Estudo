// <copyright file="FormaPagamentoDto.cs" company="Sync Softwares">
// Copyright (c) Sync Softwares. Todos os direitos reservados.
// </copyright>

using Glass.API.Backend.Models.Genericas;
using Newtonsoft.Json;
using System.Runtime.Serialization;

namespace Glass.API.Backend.Models.Pedidos.Detalhe
{
    /// <summary>
    /// Classe que encapsula os dados de forma de pagamento.
    /// </summary>
    public class FormaPagamentoDto : IdNomeDto
    {
        /// <summary>
        /// Obtém ou define os dados de parcelas do pedido.
        /// </summary>
        [DataMember]
        [JsonProperty("parcelas")]
        public ParcelasDto Parcelas { get; set; }

        /// <summary>
        /// Obtém ou define o identificador do tipo de cartão.
        /// </summary>
        [DataMember]
        [JsonProperty("idTipoCartao")]
        public int? IdTipoCartao { get; set; }
    }
}
