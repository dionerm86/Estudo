// <copyright file="IdCodigoDto.cs" company="Sync Softwares">
// Copyright (c) Sync Softwares. Todos os direitos reservados.
// </copyright>

using Newtonsoft.Json;
using System.Runtime.Serialization;

namespace Glass.API.Backend.Models.Genericas
{
    /// <summary>
    /// Classe que encapsula dados básicos (ID e código) de um item.
    /// </summary>
    [DataContract(Name = "IdCodigo")]
    public class IdCodigoDto : IdDto
    {
        /// <summary>
        /// Obtém ou define o código do item.
        /// </summary>
        [DataMember]
        [JsonProperty("codigo")]
        public string Codigo { get; set; }
    }
}
