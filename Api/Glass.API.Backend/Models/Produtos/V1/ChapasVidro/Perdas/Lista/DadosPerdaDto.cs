// <copyright file="DadosPerdaDto.cs" company="Sync Softwares">
// Copyright (c) Sync Softwares. Todos os direitos reservados.
// </copyright>

using Newtonsoft.Json;
using System;
using System.Runtime.Serialization;

namespace Glass.API.Backend.Models.Produtos.V1.ChapasVidro.Perdas.Lista
{
    /// <summary>
    /// Classe que encapsula os dados de uma perda de chapa de vidro.
    /// </summary>
    [DataContract(Name = "DadosPerda")]
    public class DadosPerdaDto
    {
        /// <summary>
        /// Obtém ou define o tipo da perda.
        /// </summary>
        [DataMember]
        [JsonProperty("tipo")]
        public string Tipo { get; set; }

        /// <summary>
        /// Obtém ou define o subtipo da perda.
        /// </summary>
        [DataMember]
        [JsonProperty("subtipo")]
        public string Subtipo { get; set; }

        /// <summary>
        /// Obtém ou define a data e hora da perda.
        /// </summary>
        [DataMember]
        [JsonProperty("data")]
        public DateTime? Data { get; set; }

        /// <summary>
        /// Obtém ou define o funcionário responsável pela perda.
        /// </summary>
        [DataMember]
        [JsonProperty("funcionario")]
        public string Funcionario { get; set; }
    }
}