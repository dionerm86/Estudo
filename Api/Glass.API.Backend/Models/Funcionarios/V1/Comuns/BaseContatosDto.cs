// <copyright file="BaseContatosDto.cs" company="Sync Softwares">
// Copyright (c) Sync Softwares. Todos os direitos reservados.
// </copyright>

using Glass.API.Backend.Models.Genericas.V1.CadastroAtualizacao;
using Newtonsoft.Json;
using System.Runtime.Serialization;

namespace Glass.API.Backend.Models.Funcionarios.V1.Comuns
{
    /// <summary>
    /// Classe que encapsula dados de contato do funcionário.
    /// </summary>
    /// <typeparam name="T">O tipo da classe de contatos.</typeparam>
    [DataContract(Name = "Contatos")]
    public abstract class BaseContatosDto<T> : BaseCadastroAtualizacaoDto<T>
        where T : BaseContatosDto<T>
    {
        /// <summary>
        /// Obtém ou define o telefone residencial de contato do funcionário.
        /// </summary>
        [DataMember]
        [JsonProperty("telefoneResidencial")]
        public string TelefoneResidencial
        {
            get { return this.ObterValor(c => c.TelefoneResidencial); }
            set { this.AdicionarValor(c => c.TelefoneResidencial, value); }
        }

        /// <summary>
        /// Obtém ou define o telefone celular de contato do funcionário.
        /// </summary>
        [DataMember]
        [JsonProperty("telefoneCelular")]
        public string TelefoneCelular
        {
            get { return this.ObterValor(c => c.TelefoneCelular); }
            set { this.AdicionarValor(c => c.TelefoneCelular, value); }
        }
    }
}
