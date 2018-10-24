// <copyright file="BancoDto.cs" company="Sync Softwares">
// Copyright (c) Sync Softwares. Todos os direitos reservados.
// </copyright>

using Newtonsoft.Json;
using System.Runtime.Serialization;

namespace Glass.API.Backend.Models.ContasBancarias.V1.Lista
{
    /// <summary>
    /// Classe que encapsula dados do banco.
    /// </summary>
    [DataContract(Name = "Banco")]
    public class BancoDto
    {
        /// <summary>
        /// Obtém ou define o código do banco.
        /// </summary>
        [DataMember]
        [JsonProperty("codigoBanco")]
        public int? CodigoBanco { get; set; }

        /// <summary>
        /// Obtém ou define o titular do banco.
        /// </summary>
        [DataMember]
        [JsonProperty("titular")]
        public string Titular { get; set; }

        /// <summary>
        /// Obtém ou define a agência do banco.
        /// </summary>
        [DataMember]
        [JsonProperty("agencia")]
        public string Agencia { get; set; }

        /// <summary>
        /// Obtém ou define a conta do banco.
        /// </summary>
        [DataMember]
        [JsonProperty("conta")]
        public string Conta { get; set; }

        /// <summary>
        /// Obtém ou define o código de convêncio com o banco.
        /// </summary>
        [DataMember]
        [JsonProperty("codigoConvenio")]
        public string CodigoConvenio { get; set; }
    }
}
