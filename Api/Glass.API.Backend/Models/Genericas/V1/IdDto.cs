// <copyright file="IdDto.cs" company="Sync Softwares">
// Copyright (c) Sync Softwares. Todos os direitos reservados.
// </copyright>

using Newtonsoft.Json;
using System.Runtime.Serialization;

namespace Glass.API.Backend.Models.Genericas.V1
{
    /// <summary>
    /// Classe que encapsula o identificador de um item.
    /// </summary>
    [DataContract(Name = "Id")]
    public class IdDto
    {
        /// <summary>
        /// Obtém ou define o identificador do item.
        /// </summary>
        [DataMember]
        [JsonProperty("id")]
        public int? Id { get; set; }
    }
}
