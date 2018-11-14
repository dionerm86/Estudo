// <copyright file="DadosPisCofinsDto.cs" company="Sync Softwares">
// Copyright (c) Sync Softwares. Todos os direitos reservados.
// </copyright>

using Glass.API.Backend.Models.Genericas.V1.CadastroAtualizacao;
using Newtonsoft.Json;
using System.Runtime.Serialization;

namespace Glass.API.Backend.Models.Cfops.V1.NaturezasOperacao.CadastroAtualizacao
{
    /// <summary>
    /// Classe que encapsula dados de IPI da natureza de operação.
    /// </summary>
    [DataContract(Name = "DadosPisCofins")]
    public class DadosPisCofinsDto : BaseCadastroAtualizacaoDto<DadosPisCofinsDto>
    {
        /// <summary>
        /// Obtém ou define o CST de PIS/COFINS da natureza de operação.
        /// </summary>
        [DataMember]
        [JsonProperty("cstPisCofins")]
        public Data.EFD.DataSourcesEFD.CstPisCofinsEnum? CstPisCofins
        {
            get { return this.ObterValor(c => c.CstPisCofins); }
            set { this.AdicionarValor(c => c.CstPisCofins, value); }
        }

        /// <summary>
        /// Obtém ou define um valor que indica se deve ser calculado PIS.
        /// </summary>
        [DataMember]
        [JsonProperty("calcularPis")]
        public bool CalcularPis
        {
            get { return this.ObterValor(c => c.CalcularPis); }
            set { this.AdicionarValor(c => c.CalcularPis, value); }
        }

        /// <summary>
        /// Obtém ou define um valor que indica se deve ser calculado COFINS.
        /// </summary>
        [DataMember]
        [JsonProperty("calcularCofins")]
        public bool CalcularCofins
        {
            get { return this.ObterValor(c => c.CalcularCofins); }
            set { this.AdicionarValor(c => c.CalcularCofins, value); }
        }
    }
}
