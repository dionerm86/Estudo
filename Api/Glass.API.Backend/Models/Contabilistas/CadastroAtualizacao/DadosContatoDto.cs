// <copyright file="DadosContatoDto.cs" company="Sync Softwares">
// Copyright (c) Sync Softwares. Todos os direitos reservados.
// </copyright>

using Glass.API.Backend.Models.Genericas.V1.CadastroAtualizacao;
using Newtonsoft.Json;
using System.Runtime.Serialization;

namespace Glass.API.Backend.Models.Contabilistas.V1.CadastroAtualizacao
{
    /// <summary>
    /// Classe que encapsula os dados de contato de um contabilista.
    /// </summary>
    [DataContract(Name = "DadosContato")]
    public class DadosContatoDto : BaseCadastroAtualizacaoDto<DadosContatoDto>
    {
        /// <summary>
        /// Obtém ou define o telefone do contabilista.
        /// </summary>
        [DataMember]
        [JsonProperty("telefone")]
        public string Telefone
        {
            get { return this.ObterValor(c => c.Telefone); }
            set { this.AdicionarValor(c => c.Telefone, value); }
        }

        /// <summary>
        /// Obtém ou define o fax do contabilista.
        /// </summary>
        [DataMember]
        [JsonProperty("fax")]
        public string Fax
        {
            get { return this.ObterValor(c => c.Fax); }
            set { this.AdicionarValor(c => c.Fax, value); }
        }

        /// <summary>
        /// Obtém ou define o email do contabilista.
        /// </summary>
        [DataMember]
        [JsonProperty("email")]
        public string Email
        {
            get { return this.ObterValor(c => c.Email); }
            set { this.AdicionarValor(c => c.Email, value); }
        }
    }
}
