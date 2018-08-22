// <copyright file="BeneficiamentosDto.cs" company="Sync Softwares">
// Copyright (c) Sync Softwares. Todos os direitos reservados.
// </copyright>

using Newtonsoft.Json;
using System.Collections.Generic;
using System.Runtime.Serialization;

namespace Glass.API.Backend.Models.Genericas
{
    /// <summary>
    /// Classe que encapsula os dados de beneficiamentos.
    /// </summary>
    [DataContract(Name = "Beneficiamentos")]
    public class BeneficiamentosDto
    {
        /// <summary>
        /// Obtém ou define o valor dos beneficiamentos.
        /// </summary>
        [DataMember]
        [JsonProperty("valor")]
        public decimal Valor { get; set; }

        /// <summary>
        /// Obtém ou define a altura do beneficiamento.
        /// </summary>
        [DataMember]
        [JsonProperty("altura")]
        public int? Altura { get; set; }

        /// <summary>
        /// Obtém ou define a largura do beneficiamento.
        /// </summary>
        [DataMember]
        [JsonProperty("largura")]
        public int? Largura { get; set; }

        /// <summary>
        /// Obtém ou define a espessura do beneficiamento.
        /// </summary>
        [DataMember]
        [JsonProperty("espessura")]
        public double? Espessura { get; set; }

        /// <summary>
        /// Obtém ou define um valor que indica se o produto é redondo.
        /// </summary>
        [DataMember]
        [JsonProperty("redondo")]
        public bool? Redondo { get; set; }

        /// <summary>
        /// Obtém ou define os itens de beneficiamentos.
        /// </summary>
        [DataMember]
        [JsonProperty("itens")]
        public IEnumerable<ItemBeneficiamentoDto> Itens { get; set; }
    }
}
