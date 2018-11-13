// <copyright file="DadosPisCofinsDto.cs" company="Sync Softwares">
// Copyright (c) Sync Softwares. Todos os direitos reservados.
// </copyright>

using Glass.API.Backend.Models.Genericas.V1;
using Newtonsoft.Json;
using System.Runtime.Serialization;

namespace Glass.API.Backend.Models.Cfops.V1.NaturezasOperacao.Lista
{
    /// <summary>
    /// Classe que encapsula dados de IPI da natureza de operação.
    /// </summary>
    [DataContract(Name = "DadosPisCofins")]
    public class DadosPisCofinsDto
    {
        /// <summary>
        /// Obtém ou define o CST de PIS/COFINS da natureza de operação.
        /// </summary>
        [DataMember]
        [JsonProperty("cstPisCofins")]
        public IdNomeDto CstPisCofins { get; set; }

        /// <summary>
        /// Obtém ou define um valor que indica se deve ser calculado PIS.
        /// </summary>
        [DataMember]
        [JsonProperty("calcularPis")]
        public bool CalcularPis { get; set; }

        /// <summary>
        /// Obtém ou define um valor que indica se deve ser calculado COFINS.
        /// </summary>
        [DataMember]
        [JsonProperty("calcularCofins")]
        public bool CalcularCofins { get; set; }
    }
}
