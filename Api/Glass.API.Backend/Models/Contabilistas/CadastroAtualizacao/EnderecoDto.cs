// <copyright file="EnderecoDto.cs" company="Sync Softwares">
// Copyright (c) Sync Softwares. Todos os direitos reservados.
// </copyright>

using Glass.API.Backend.Models.Genericas.V1.CadastroAtualizacao;
using Newtonsoft.Json;
using System.Runtime.Serialization;

namespace Glass.API.Backend.Models.Contabilistas.V1.CadastroAtualizacao
{
    /// <summary>
    /// Classe que encapsula os dados de endereço de um contabilista.
    /// </summary>
    [DataContract(Name = "Endereco")]
    public class EnderecoDto : BaseCadastroAtualizacaoDto<EnderecoDto>
    {
        /// <summary>
        /// Obtém ou define o logradouro do contabilista.
        /// </summary>
        [DataMember]
        [JsonProperty("logradouro")]
        public string Logradouro
        {
            get { return this.ObterValor(c => c.Logradouro); }
            set { this.AdicionarValor(c => c.Logradouro, value); }
        }

        /// <summary>
        /// Obtém ou define o número do endereço do contabilista.
        /// </summary>
        [DataMember]
        [JsonProperty("numero")]
        public string Numero
        {
            get { return this.ObterValor(c => c.Numero); }
            set { this.AdicionarValor(c => c.Numero, value); }
        }

        /// <summary>
        /// Obtém ou define o complemento do endereço do contabilista.
        /// </summary>
        [DataMember]
        [JsonProperty("complemento")]
        public string Complemento
        {
            get { return this.ObterValor(c => c.Complemento); }
            set { this.AdicionarValor(c => c.Complemento, value); }
        }

        /// <summary>
        /// Obtém ou define o bairro do contabilista.
        /// </summary>
        [DataMember]
        [JsonProperty("bairro")]
        public string Bairro
        {
            get { return this.ObterValor(c => c.Bairro); }
            set { this.AdicionarValor(c => c.Bairro, value); }
        }

        /// <summary>
        /// Obtém ou define a cidade do contabilista.
        /// </summary>
        [DataMember]
        [JsonProperty("cidade")]
        public int Cidade
        {
            get { return this.ObterValor(c => c.Cidade); }
            set { this.AdicionarValor(c => c.Cidade, value); }
        }

        /// <summary>
        /// Obtém ou define o CEP do contabilista.
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
