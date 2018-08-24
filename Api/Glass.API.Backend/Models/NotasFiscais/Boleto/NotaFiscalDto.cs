// <copyright file="NotaFiscalDto.cs" company="Sync Softwares">
// Copyright (c) Sync Softwares. Todos os direitos reservados.
// </copyright>

using Newtonsoft.Json;
using System.Runtime.Serialization;

namespace Glass.API.Backend.Models.NotasFiscais.Boleto
{
    /// <summary>
    /// Classe que encapsula os dados de retorno ao consultar uma nota fiscal pelo boleto.
    /// </summary>
    [DataContract]
    public class NotaFiscalDto
    {
        /// <summary>
        /// Obtém ou define o identificador da nota fiscal.
        /// </summary>
        [DataMember]
        [JsonProperty("idNotaFiscal")]
        public int? IdNotaFiscal { get; set; }
    }
}
