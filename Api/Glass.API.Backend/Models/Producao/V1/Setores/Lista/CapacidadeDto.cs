// <copyright file="CapacidadeDto.cs" company="Sync Softwares">
// Copyright (c) Sync Softwares. Todos os direitos reservados.
// </copyright>

using Newtonsoft.Json;
using System.Runtime.Serialization;

namespace Glass.API.Backend.Models.Producao.V1.Setores.Lista
{
    /// <summary>
    /// Classe que encapsula dados da capacidade do setor.
    /// </summary>
    [DataContract(Name = "Capacidade")]
    public class CapacidadeDto
    {
        /// <summary>
        /// Obtém ou define a altura máxima de peças que podem entrar nesse setor.
        /// </summary>
        [DataMember]
        [JsonProperty("alturaMaxima")]
        public int AlturaMaxima { get; set; }

        /// <summary>
        /// Obtém ou define a largura máxima de peças que podem entrar nesse setor.
        /// </summary>
        [DataMember]
        [JsonProperty("larguraMaxima")]
        public int LarguraMaxima { get; set; }

        /// <summary>
        /// Obtém ou define a capacidade diária deste setor.
        /// </summary>
        [DataMember]
        [JsonProperty("diaria")]
        public int? Diaria { get; set; }

        /// <summary>
        /// Obtém ou define um valor que indica se a capacidade diária deste setor será ignorada.
        /// </summary>
        [DataMember]
        [JsonProperty("ignorarCapacidadeDiaria")]
        public bool IgnorarCapacidadeDiaria { get; set; }
    }
}
