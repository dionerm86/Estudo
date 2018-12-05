// <copyright file="DadosIcmsDto.cs" company="Sync Softwares">
// Copyright (c) Sync Softwares. Todos os direitos reservados.
// </copyright>

using Glass.API.Backend.Models.Genericas.V1.CadastroAtualizacao;
using Newtonsoft.Json;
using System.Runtime.Serialization;

namespace Glass.API.Backend.Models.Cfops.V1.NaturezasOperacao.CadastroAtualizacao
{
    /// <summary>
    /// Classe que encapsula dados de ICMS da natureza de operação.
    /// </summary>
    [DataContract(Name = "DadosIcms")]
    public class DadosIcmsDto : BaseCadastroAtualizacaoDto<DadosIcmsDto>
    {
        /// <summary>
        /// Obtém ou define o CST de ICMS da natureza de operação.
        /// </summary>
        [DataMember]
        [JsonProperty("cstIcms")]
        public string CstIcms
        {
            get { return this.ObterValor(c => c.CstIcms); }
            set { this.AdicionarValor(c => c.CstIcms, value); }
        }

        /// <summary>
        /// Obtém ou define o CSOSN da natureza de operação.
        /// </summary>
        [DataMember]
        [JsonProperty("csosn")]
        public string Csosn
        {
            get { return this.ObterValor(c => c.Csosn); }
            set { this.AdicionarValor(c => c.Csosn, value); }
        }

        /// <summary>
        /// Obtém ou define um valor que indica se deve ser calculado ICMS.
        /// </summary>
        [DataMember]
        [JsonProperty("calcularIcms")]
        public bool CalcularIcms
        {
            get { return this.ObterValor(c => c.CalcularIcms); }
            set { this.AdicionarValor(c => c.CalcularIcms, value); }
        }

        /// <summary>
        /// Obtém ou define um valor que indica se deve ser calculado ICMS ST.
        /// </summary>
        [DataMember]
        [JsonProperty("calcularIcmsSt")]
        public bool CalcularIcmsSt
        {
            get { return this.ObterValor(c => c.CalcularIcmsSt); }
            set { this.AdicionarValor(c => c.CalcularIcmsSt, value); }
        }

        /// <summary>
        /// Obtém ou define um valor que indica se o IPI deve integrar a base de cálculo do ICMS.
        /// </summary>
        [DataMember]
        [JsonProperty("ipiIntegraBcIcms")]
        public bool IpiIntegraBcIcms
        {
            get { return this.ObterValor(c => c.IpiIntegraBcIcms); }
            set { this.AdicionarValor(c => c.IpiIntegraBcIcms, value); }
        }

        /// <summary>
        /// Obtém ou define um valor que indica se o valor do ICMS desonerado deve ser debitado do total da nota fiscal.
        /// </summary>
        [DataMember]
        [JsonProperty("debitarIcmsDesoneradoTotalNf")]
        public bool DebitarIcmsDesoneradoTotalNf
        {
            get { return this.ObterValor(c => c.DebitarIcmsDesoneradoTotalNf); }
            set { this.AdicionarValor(c => c.DebitarIcmsDesoneradoTotalNf, value); }
        }

        /// <summary>
        /// Obtém ou define o percentual de redução da base de cálculo do ICMS ST.
        /// </summary>
        [DataMember]
        [JsonProperty("percentualReducaoBcIcms")]
        public decimal PercentualReducaoBcIcms
        {
            get { return this.ObterValor(c => c.PercentualReducaoBcIcms); }
            set { this.AdicionarValor(c => c.PercentualReducaoBcIcms, value); }
        }

        /// <summary>
        /// Obtém ou define o percentual de diferimento do ICMS.
        /// </summary>
        [DataMember]
        [JsonProperty("percentualDiferimento")]
        public decimal PercentualDiferimento
        {
            get { return this.ObterValor(c => c.PercentualDiferimento); }
            set { this.AdicionarValor(c => c.PercentualDiferimento, value); }
        }

        /// <summary>
        /// Obtém ou define um valor que indica se deverá ser calculado o DIFAL.
        /// </summary>
        [DataMember]
        [JsonProperty("calcularDifal")]
        public bool CalcularDifal
        {
            get { return this.ObterValor(c => c.CalcularDifal); }
            set { this.AdicionarValor(c => c.CalcularDifal, value); }
        }
    }
}
