// <copyright file="ContatosDto.cs" company="Sync Softwares">
// Copyright (c) Sync Softwares. Todos os direitos reservados.
// </copyright>

using Newtonsoft.Json;
using System.Runtime.Serialization;

namespace Glass.API.Backend.Models.Funcionarios.Lista
{
    /// <summary>
    /// Classe que encapsula dados de contato do funcionário.
    /// </summary>
    [DataContract(Name = "Contatos")]
    public class ContatosDto
    {
        /// <summary>
        /// Obtém ou define o telefone residencial do funcionário.
        /// </summary>
        [DataMember]
        [JsonProperty("telefoneResidencial")]
        public string TelefoneResidencial { get; set; }

        /// <summary>
        /// Obtém ou define o telefone celular do fucionário.
        /// </summary>
        [DataMember]
        [JsonProperty("telefoneCelular")]
        public string TelefoneCelular { get; set; }
    }
}
