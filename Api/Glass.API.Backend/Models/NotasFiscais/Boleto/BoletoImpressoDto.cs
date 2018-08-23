// <copyright file="BoletoImpressoDto.cs" company="Sync Softwares">
// Copyright (c) Sync Softwares. Todos os direitos reservados.
// </copyright>

using Newtonsoft.Json;
using System.Runtime.Serialization;

namespace Glass.API.Backend.Models.NotasFiscais.Boleto
{
    /// <summary>
    /// Classe que encapsula os dados de retorno ao verificar se um boleto está impresso.
    /// </summary>
    [DataContract]
    public class BoletoImpressoDto
    {
        /// <summary>
        /// Obtém ou define a mensagem que indica se o boleto está impresso.
        /// </summary>
        [DataMember]
        [JsonProperty("mensagem")]
        public string Mensagem { get; set; }
    }
}
