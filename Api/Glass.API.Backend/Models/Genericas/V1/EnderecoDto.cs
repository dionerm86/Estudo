// <copyright file="EnderecoDto.cs" company="Sync Softwares">
// Copyright (c) Sync Softwares. Todos os direitos reservados.
// </copyright>

using Glass.API.Backend.Models.Genericas.V1.CadastroAtualizacao;
using Newtonsoft.Json;
using System.Runtime.Serialization;

namespace Glass.API.Backend.Models.Genericas.V1
{
    /// <summary>
    /// Classe que encapsula dados básicos de um endereço.
    /// </summary>
    [DataContract(Name = "Endereco")]
    public class EnderecoDto : BaseCadastroAtualizacaoDto<EnderecoDto>
    {
        /// <summary>
        /// Obtém ou define o logradouro do funcionário.
        /// </summary>
        [DataMember]
        [JsonProperty("logradouro")]
        public string Logradouro
        {
            get { return this.ObterValor(c => c.Logradouro); }
            set { this.AdicionarValor(c => c.Logradouro, value); }
        }

        /// <summary>
        /// Obtém ou define o número do endereço do funcionário.
        /// </summary>
        [DataMember]
        [JsonProperty("numero")]
        public string Numero
        {
            get { return this.ObterValor(c => c.Numero); }
            set { this.AdicionarValor(c => c.Numero, value); }
        }

        /// <summary>
        /// Obtém ou define o complemento do endereço do funcionário.
        /// </summary>
        [DataMember]
        [JsonProperty("complemento")]
        public string Complemento
        {
            get { return this.ObterValor(c => c.Complemento); }
            set { this.AdicionarValor(c => c.Complemento, value); }
        }

        /// <summary>
        /// Obtém ou define o bairro do funcionário.
        /// </summary>
        [DataMember]
        [JsonProperty("bairro")]
        public string Bairro
        {
            get { return this.ObterValor(c => c.Bairro); }
            set { this.AdicionarValor(c => c.Bairro, value); }
        }

        /// <summary>
        /// Obtém ou define a cidade do funcionário.
        /// </summary>
        [DataMember]
        [JsonProperty("cidade")]
        public CidadeDto Cidade
        {
            get { return this.ObterValor(c => c.Cidade); }
            set { this.AdicionarValor(c => c.Cidade, value); }
        }

        /// <summary>
        /// Obtém ou define o CEP do funcionário.
        /// </summary>
        [DataMember]
        [JsonProperty("cep")]
        public string Cep
        {
            get { return this.ObterValor(c => c.Cep); }
            set { this.AdicionarValor(c => c.Cep, value); }
        }
    }
}
