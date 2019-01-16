// <copyright file="DatasDto.cs" company="Sync Softwares">
// Copyright (c) Sync Softwares. Todos os direitos reservados.
// </copyright>

using Newtonsoft.Json;
using System;
using System.Runtime.Serialization;

namespace Glass.API.Backend.Models.Producao.V1.Retalhos.Lista
{
    /// <summary>
    /// Classe que encapsula os dados das datas da lista de retalhos de produção.
    /// </summary>
    [DataContract(Name = "Data")]
    public class DatasDto
    {
        /// <summary>
        /// Obtém ou define a data do cadastro de retalhos de produção.
        /// </summary>
        [DataMember]
        [JsonProperty("cadastro")]
        public DateTime? Cadastro { get; set; }

        /// <summary>
        /// Obtém ou define a data de uso de retalhos de produção.
        /// </summary>
        [DataMember]
        [JsonProperty("uso")]
        public DateTime? Uso { get; set; }
    }
}