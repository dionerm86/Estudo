// <copyright file="BancoDto.cs" company="Sync Softwares">
// Copyright (c) Sync Softwares. Todos os direitos reservados.
// </copyright>

using Glass.API.Backend.Models.Genericas.V1.CadastroAtualizacao;
using Newtonsoft.Json;
using System.Runtime.Serialization;

namespace Glass.API.Backend.Models.ContasBancarias.V1.CadastroAtualizacao
{
    /// <summary>
    /// Classe que encapsula dados do banco.
    /// </summary>
    [DataContract(Name = "Banco")]
    public class BancoDto : BaseCadastroAtualizacaoDto<BancoDto>
    {
        /// <summary>
        /// Obtém ou define o código do banco.
        /// </summary>
        [DataMember]
        [JsonProperty("codigoBanco")]
        public int? CodigoBanco
        {
            get { return this.ObterValor(c => c.CodigoBanco); }
            set { this.AdicionarValor(c => c.CodigoBanco, value); }
        }

        /// <summary>
        /// Obtém ou define o titular do banco.
        /// </summary>
        [DataMember]
        [JsonProperty("titular")]
        public string Titular
        {
            get { return this.ObterValor(c => c.Titular); }
            set { this.AdicionarValor(c => c.Titular, value); }
        }

        /// <summary>
        /// Obtém ou define a agência do banco.
        /// </summary>
        [DataMember]
        [JsonProperty("agencia")]
        public string Agencia
        {
            get { return this.ObterValor(c => c.Agencia); }
            set { this.AdicionarValor(c => c.Agencia, value); }
        }

        /// <summary>
        /// Obtém ou define a conta do banco.
        /// </summary>
        [DataMember]
        [JsonProperty("conta")]
        public string Conta
        {
            get { return this.ObterValor(c => c.Conta); }
            set { this.AdicionarValor(c => c.Conta, value); }
        }

        /// <summary>
        /// Obtém ou define o código de convêncio com o banco.
        /// </summary>
        [DataMember]
        [JsonProperty("codigoConvenio")]
        public string CodigoConvenio
        {
            get { return this.ObterValor(c => c.CodigoConvenio); }
            set { this.AdicionarValor(c => c.CodigoConvenio, value); }
        }
    }
}
