// <copyright file="DocumentosDto.cs" company="Sync Softwares">
// Copyright (c) Sync Softwares. Todos os direitos reservados.
// </copyright>

using Newtonsoft.Json;
using System.Runtime.Serialization;

namespace Glass.API.Backend.Models.Funcionarios.V1.Lista
{
    /// <summary>
    /// Classe que encapsula dados dos documentos do funcionário.
    /// </summary>
    [DataContract(Name = "Documentos")]
    public class DocumentosDto
    {
        /// <summary>
        /// Obtém ou define o rg do funcionário.
        /// </summary>
        [DataMember]
        [JsonProperty("rg")]
        public string Rg { get; set; }

        /// <summary>
        /// Obtém ou define o cpf do fucionário.
        /// </summary>
        [DataMember]
        [JsonProperty("cpf")]
        public string Cpf { get; set; }
    }
}
