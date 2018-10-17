// <copyright file="ParcelasDto.cs" company="Sync Softwares">
// Copyright (c) Sync Softwares. Todos os direitos reservados.
// </copyright>

using Newtonsoft.Json;
using System.Collections.Generic;
using System.Runtime.Serialization;

namespace Glass.API.Backend.Models.Pedidos.V1.CadastroAtualizacao
{
    /// <summary>
    /// Classe que encapsula os dados de parcelas.
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
        /// Obtém ou define o número de parcelas escolhido.
        /// </summary>
        [DataMember]
        [JsonProperty("numeroParcelas")]
        public int? NumeroParcelas { get; set; }

        /// <summary>
        /// Obtém ou define os detalhes das parcelas do pedido.
        /// </summary>
        [DataMember]
        [JsonProperty("detalhes")]
        public IEnumerable<DetalheParcelaDto> Detalhes { get; set; }
    }
}
