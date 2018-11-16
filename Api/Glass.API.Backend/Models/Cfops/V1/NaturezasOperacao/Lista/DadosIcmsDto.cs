// <copyright file="DadosIcmsDto.cs" company="Sync Softwares">
// Copyright (c) Sync Softwares. Todos os direitos reservados.
// </copyright>

using Glass.API.Backend.Models.Genericas.V1;
using Newtonsoft.Json;
using System.Runtime.Serialization;

namespace Glass.API.Backend.Models.Cfops.V1.NaturezasOperacao.Lista
{
    /// <summary>
    /// Classe que encapsula dados de ICMS da natureza de operação.
    /// </summary>
    [DataContract(Name = "DadosIcms")]
    public class DadosIcmsDto
    {
        /// <summary>
        /// Obtém ou define o CST de ICMS da natureza de operação.
        /// </summary>
        [DataMember]
        [JsonProperty("cstIcms")]
        public CodigoNomeDto CstIcms { get; set; }

        /// <summary>
        /// Obtém ou define o CSOSN da natureza de operação.
        /// </summary>
        [DataMember]
        [JsonProperty("csosn")]
        public CodigoNomeDto Csosn { get; set; }

        /// <summary>
        /// Obtém ou define um valor que indica se deve ser calculado ICMS.
        /// </summary>
        [DataMember]
        [JsonProperty("calcularIcms")]
        public bool CalcularIcms { get; set; }

        /// <summary>
        /// Obtém ou define um valor que indica se deve ser calculado ICMS ST.
        /// </summary>
        [DataMember]
        [JsonProperty("calcularIcmsSt")]
        public bool CalcularIcmsSt { get; set; }

        /// <summary>
        /// Obtém ou define um valor que indica se o IPI deve integrar a base de cálculo do ICMS.
        /// </summary>
        [DataMember]
        [JsonProperty("ipiIntegraBcIcms")]
        public bool IpiIntegraBcIcms { get; set; }

        /// <summary>
        /// Obtém ou define um valor que indica se o valor do ICMS desonerado deve ser debitado do total da nota fiscal.
        /// </summary>
        [DataMember]
        [JsonProperty("debitarIcmsDesoneradoTotalNf")]
        public bool DebitarIcmsDesoneradoTotalNf { get; set; }

        /// <summary>
        /// Obtém ou define o percentual de redução da base de cálculo do ICMS ST.
        /// </summary>
        [DataMember]
        [JsonProperty("percentualReducaoBcIcms")]
        public decimal PercentualReducaoBcIcms { get; set; }

        /// <summary>
        /// Obtém ou define o percentual de diferimento do ICMS.
        /// </summary>
        [DataMember]
        [JsonProperty("percentualDiferimento")]
        public decimal PercentualDiferimento { get; set; }

        /// <summary>
        /// Obtém ou define um valor que indica se deverá ser calculado o DIFAL.
        /// </summary>
        [DataMember]
        [JsonProperty("calcularDifal")]
        public bool CalcularDifal { get; set; }
    }
}
