// <copyright file="TotaisCreditoDtoDto.cs" company="Sync Softwares">
// Copyright (c) Sync Softwares. Todos os direitos reservados.
// </copyright>

using Newtonsoft.Json;
using System.Runtime.Serialization;

namespace Glass.API.Backend.Models.Caixas.Geral.Lista
{
    /// <summary>
    /// Classe que encapsula dados dos valores de crédito.
    /// </summary>
    [DataContract(Name = "TotaisCredito")]
    public class TotaisCreditoDto
    {
        /// <summary>
        /// Obtém ou define o total de crédito gerado.
        /// </summary>
        [DataMember]
        [JsonProperty("gerado")]
        public decimal Gerado { get; set; }

        /// <summary>
        /// Obtém ou define o total de crédito recebido.
        /// </summary>
        [DataMember]
        [JsonProperty("recebido")]
        public decimal Recebido { get; set; }
    }
}
