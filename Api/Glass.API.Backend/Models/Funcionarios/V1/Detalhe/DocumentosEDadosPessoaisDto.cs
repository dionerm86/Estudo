// <copyright file="DocumentosEDadosPessoaisDto.cs" company="Sync Softwares">
// Copyright (c) Sync Softwares. Todos os direitos reservados.
// </copyright>

using Newtonsoft.Json;
using System;
using System.Runtime.Serialization;

namespace Glass.API.Backend.Models.Funcionarios.Detalhe
{
    /// <summary>
    /// Classe que encapsula os dados de documentação d pessoais do funcionário.
    /// </summary>
    [DataContract(Name = "DocumentosEDadosPessoais")]
    public class DocumentosEDadosPessoaisDto
    {
        /// <summary>
        /// Obtém ou define o rg do funcionário.
        /// </summary>
        [DataMember]
        [JsonProperty("rg")]
        public string Rg { get; set; }

        /// <summary>
        /// Obtém ou define o cpf do funcionário.
        /// </summary>
        [DataMember]
        [JsonProperty("cpf")]
        public string Cpf { get; set; }

        /// <summary>
        /// Obtém ou define a funcao do funcionário.
        /// </summary>
        [DataMember]
        [JsonProperty("funcao")]
        public string Funcao { get; set; }

        /// <summary>
        /// Obtém ou define o estado civil do funcionário.
        /// </summary>
        [DataMember]
        [JsonProperty("estadoCivil")]
        public string EstadoCivil { get; set; }

        /// <summary>
        /// Obtém ou define a data de nascimento do funcionário..
        /// </summary>
        [DataMember]
        [JsonProperty("dataNascimento")]
        public DateTime? DataNascimento { get; set; }

        /// <summary>
        /// Obtém ou define a data de entrada do funcionário.
        /// </summary>
        [DataMember]
        [JsonProperty("dataEntrada")]
        public DateTime? DataEntrada { get; set; }

        /// <summary>
        /// Obtém ou define a data de saida do funcionário.
        /// </summary>
        [DataMember]
        [JsonProperty("dataSaida")]
        public DateTime? DataSaida { get; set; }

        /// <summary>
        /// Obtém ou define o salário do funcionário.
        /// </summary>
        [DataMember]
        [JsonProperty("salario")]
        public decimal Salario { get; set; }

        /// <summary>
        /// Obtém ou define o valor da gratificação do funcionário.
        /// </summary>
        [DataMember]
        [JsonProperty("gratificacao")]
        public decimal Gratificacao { get; set; }

        /// <summary>
        /// Obtém ou define o numero da Carteira de trabalho do funcionário.
        /// </summary>
        [DataMember]
        [JsonProperty("numeroCTPS")]
        public string NumeroCTPS { get; set; }

        /// <summary>
        /// Obtém ou define o valor do auxílio alimentação do funcionário.
        /// </summary>
        [DataMember]
        [JsonProperty("auxilioAlimentacao")]
        public decimal AuxilioAlimentacao { get; set; }

        /// <summary>
        /// Obtém ou define o número do pis do funcionário.
        /// </summary>
        [DataMember]
        [JsonProperty("numeroPis")]
        public string NumeroPis { get; set; }

        /// <summary>
        /// Obtém ou define um valor que indica se o funcionário esta registrado.
        /// </summary>
        [DataMember]
        [JsonProperty("registrado")]
        public bool Registrado { get; set; }

        /// <summary>
        /// Obtém ou define a foto do funcionário..
        /// </summary>
        [DataMember]
        [JsonProperty("foto")]
        public string Foto { get; set; }
    }
}
