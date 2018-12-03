// <copyright file="CadastroAtualizacaoDto.cs" company="Sync Softwares">
// Copyright (c) Sync Softwares. Todos os direitos reservados.
// </copyright>

using Glass.API.Backend.Models.Genericas.V1.CadastroAtualizacao;
using Newtonsoft.Json;
using System.Runtime.Serialization;

namespace Glass.API.Backend.Models.Contabilistas.V1.CadastroAtualizacao
{
    /// <summary>
    /// Classe que encapsula os dados de cadastro ou atualização de um contabilista.
    /// </summary>
    [DataContract(Name = "CadastroAtualizacao")]
    public class CadastroAtualizacaoDto : BaseCadastroAtualizacaoDto<CadastroAtualizacaoDto>
    {
        /// <summary>
        /// Obtém ou define o nome do contabilista.
        /// </summary>
        [DataMember]
        [JsonProperty("nome")]
        public string Nome
        {
            get { return this.ObterValor(c => c.Nome); }
            set { this.AdicionarValor(c => c.Nome, value); }
        }

        /// <summary>
        /// Obtém ou define o tipo do contabilista.
        /// </summary>
        [DataMember]
        [JsonProperty("tipoPessoa")]
        public string TipoPessoa
        {
            get { return this.ObterValor(c => c.TipoPessoa); }
            set { this.AdicionarValor(c => c.TipoPessoa, value); }
        }

        /// <summary>
        /// Obtém ou define o CPF/CNPJ do contabilista.
        /// </summary>
        [DataMember]
        [JsonProperty("cpfCnpj")]
        public string CpfCnpj
        {
            get { return this.ObterValor(c => c.CpfCnpj); }
            set { this.AdicionarValor(c => c.CpfCnpj, value); }
        }

        /// <summary>
        /// Obtém ou define o CRC do contabilista.
        /// </summary>
        [DataMember]
        [JsonProperty("crc")]
        public string Crc
        {
            get { return this.ObterValor(c => c.Crc); }
            set { this.AdicionarValor(c => c.Crc, value); }
        }

        /// <summary>
        /// Obtém ou define a situação do contabilista.
        /// </summary>
        [DataMember]
        [JsonProperty("situacao")]
        public Situacao Situacao
        {
            get { return this.ObterValor(c => c.Situacao); }
            set { this.AdicionarValor(c => c.Situacao, value); }
        }

        /// <summary>
        /// Obtém ou define dados do contato do contabilista.
        /// </summary>
        [DataMember]
        [JsonProperty("dadosContato")]
        public DadosContatoDto DadosContato
        {
            get { return this.ObterValor(c => c.DadosContato); }
            set { this.AdicionarValor(c => c.DadosContato, value); }
        }

        /// <summary>
        /// Obtém ou define dados do endereço do contabilista.
        /// </summary>
        [DataMember]
        [JsonProperty("endereco")]
        public EnderecoDto Endereco
        {
            get { return this.ObterValor(c => c.Endereco); }
            set { this.AdicionarValor(c => c.Endereco, value); }
        }
    }
}
