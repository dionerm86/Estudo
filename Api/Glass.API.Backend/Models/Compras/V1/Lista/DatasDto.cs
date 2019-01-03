// <copyright file="DatasDto.cs" company="Sync Softwares">
// Copyright (c) Sync Softwares. Todos os direitos reservados.
// </copyright>

using Newtonsoft.Json;
using System;
using System.Runtime.Serialization;

namespace Glass.API.Backend.Models.Compras.V1.Lista
{
    /// <summary>
    /// Classe que encapsula os dados das datas para a lista de compras.
    /// </summary>
    [DataContract(Name = "Datas")]
    public class DatasDto
    {
        /// <summary>
        /// Obtém ou define a data de fabricação.
        /// </summary>
        [DataMember]
        [JsonProperty("fabrica")]
        public DateTime? Fabrica { get; set; }

        /// <summary>
        /// Obtém ou define a data de cadastro.
        /// </summary>
        [DataMember]
        [JsonProperty("cadastro")]
        public DateTime? Cadastro { get; set; }
    }
}