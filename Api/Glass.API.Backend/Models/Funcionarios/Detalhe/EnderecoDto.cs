// <copyright file="EnderecoDto.cs" company="Sync Softwares">
// Copyright (c) Sync Softwares. Todos os direitos reservados.
// </copyright>

using Newtonsoft.Json;
using System.Runtime.Serialization;

namespace Glass.API.Backend.Models.Funcionarios.Detalhe
{
    /// <summary>
    /// Classe que encapsula os dados de endereço.
    /// </summary>
    [DataContract(Name = "Endereco")]
    public class EnderecoDto
    {
        /// <summary>
        /// Obtém ou define o telefone de contato do cliente.
        /// </summary>
        [DataMember]
        [JsonProperty("logradouro")]
        public string Logradouro { get; set; }

        /// <summary>
        /// Obtém ou define o telefone de contato do cliente.
        /// </summary>
        [DataMember]
        [JsonProperty("complemento")]
        public string Complemento { get; set; }

        /// <summary>
        /// Obtém ou define o telefone de contato do cliente.
        /// </summary>
        [DataMember]
        [JsonProperty("bairro")]
        public string Bairro { get; set; }

        /// <summary>
        /// Obtém ou define o telefone de contato do cliente.
        /// </summary>
        [DataMember]
        [JsonProperty("cidade")]
        public CidadeDto Cidade { get; set; }

        /// <summary>
        /// Obtém ou define o telefone de contato do cliente.
        /// </summary>
        [DataMember]
        [JsonProperty("cep")]
        public string Cep { get; set; }
    }
}
