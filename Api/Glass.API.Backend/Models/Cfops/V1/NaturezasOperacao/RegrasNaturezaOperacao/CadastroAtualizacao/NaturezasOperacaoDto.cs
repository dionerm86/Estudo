// <copyright file="NaturezasOperacaoDto.cs" company="Sync Softwares">
// Copyright (c) Sync Softwares. Todos os direitos reservados.
// </copyright>

using Glass.API.Backend.Models.Genericas.V1.CadastroAtualizacao;
using Newtonsoft.Json;
using System.Runtime.Serialization;

namespace Glass.API.Backend.Models.Cfops.V1.NaturezasOperacao.RegrasNaturezaOperacao.CadastroAtualizacao
{
    /// <summary>
    /// Classe que encapsula dados das naturezas de operação da regra de natureza de operação.
    /// </summary>
    [DataContract(Name = "Cores")]
    public class NaturezasOperacaoDto : BaseCadastroAtualizacaoDto<NaturezasOperacaoDto>
    {
        /// <summary>
        /// Obtém ou define a natureza de operação intraestadual da regra de natureza de operação.
        /// </summary>
        [DataMember]
        [JsonProperty("intraestadual")]
        public int Intraestadual
        {
            get { return this.ObterValor(c => c.Intraestadual); }
            set { this.AdicionarValor(c => c.Intraestadual, value); }
        }

        /// <summary>
        /// Obtém ou define a natureza de operação interestadual da regra de natureza de operação.
        /// </summary>
        [DataMember]
        [JsonProperty("interestadual")]
        public int Interestadual
        {
            get { return this.ObterValor(c => c.Interestadual); }
            set { this.AdicionarValor(c => c.Interestadual, value); }
        }

        /// <summary>
        /// Obtém ou define a natureza de operação intraestadual com ST da regra de natureza de operação.
        /// </summary>
        [DataMember]
        [JsonProperty("intraestadualComSt")]
        public int? IntraestadualComSt
        {
            get { return this.ObterValor(c => c.IntraestadualComSt); }
            set { this.AdicionarValor(c => c.IntraestadualComSt, value); }
        }

        /// <summary>
        /// Obtém ou define a natureza de operação interestadual com ST da regra de natureza de operação.
        /// </summary>
        [DataMember]
        [JsonProperty("interestadualComSt")]
        public int? InterestadualComSt
        {
            get { return this.ObterValor(c => c.InterestadualComSt); }
            set { this.AdicionarValor(c => c.InterestadualComSt, value); }
        }
    }
}
