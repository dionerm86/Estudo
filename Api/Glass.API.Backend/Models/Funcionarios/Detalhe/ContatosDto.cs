// <copyright file="ContatosDto.cs" company="Sync Softwares">
// Copyright (c) Sync Softwares. Todos os direitos reservados.
// </copyright>

using Newtonsoft.Json;
using System.Runtime.Serialization;

namespace Glass.API.Backend.Models.Funcionarios.Detalhe
{
    /// <summary>
    /// Classe que encapsula os dados de contato.
    /// </summary>
    [DataContract(Name = "Contatos")]
    public class ContatosDto
    {
        /// <summary>
        /// Obtém ou define o telefone residencial de contato do funcionário.
        /// </summary>
        [DataMember]
        [JsonProperty("telefoneResidencial")]
        public string TelefoneResidencial { get; set; }

        /// <summary>
        /// Obtém ou define o telefone celular de contato do funcionário.
        /// </summary>
        [DataMember]
        [JsonProperty("telefoneCelular")]
        public string TelefoneCelular { get; set; }

        /// <summary>
        /// Obtém ou define o telefone de contato do funcionário.
        /// </summary>
        [DataMember]
        [JsonProperty("telefoneContato")]
        public string TelefoneContato { get; set; }

        /// <summary>
        /// Obtém ou define o email do funcionário.
        /// </summary>
        [DataMember]
        [JsonProperty("email")]
        public string Email { get; set; }

        /// <summary>
        /// Obtém ou define o ramal do funcionário.
        /// </summary>
        [DataMember]
        [JsonProperty("ramal")]
        public string Ramal { get; set; }
    }
}
