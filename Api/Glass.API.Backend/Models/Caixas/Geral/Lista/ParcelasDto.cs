// <copyright file="ParcelasDto.cs" company="Sync Softwares">
// Copyright (c) Sync Softwares. Todos os direitos reservados.
// </copyright>

using Newtonsoft.Json;
using System.Runtime.Serialization;

namespace Glass.API.Backend.Models.Caixas.Geral.Lista
{
    /// <summary>
    /// Classe que encapsula dados dos valores de parcelas (contas a receber/recebidas).
    /// </summary>
    [DataContract(Name = "Parcelas")]
    public class ParcelasDto
    {
        /// <summary>
        /// Obtém ou define o total de contas a receber geradas.
        /// </summary>
        [DataMember]
        [JsonProperty("gerada")]
        public decimal Gerada { get; set; }

        /// <summary>
        /// Obtém ou define o total de contas recebidas contábeis.
        /// </summary>
        [DataMember]
        [JsonProperty("recebidaContabil")]
        public decimal RecebidaContabil { get; set; }

        /// <summary>
        /// Obtém ou define o total de contas recebidas não contábeis.
        /// </summary>
        [DataMember]
        [JsonProperty("recebidaNaoContabil")]
        public decimal RecebidaNaoContabil { get; set; }
    }
}
