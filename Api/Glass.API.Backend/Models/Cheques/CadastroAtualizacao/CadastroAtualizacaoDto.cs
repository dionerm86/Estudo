// <copyright file="CadastroAtualizacaoDto.cs" company="Sync Softwares">
// Copyright (c) Sync Softwares. Todos os direitos reservados.
// </copyright>

using Newtonsoft.Json;
using System;
using System.Runtime.Serialization;

namespace Glass.API.Backend.Models.Cheques.CadastroAtualizacao
{
    /// <summary>
    /// Classe que encapsula os dados de atualização do cheque.
    /// </summary>
    [DataContract(Name = "CadastroAtualizacao")]
    public class CadastroAtualizacaoDto
    {
        /// <summary>
        /// Obtém ou define o número do cheque.
        /// </summary>
        [DataMember]
        [JsonProperty("numeroCheque")]
        public int NumeroCheque { get; set; }

        /// <summary>
        /// Obtém ou define o dígito do número do cheque.
        /// </summary>
        [DataMember]
        [JsonProperty("digitoNumeroCheque")]
        public string DigitoNumeroCheque { get; set; }

        /// <summary>
        /// Obtém ou define o banco do cheque.
        /// </summary>
        [DataMember]
        [JsonProperty("banco")]
        public string Banco { get; set; }

        /// <summary>
        /// Obtém ou define a agência do cheque.
        /// </summary>
        [DataMember]
        [JsonProperty("agencia")]
        public string Agencia { get; set; }

        /// <summary>
        /// Obtém ou define a conta do cheque.
        /// </summary>
        [DataMember]
        [JsonProperty("conta")]
        public string Conta { get; set; }

        /// <summary>
        /// Obtém ou define o titular do cheque.
        /// </summary>
        [DataMember]
        [JsonProperty("titular")]
        public string Titular { get; set; }

        /// <summary>
        /// Obtém ou define a data de vencimento do cheque.
        /// </summary>
        [DataMember]
        [JsonProperty("dataVencimento")]
        public DateTime DataVencimento { get; set; }

        /// <summary>
        /// Obtém ou define a observação do cheque.
        /// </summary>
        [DataMember]
        [JsonProperty("observacao")]
        public string Observacao { get; set; }
    }
}