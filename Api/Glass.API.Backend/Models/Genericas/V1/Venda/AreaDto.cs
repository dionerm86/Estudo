﻿// <copyright file="AreaDto.cs" company="Sync Softwares">
// Copyright (c) Sync Softwares. Todos os direitos reservados.
// </copyright>

using Newtonsoft.Json;
using System.Runtime.Serialization;

namespace Glass.API.Backend.Models.Genericas.V1.Venda
{
    /// <summary>
    /// Classe que encapsula os dados da área de um produto.
    /// </summary>
    [DataContract(Name = "Area")]
    public class AreaDto
    {
        /// <summary>
        /// Obtém ou define a área para cálculo.
        /// </summary>
        [DataMember]
        [JsonProperty("paraCalculo")]
        public decimal ParaCalculo { get; set; }

        /// <summary>
        /// Obtém ou define a área real.
        /// </summary>
        [DataMember]
        [JsonProperty("real")]
        public decimal Real { get; set; }

        /// <summary>
        /// Obtém ou define a área para cálculo, sem chapa.
        /// </summary>
        [DataMember]
        [JsonProperty("paraCalculoSemChapa")]
        public string ParaCalculoSemChapa { get; set; }
    }
}
