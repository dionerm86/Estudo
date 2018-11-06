// <copyright file="CoresDto.cs" company="Sync Softwares">
// Copyright (c) Sync Softwares. Todos os direitos reservados.
// </copyright>

using Glass.API.Backend.Models.Genericas.V1;
using Newtonsoft.Json;
using System.Runtime.Serialization;

namespace Glass.API.Backend.Models.Cfops.V1.NaturezasOperacao.RegrasNaturezaOperacao.Lista
{
    /// <summary>
    /// Classe que encapsula dados das cores da regra de natureza de operação.
    /// </summary>
    [DataContract(Name = "Cores")]
    public class CoresDto
    {
        /// <summary>
        /// Obtém ou define a cor do vidro da regra de natureza de operação.
        /// </summary>
        [DataMember]
        [JsonProperty("vidro")]
        public IdNomeDto Vidro { get; set; }

        /// <summary>
        /// Obtém ou define a cor da ferragem da regra de natureza de operação.
        /// </summary>
        [DataMember]
        [JsonProperty("ferragem")]
        public IdNomeDto Ferragem { get; set; }

        /// <summary>
        /// Obtém ou define a cor do alumínio da regra de natureza de operação.
        /// </summary>
        [DataMember]
        [JsonProperty("aluminio")]
        public IdNomeDto Aluminio { get; set; }
    }
}
