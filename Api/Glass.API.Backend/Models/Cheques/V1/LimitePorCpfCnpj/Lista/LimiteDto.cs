// <copyright file="LimiteDto.cs" company="Sync Softwares">
// Copyright (c) Sync Softwares. Todos os direitos reservados.
// </copyright>

using Newtonsoft.Json;
using System.Runtime.Serialization;

namespace Glass.API.Backend.Models.Cheques.V1.LimitePorCpfCnpj.Lista
{
    /// <summary>
    /// Classe que encapsula os dados referentes ao limite de um item da lista de limites de cheques por cpf/cnpj.
    /// </summary>
    [DataContract(Name = "Limite")]
    public class LimiteDto
    {
        /// <summary>
        /// Obtém ou define total do limite.
        /// </summary>
        [DataMember]
        [JsonProperty("total")]
        public decimal? Total { get; set; }

        /// <summary>
        /// Obtém ou define o total utilizado do limite.
        /// </summary>
        [DataMember]
        [JsonProperty("utilizado")]
        public decimal? Utilizado { get; set; }

        /// <summary>
        /// Obtém ou define o total restante do limite.
        /// </summary>
        [DataMember]
        [JsonProperty("restante")]
        public decimal? Restante { get; set; }
    }
}