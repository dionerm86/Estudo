// <copyright file="TelefonesDto.cs" company="Sync Softwares">
// Copyright (c) Sync Softwares. Todos os direitos reservados.
// </copyright>

using Newtonsoft.Json;
using System.Runtime.Serialization;

namespace Glass.API.Backend.Models.Comissionados.Lista
{
    /// <summary>
    /// Classe que encapsula os telefones do comissionado.
    /// </summary>
    [DataContract(Name = "Telefones")]
    public class TelefonesDto
    {
        /// <summary>
        /// Obtém ou define o telefone residencial.
        /// </summary>
        [DataMember]
        [JsonProperty("residencial")]
        public string Residencial { get; set; }

        /// <summary>
        /// Obtém ou define o telefone celular.
        /// </summary>
        [DataMember]
        [JsonProperty("celular")]
        public string Celular { get; set; }
    }
}
