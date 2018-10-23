// <copyright file="CadastroAtualizacaoDto.cs" company="Sync Softwares">
// Copyright (c) Sync Softwares. Todos os direitos reservados.
// </copyright>

using Glass.API.Backend.Models.Genericas.V1.CadastroAtualizacao;
using Newtonsoft.Json;
using System.Runtime.Serialization;

namespace Glass.API.Backend.Models.Cfops.V1.CadastroAtualizacao
{
    /// <summary>
    /// Classe que encapsula os dados de cadastro ou atualização de um CFOP.
    /// </summary>
    [DataContract(Name = "CadastroAtualizacao")]
    public class CadastroAtualizacaoDto : BaseCadastroAtualizacaoDto<CadastroAtualizacaoDto>
    {
        /// <summary>
        /// Obtém ou define o código interno do CFOP.
        /// </summary>
        [DataMember]
        [JsonProperty("codInterno")]
        public string CodInterno
        {
            get { return this.ObterValor(c => c.CodInterno); }
            set { this.AdicionarValor(c => c.CodInterno, value); }
        }

        /// <summary>
        /// Obtém ou define o nome do CFOP.
        /// </summary>
        [DataMember]
        [JsonProperty("nome")]
        public string Nome
        {
            get { return this.ObterValor(c => c.Nome); }
            set { this.AdicionarValor(c => c.Nome, value); }
        }

        /// <summary>
        /// Obtém ou define o tipo do CFOP.
        /// </summary>
        [DataMember]
        [JsonProperty("idTipoCfop")]
        public int? IdTipoCfop
        {
            get { return this.ObterValor(c => c.IdTipoCfop); }
            set { this.AdicionarValor(c => c.IdTipoCfop, value); }
        }

        /// <summary>
        /// Obtém ou define o tipo de mercadoria do CFOP.
        /// </summary>
        [DataMember]
        [JsonProperty("tipoMercadoria")]
        public Data.Model.TipoMercadoria? TipoMercadoria
        {
            get { return this.ObterValor(c => c.TipoMercadoria); }
            set { this.AdicionarValor(c => c.TipoMercadoria, value); }
        }

        /// <summary>
        /// Obtém ou define um valor que indica se o CFOP deve controlar estoque de terceiros.
        /// </summary>
        [DataMember]
        [JsonProperty("alterarEstoqueTerceiros")]
        public bool AlterarEstoqueTerceiros
        {
            get { return this.ObterValor(c => c.AlterarEstoqueTerceiros); }
            set { this.AdicionarValor(c => c.AlterarEstoqueTerceiros, value); }
        }

        /// <summary>
        /// Obtém ou define um valor que indica se o CFOP deve controlar o estoque do cliente.
        /// </summary>
        [DataMember]
        [JsonProperty("alterarEstoqueCliente")]
        public bool AlterarEstoqueCliente
        {
            get { return this.ObterValor(c => c.AlterarEstoqueCliente); }
            set { this.AdicionarValor(c => c.AlterarEstoqueCliente, value); }
        }

        /// <summary>
        /// Obtém ou define a observação do CFOP.
        /// </summary>
        [DataMember]
        [JsonProperty("obs")]
        public string Obs
        {
            get { return this.ObterValor(c => c.Obs); }
            set { this.AdicionarValor(c => c.Obs, value); }
        }
    }
}
