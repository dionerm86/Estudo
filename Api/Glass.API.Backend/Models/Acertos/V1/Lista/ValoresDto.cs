// <copyright file="ValoresDto.cs" company="Sync Softwares">
// Copyright (c) Sync Softwares. Todos os direitos reservados.
// </copyright>

using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Web;

namespace Glass.API.Backend.Models.Acertos.V1.Lista
{
    /// <summary>
    /// 
    /// </summary>
    public class ValoresDto
    {
        /// <summary>
        /// Obtém ou define os valores.
        /// </summary>
        [DataMember]
        [JsonProperty("pagar")]
        public decimal Pagar { get; set; }

        /// <summary>
        /// Obtém ou define os valores.
        /// </summary>
        [DataMember]
        [JsonProperty("receber")]
        public decimal Receber { get; set; }

        /// <summary>
        /// Obtém ou define os valores.
        /// </summary>
        [DataMember]
        [JsonProperty("saldo")]
        public decimal Saldo { get; set; }
    }
}