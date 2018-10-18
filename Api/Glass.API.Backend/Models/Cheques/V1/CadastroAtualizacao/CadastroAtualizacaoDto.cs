// <copyright file="CadastroAtualizacaoDto.cs" company="Sync Softwares">
// Copyright (c) Sync Softwares. Todos os direitos reservados.
// </copyright>

using Glass.API.Backend.Models.Genericas.V1.CadastroAtualizacao;
using Newtonsoft.Json;
using System;
using System.Runtime.Serialization;

namespace Glass.API.Backend.Models.Cheques.V1.CadastroAtualizacao
{
    /// <summary>
    /// Classe que encapsula os dados de atualização do cheque.
    /// </summary>
    [DataContract(Name = "CadastroAtualizacao")]
    public class CadastroAtualizacaoDto : BaseCadastroAtualizacaoDto<CadastroAtualizacaoDto>
    {
        /// <summary>
        /// Obtém ou define o número do cheque.
        /// </summary>
        [DataMember]
        [JsonProperty("numeroCheque")]
        public int NumeroCheque
        {
            get { return this.ObterValor(c => c.NumeroCheque); }
            set { this.AdicionarValor(c => c.NumeroCheque, value); }
        }

        /// <summary>
        /// Obtém ou define o dígito do número do cheque.
        /// </summary>
        [DataMember]
        [JsonProperty("digitoNumeroCheque")]
        public string DigitoNumeroCheque
        {
            get { return this.ObterValor(c => c.DigitoNumeroCheque); }
            set { this.AdicionarValor(c => c.DigitoNumeroCheque, value); }
        }

        /// <summary>
        /// Obtém ou define o banco do cheque.
        /// </summary>
        [DataMember]
        [JsonProperty("banco")]
        public string Banco
        {
            get { return this.ObterValor(c => c.Banco); }
            set { this.AdicionarValor(c => c.Banco, value); }
        }

        /// <summary>
        /// Obtém ou define a agência do cheque.
        /// </summary>
        [DataMember]
        [JsonProperty("agencia")]
        public string Agencia
        {
            get { return this.ObterValor(c => c.Agencia); }
            set { this.AdicionarValor(c => c.Agencia, value); }
        }

        /// <summary>
        /// Obtém ou define a conta do cheque.
        /// </summary>
        [DataMember]
        [JsonProperty("conta")]
        public string Conta
        {
            get { return this.ObterValor(c => c.Conta); }
            set { this.AdicionarValor(c => c.Conta, value); }
        }

        /// <summary>
        /// Obtém ou define o titular do cheque.
        /// </summary>
        [DataMember]
        [JsonProperty("titular")]
        public string Titular
        {
            get { return this.ObterValor(c => c.Titular); }
            set { this.AdicionarValor(c => c.Titular, value); }
        }

        /// <summary>
        /// Obtém ou define a data de vencimento do cheque.
        /// </summary>
        [DataMember]
        [JsonProperty("dataVencimento")]
        public DateTime? DataVencimento
        {
            get { return this.ObterValor(c => c.DataVencimento); }
            set { this.AdicionarValor(c => c.DataVencimento, value); }
        }

        /// <summary>
        /// Obtém ou define a observação do cheque.
        /// </summary>
        [DataMember]
        [JsonProperty("observacao")]
        public string Observacao
        {
            get { return this.ObterValor(c => c.Observacao); }
            set { this.AdicionarValor(c => c.Observacao, value); }
        }
    }
}
