// <copyright file="CnabDto.cs" company="Sync Softwares">
// Copyright (c) Sync Softwares. Todos os direitos reservados.
// </copyright>

using Newtonsoft.Json;
using System.Runtime.Serialization;

namespace Glass.API.Backend.Models.ContasBancarias.V1.Lista
{
    /// <summary>
    /// Classe que encapsula dados do CNAB.
    /// </summary>
    [DataContract(Name = "Cnab")]
    public class CnabDto
    {
        /// <summary>
        /// Obtém ou define o código do cliente.
        /// </summary>
        [DataMember]
        [JsonProperty("codigoCliente")]
        public string CodigoCliente { get; set; }

        /// <summary>
        /// Obtém ou define o posto.
        /// </summary>
        [DataMember]
        [JsonProperty("posto")]
        public int? Posto { get; set; }
    }
}
