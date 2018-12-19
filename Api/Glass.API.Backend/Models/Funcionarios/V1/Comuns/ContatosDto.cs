// <copyright file="ContatosDto.cs" company="Sync Softwares">
// Copyright (c) Sync Softwares. Todos os direitos reservados.
// </copyright>

using Newtonsoft.Json;
using System.Runtime.Serialization;

namespace Glass.API.Backend.Models.Funcionarios.V1.Comuns
{
    /// <summary>
    /// Classe que encapsula os dados de contato.
    /// </summary>
    [DataContract(Name = "Contatos")]
    public class ContatosDto : BaseContatosDto<ContatosDto>
    {
        /// <summary>
        /// Obtém ou define o telefone de contato do funcionário.
        /// </summary>
        [DataMember]
        [JsonProperty("telefoneContato")]
        public string TelefoneContato
        {
            get { return this.ObterValor(c => c.TelefoneContato); }
            set { this.AdicionarValor(c => c.TelefoneContato, value); }
        }

        /// <summary>
        /// Obtém ou define o email do funcionário.
        /// </summary>
        [DataMember]
        [JsonProperty("email")]
        public string Email
        {
            get { return this.ObterValor(c => c.Email); }
            set { this.AdicionarValor(c => c.Email, value); }
        }

        /// <summary>
        /// Obtém ou define o ramal do funcionário.
        /// </summary>
        [DataMember]
        [JsonProperty("ramal")]
        public string Ramal
        {
            get { return this.ObterValor(c => c.Ramal); }
            set { this.AdicionarValor(c => c.Ramal, value); }
        }
    }
}
