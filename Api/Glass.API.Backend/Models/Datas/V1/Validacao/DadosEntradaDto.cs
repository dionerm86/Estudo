// <copyright file="DadosEntradaDto.cs" company="Sync Softwares">
// Copyright (c) Sync Softwares. Todos os direitos reservados.
// </copyright>

using Newtonsoft.Json;
using System;
using System.Runtime.Serialization;

namespace Glass.API.Backend.Models.Datas.V1.Validacao
{
    /// <summary>
    /// Classe com os dados de entrada do método de validação de datas.
    /// </summary>
    [DataContract(Name = "DadosEntrada")]
    public class DadosEntradaDto
    {
        /// <summary>
        /// Obtém ou define a data que será validada.
        /// </summary>
        [DataMember]
        [JsonProperty("data")]
        public DateTime Data { get; set; }

        /// <summary>
        /// Obtém ou define um valor que indica se os fins de semana são valores válidos para a data.
        /// </summary>
        [DataMember]
        [JsonProperty("permitirFimDeSemana")]
        public bool PermitirFimDeSemana { get; set; }

        /// <summary>
        /// Obtém ou define um valor que indica se os feriados são valores válidos para a data.
        /// </summary>
        [DataMember]
        [JsonProperty("permitirFeriado")]
        public bool PermitirFeriado { get; set; }
    }
}
