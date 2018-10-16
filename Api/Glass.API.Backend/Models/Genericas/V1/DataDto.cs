// <copyright file="DataDto.cs" company="Sync Softwares">
// Copyright (c) Sync Softwares. Todos os direitos reservados.
// </copyright>

using Newtonsoft.Json;
using System;
using System.Runtime.Serialization;

namespace Glass.API.Backend.Models.Genericas.V1
{
    /// <summary>
    /// Classe que encapsula data e nome do funcionário que realizou uma operação no sistema.
    /// </summary>
    [DataContract(Name = "Data")]
    public class DataDto
    {
        /// <summary>
        /// Obtém ou define a data da operação.
        /// </summary>
        [DataMember]
        [JsonProperty("data")]
        public DateTime Data { get; set; }
    }
}
