// <copyright file="TotaisChequesDto.cs" company="Sync Softwares">
// Copyright (c) Sync Softwares. Todos os direitos reservados.
// </copyright>

using Newtonsoft.Json;
using System.Runtime.Serialization;

namespace Glass.API.Backend.Models.Caixas.V1.Geral.Lista
{
    /// <summary>
    /// Classe que encapsula dados dos valores recebido no caixa.
    /// </summary>
    [DataContract(Name = "TotaisCheques")]
    public class TotaisChequesDto
    {
        /// <summary>
        /// Obtém ou define o total de cheques devolvidos.
        /// </summary>
        [DataMember]
        [JsonProperty("devolvido")]
        public decimal Devolvido { get; set; }

        /// <summary>
        /// Obtém ou define o total de cheques de terceiros.
        /// </summary>
        [DataMember]
        [JsonProperty("terceiro")]
        public decimal Terceiro { get; set; }

        /// <summary>
        /// Obtém ou define o total de cheques reapresentados.
        /// </summary>
        [DataMember]
        [JsonProperty("reapresentado")]
        public decimal Reapresentado { get; set; }
    }
}
