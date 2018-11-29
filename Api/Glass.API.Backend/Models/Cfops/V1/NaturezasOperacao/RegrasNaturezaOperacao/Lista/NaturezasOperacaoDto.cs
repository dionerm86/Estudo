// <copyright file="NaturezasOperacaoDto.cs" company="Sync Softwares">
// Copyright (c) Sync Softwares. Todos os direitos reservados.
// </copyright>

using Glass.API.Backend.Models.Genericas.V1;
using Newtonsoft.Json;
using System.Runtime.Serialization;

namespace Glass.API.Backend.Models.Cfops.V1.NaturezasOperacao.RegrasNaturezaOperacao.Lista
{
    /// <summary>
    /// Classe que encapsula dados das naturezas de operação associadas à regra de natureza de operação.
    /// </summary>
    [DataContract(Name = "NaturezasOperacao")]
    public class NaturezasOperacaoDto
    {
        /// <summary>
        /// Obtém ou define a natureza de operação intraestadual.
        /// </summary>
        [DataMember]
        [JsonProperty("intraestadual")]
        public IdNomeDto Intraestadual { get; set; }

        /// <summary>
        /// Obtém ou define a natureza de operação interestadual.
        /// </summary>
        [DataMember]
        [JsonProperty("interestadual")]
        public IdNomeDto Interestadual { get; set; }

        /// <summary>
        /// Obtém ou define a natureza de operação intraestadual com ST.
        /// </summary>
        [DataMember]
        [JsonProperty("intraestadualComSt")]
        public IdNomeDto IntraestadualComSt { get; set; }

        /// <summary>
        /// Obtém ou define a natureza de operação interestadual com ST.
        /// </summary>
        [DataMember]
        [JsonProperty("interestadualComSt")]
        public IdNomeDto InterestadualComSt { get; set; }
    }
}
