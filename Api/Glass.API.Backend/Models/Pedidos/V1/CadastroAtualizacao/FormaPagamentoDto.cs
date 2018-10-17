// <copyright file="FormaPagamentoDto.cs" company="Sync Softwares">
// Copyright (c) Sync Softwares. Todos os direitos reservados.
// </copyright>

using Newtonsoft.Json;
using System.Runtime.Serialization;

namespace Glass.API.Backend.Models.Pedidos.V1.CadastroAtualizacao
{
    /// <summary>
    /// Classe que encapsula os dados das formas de pagamento do pedido.
    /// </summary>
    [DataContract(Name = "FormaPagamento")]
    public class FormaPagamentoDto
    {
        /// <summary>
        /// Obtém ou define o identificador da forma de pagamento do pedido.
        /// </summary>
        [DataMember]
        [JsonProperty("id")]
        public int? Id { get; set; }

        /// <summary>
        /// Obtém ou define o identificador do tipo de cartão do pedido.
        /// </summary>
        [DataMember]
        [JsonProperty("idTipoCartao")]
        public int? IdTipoCartao { get; set; }

        /// <summary>
        /// Obtém ou define os detalhes das parcelas do pedido.
        /// </summary>
        [DataMember]
        [JsonProperty("parcelas")]
        public ParcelasDto Parcelas { get; set; }
    }
}
