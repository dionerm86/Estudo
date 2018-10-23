// <copyright file="TipoCfopDto.cs" company="Sync Softwares">
// Copyright (c) Sync Softwares. Todos os direitos reservados.
// </copyright>

using Newtonsoft.Json;
using System.Runtime.Serialization;

namespace Glass.API.Backend.Models.Cfops.V1.Lista
{
    /// <summary>
    /// Classe que encapsula dados do tipo do CFOP.
    /// </summary>
    [DataContract(Name = "TipoCfop")]
    public class TipoCfopDto
    {
        /// <summary>
        /// Obtém ou define o ID do tipo do CFOP.
        /// </summary>
        [DataMember]
        [JsonProperty("idTipoCfop")]
        public int? IdTipoCfop { get; set; }

        /// <summary>
        /// Obtém ou define a descrição do tipo do CFOP.
        /// </summary>
        [DataMember]
        [JsonProperty("descricao")]
        public string Descricao { get; set; }
    }
}
