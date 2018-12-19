// <copyright file="DocumentosEDadosPessoaisDto.cs" company="Sync Softwares">
// Copyright (c) Sync Softwares. Todos os direitos reservados.
// </copyright>

using Glass.API.Backend.Models.Genericas.V1.CadastroAtualizacao;
using Newtonsoft.Json;
using System;
using System.Runtime.Serialization;

namespace Glass.API.Backend.Models.Funcionarios.V1.Comuns
{
    /// <summary>
    /// Classe que encapsula os dados de documentação d pessoais do funcionário.
    /// </summary>
    /// <typeparam name="T">O tipo da classe que será instanciada.</typeparam>
    public abstract class DocumentosEDadosPessoaisDto<T> : BaseCadastroAtualizacaoDto<T>
        where T : DocumentosEDadosPessoaisDto<T>
    {
        /// <summary>
        /// Obtém ou define o rg do funcionário.
        /// </summary>
        [DataMember]
        [JsonProperty("rg")]
        public string Rg
        {
            get { return this.ObterValor(c => c.Rg); }
            set { this.AdicionarValor(c => c.Rg, value); }
        }

        /// <summary>
        /// Obtém ou define o cpf do funcionário.
        /// </summary>
        [DataMember]
        [JsonProperty("cpf")]
        public string Cpf
        {
            get { return this.ObterValor(c => c.Cpf); }
            set { this.AdicionarValor(c => c.Cpf, value); }
        }

        /// <summary>
        /// Obtém ou define a funcao do funcionário.
        /// </summary>
        [DataMember]
        [JsonProperty("funcao")]
        public string Funcao
        {
            get { return this.ObterValor(c => c.Funcao); }
            set { this.AdicionarValor(c => c.Funcao, value); }
        }

        /// <summary>
        /// Obtém ou define o estado civil do funcionário.
        /// </summary>
        [DataMember]
        [JsonProperty("estadoCivil")]
        public EstadoCivil? EstadoCivil
        {
            get { return this.ObterValor(c => c.EstadoCivil); }
            set { this.AdicionarValor(c => c.EstadoCivil, value); }
        }

        /// <summary>
        /// Obtém ou define a data de nascimento do funcionário..
        /// </summary>
        [DataMember]
        [JsonProperty("dataNascimento")]
        public DateTime? DataNascimento
        {
            get { return this.ObterValor(c => c.DataNascimento); }
            set { this.AdicionarValor(c => c.DataNascimento, value); }
        }

        /// <summary>
        /// Obtém ou define a data de entrada do funcionário.
        /// </summary>
        [DataMember]
        [JsonProperty("dataEntrada")]
        public DateTime? DataEntrada
        {
            get { return this.ObterValor(c => c.DataEntrada); }
            set { this.AdicionarValor(c => c.DataEntrada, value); }
        }

        /// <summary>
        /// Obtém ou define a data de saida do funcionário.
        /// </summary>
        [DataMember]
        [JsonProperty("dataSaida")]
        public DateTime? DataSaida
        {
            get { return this.ObterValor(c => c.DataSaida); }
            set { this.AdicionarValor(c => c.DataSaida, value); }
        }

        /// <summary>
        /// Obtém ou define o salário do funcionário.
        /// </summary>
        [DataMember]
        [JsonProperty("salario")]
        public decimal Salario
        {
            get { return this.ObterValor(c => c.Salario); }
            set { this.AdicionarValor(c => c.Salario, value); }
        }

        /// <summary>
        /// Obtém ou define o valor da gratificação do funcionário.
        /// </summary>
        [DataMember]
        [JsonProperty("gratificacao")]
        public decimal Gratificacao
        {
            get { return this.ObterValor(c => c.Gratificacao); }
            set { this.AdicionarValor(c => c.Gratificacao, value); }
        }

        /// <summary>
        /// Obtém ou define o numero da Carteira de trabalho do funcionário.
        /// </summary>
        [DataMember]
        [JsonProperty("numeroCTPS")]
        public string NumeroCTPS
        {
            get { return this.ObterValor(c => c.NumeroCTPS); }
            set { this.AdicionarValor(c => c.NumeroCTPS, value); }
        }

        /// <summary>
        /// Obtém ou define o valor do auxílio alimentação do funcionário.
        /// </summary>
        [DataMember]
        [JsonProperty("auxilioAlimentacao")]
        public decimal AuxilioAlimentacao
        {
            get { return this.ObterValor(c => c.AuxilioAlimentacao); }
            set { this.AdicionarValor(c => c.AuxilioAlimentacao, value); }
        }

        /// <summary>
        /// Obtém ou define o número do pis do funcionário.
        /// </summary>
        [DataMember]
        [JsonProperty("numeroPis")]
        public string NumeroPis
        {
            get { return this.ObterValor(c => c.NumeroPis); }
            set { this.AdicionarValor(c => c.NumeroPis, value); }
        }

        /// <summary>
        /// Obtém ou define um valor que indica se o funcionário esta registrado.
        /// </summary>
        [DataMember]
        [JsonProperty("registrado")]
        public bool Registrado
        {
            get { return this.ObterValor(c => c.Registrado); }
            set { this.AdicionarValor(c => c.Registrado, value); }
        }
    }
}
