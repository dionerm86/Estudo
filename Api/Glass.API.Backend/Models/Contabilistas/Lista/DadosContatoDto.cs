// <copyright file="DadosContatoDto.cs" company="Sync Softwares">
// Copyright (c) Sync Softwares. Todos os direitos reservados.
// </copyright>

using Newtonsoft.Json;
using System.Runtime.Serialization;

namespace Glass.API.Backend.Models.Contabilistas.V1.Lista
{
    /// <summary>
    /// Classe que encapsula os dados de contato de um contabilista.
    /// </summary>
    [DataContract(Name = "DadosContato")]
    public class DadosContatoDto
    {
        /// <summary>
        /// Obtém ou define o telefone do contabilista.
        /// </summary>
        [DataMember]
        [JsonProperty("telefone")]
        public string Telefone { get; set; }

        /// <summary>
        /// Obtém ou define o fax do contabilista.
        /// </summary>
        [DataMember]
        [JsonProperty("fax")]
        public string Fax { get; set; }

        /// <summary>
        /// Obtém ou define o email do contabilista.
        /// </summary>
        [DataMember]
        [JsonProperty("email")]
        public string Email { get; set; }
    }
}
