// <copyright file="PerdaDto.cs" company="Sync Softwares">
// Copyright (c) Sync Softwares. Todos os direitos reservados.
// </copyright>

using Newtonsoft.Json;
using System.Runtime.Serialization;

namespace Glass.API.Backend.Models.Producao.V1.Setores.Lista
{
    /// <summary>
    /// Classe que encapsula dados de perda do setor.
    /// </summary>
    [DataContract(Name = "Perda")]
    public class PerdaDto
    {
        /// <summary>
        /// Obtém ou define o desafio de perda do setor.
        /// </summary>
        [DataMember]
        [JsonProperty("desafio")]
        public decimal Desafio { get; set; }

        /// <summary>
        /// Obtém ou define a meta de perda do setor.
        /// </summary>
        [DataMember]
        [JsonProperty("meta")]
        public decimal Meta { get; set; }
    }
}
