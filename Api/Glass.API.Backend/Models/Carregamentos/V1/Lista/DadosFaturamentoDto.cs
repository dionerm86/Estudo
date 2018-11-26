// <copyright file="DadosFaturamentoDto.cs" company="Sync Softwares">
// Copyright (c) Sync Softwares. Todos os direitos reservados.
// </copyright>

using Newtonsoft.Json;
using System.Runtime.Serialization;

namespace Glass.API.Backend.Models.Carregamentos.V1.Lista
{
    /// <summary>
    /// Classe que encapsula dados de faturamento do carregamento.
    /// </summary>
    [DataContract(Name = "DadosFaturamento")]
    public class DadosFaturamentoDto
    {
        /// <summary>
        /// Obtém ou define o identificador da liberação.
        /// </summary>
        [DataMember]
        [JsonProperty("idLiberacao")]
        public int IdLiberacao { get; set; }

        /// <summary>
        /// Obtém ou define o identificador da nota fiscal.
        /// </summary>
        [DataMember]
        [JsonProperty("idNotaFiscal")]
        public int IdNotaFiscal { get; set; }
    }
}
