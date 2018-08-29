// <copyright file="CertificadoDto.cs" company="Sync Softwares">
// Copyright (c) Sync Softwares. Todos os direitos reservados.
// </copyright>

using Newtonsoft.Json;
using System;
using System.Runtime.Serialization;

namespace Glass.API.Backend.Models.Lojas.Certificado
{
    /// <summary>
    /// Classe que encapsula os dados de certificado da loja.
    /// </summary>
    [DataContract]
    public class CertificadoDto
    {
        /// <summary>
        /// Obtém ou define a data de vencimento do certificado da loja.
        /// </summary>
        [DataMember]
        [JsonProperty("dataVencimento")]
        public DateTime? DataVencimento { get; set; }

        /// <summary>
        /// Obtém ou define um valor que indica se o certificado da loja está vencido.
        /// </summary>
        [DataMember]
        [JsonProperty("vencido")]
        public bool Vencido { get; set; }

        /// <summary>
        /// Obtém ou define quantos dias faltam para o certificado vencer.
        /// </summary>
        [DataMember]
        [JsonProperty("diasParaVencimento")]
        public int DiasParaVencimento { get; set; }
    }
}
