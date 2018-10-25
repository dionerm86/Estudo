// <copyright file="TempoLoginDto.cs" company="Sync Softwares">
// Copyright (c) Sync Softwares. Todos os direitos reservados.
// </copyright>

using Newtonsoft.Json;
using System.Runtime.Serialization;

namespace Glass.API.Backend.Models.Producao.V1.Setores.Lista
{
    /// <summary>
    /// Classe que encapsula dados de login.
    /// </summary>
    [DataContract(Name = "TempoLogin")]
    public class TempoLoginDto
    {
        /// <summary>
        /// Obtém ou define o tempo máximo de login deste setor.
        /// </summary>
        [DataMember]
        [JsonProperty("maximo")]
        public int Maximo { get; set; }

        /// <summary>
        /// Obtém ou define um tempo para alerta de inatividade.
        /// </summary>
        [DataMember]
        [JsonProperty("alertaInatividade")]
        public int AlertaInatividade { get; set; }
    }
}
