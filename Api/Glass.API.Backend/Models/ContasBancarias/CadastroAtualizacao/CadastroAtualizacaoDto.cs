// <copyright file="CadastroAtualizacaoDto.cs" company="Sync Softwares">
// Copyright (c) Sync Softwares. Todos os direitos reservados.
// </copyright>

using Glass.API.Backend.Models.Genericas.V1.CadastroAtualizacao;
using Newtonsoft.Json;
using System.Runtime.Serialization;

namespace Glass.API.Backend.Models.ContasBancarias.V1.CadastroAtualizacao
{
    /// <summary>
    /// Classe que encapsula os dados de cadastro ou atualização de uma conta bancária.
    /// </summary>
    [DataContract(Name = "CadastroAtualizacao")]
    public class CadastroAtualizacaoDto : BaseCadastroAtualizacaoDto<CadastroAtualizacaoDto>
    {
        /// <summary>
        /// Obtém ou define o nome do banco.
        /// </summary>
        [DataMember]
        [JsonProperty("nome")]
        public string Nome
        {
            get { return this.ObterValor(c => c.Nome); }
            set { this.AdicionarValor(c => c.Nome, value); }
        }

        /// <summary>
        /// Obtém ou define a situação da conta bancária.
        /// </summary>
        [DataMember]
        [JsonProperty("situacao")]
        public Situacao Situacao
        {
            get { return this.ObterValor(c => c.Situacao); }
            set { this.AdicionarValor(c => c.Situacao, value); }
        }

        /// <summary>
        /// Obtém ou define a loja da conta bancária.
        /// </summary>
        [DataMember]
        [JsonProperty("idLoja")]
        public int? IdLoja
        {
            get { return this.ObterValor(c => c.IdLoja); }
            set { this.AdicionarValor(c => c.IdLoja, value); }
        }

        /// <summary>
        /// Obtém ou define os dados do banco da conta bancária.
        /// </summary>
        [DataMember]
        [JsonProperty("dadosBanco")]
        public DadosBancoDto DadosBanco
        {
            get { return this.ObterValor(c => c.DadosBanco); }
            set { this.AdicionarValor(c => c.DadosBanco, value); }
        }

        /// <summary>
        /// Obtém ou define os dados do cnab da conta bancária.
        /// </summary>
        [DataMember]
        [JsonProperty("cnab")]
        public CnabDto Cnab
        {
            get { return this.ObterValor(c => c.Cnab); }
            set { this.AdicionarValor(c => c.Cnab, value); }
        }
    }
}
