// <copyright file="CadastroAtualizacaoDto.cs" company="Sync Softwares">
// Copyright (c) Sync Softwares. Todos os direitos reservados.
// </copyright>

using Glass.API.Backend.Models.Genericas.V1.CadastroAtualizacao;
using Newtonsoft.Json;
using System.Runtime.Serialization;

namespace Glass.API.Backend.Models.Cfops.V1.NaturezasOperacao.CadastroAtualizacao
{
    /// <summary>
    /// Classe que encapsula os dados de cadastro ou atualização de uma natureza de operação.
    /// </summary>
    [DataContract(Name = "CadastroAtualizacao")]
    public class CadastroAtualizacaoDto : BaseCadastroAtualizacaoDto<CadastroAtualizacaoDto>
    {
        /// <summary>
        /// Obtém ou define o identificador do CFOP.
        /// </summary>
        [DataMember]
        [JsonProperty("idCfop")]
        public int IdCfop
        {
            get { return this.ObterValor(c => c.IdCfop); }
            set { this.AdicionarValor(c => c.IdCfop, value); }
        }

        /// <summary>
        /// Obtém ou define o código da natureza de operação.
        /// </summary>
        [DataMember]
        [JsonProperty("codigo")]
        public string Codigo
        {
            get { return this.ObterValor(c => c.Codigo); }
            set { this.AdicionarValor(c => c.Codigo, value); }
        }

        /// <summary>
        /// Obtém ou define a mensagem da natureza de operação.
        /// </summary>
        [DataMember]
        [JsonProperty("mensagem")]
        public string Mensagem
        {
            get { return this.ObterValor(c => c.Mensagem); }
            set { this.AdicionarValor(c => c.Mensagem, value); }
        }

        /// <summary>
        /// Obtém ou define um valor que indica se a natureza de operação altera o estoque fiscal.
        /// </summary>
        [DataMember]
        [JsonProperty("alterarEstoqueFiscal")]
        public bool AlterarEstoqueFiscal
        {
            get { return this.ObterValor(c => c.AlterarEstoqueFiscal); }
            set { this.AdicionarValor(c => c.AlterarEstoqueFiscal, value); }
        }

        /// <summary>
        /// Obtém ou define um valor que indica se a natureza de operação será usada para cálculo de energia elétrica.
        /// </summary>
        [DataMember]
        [JsonProperty("calculoDeEnergiaEletrica")]
        public bool CalculoDeEnergiaEletrica
        {
            get { return this.ObterValor(c => c.CalculoDeEnergiaEletrica); }
            set { this.AdicionarValor(c => c.CalculoDeEnergiaEletrica, value); }
        }

        /// <summary>
        /// Obtém ou define o NCM da natureza de operação.
        /// </summary>
        [DataMember]
        [JsonProperty("ncm")]
        public string Ncm
        {
            get { return this.ObterValor(c => c.Ncm); }
            set { this.AdicionarValor(c => c.Ncm, value); }
        }

        /// <summary>
        /// Obtém ou define dados de ICMS da natureza de operação.
        /// </summary>
        [DataMember]
        [JsonProperty("dadosIcms")]
        public DadosIcmsDto DadosIcms
        {
            get { return this.ObterValor(c => c.DadosIcms); }
            set { this.AdicionarValor(c => c.DadosIcms, value); }
        }

        /// <summary>
        /// Obtém ou define dados de IPI da natureza de operação.
        /// </summary>
        [DataMember]
        [JsonProperty("dadosIpi")]
        public DadosIpiDto DadosIpi
        {
            get { return this.ObterValor(c => c.DadosIpi); }
            set { this.AdicionarValor(c => c.DadosIpi, value); }
        }

        /// <summary>
        /// Obtém ou define dados de PIS/COFINS da natureza de operação.
        /// </summary>
        [DataMember]
        [JsonProperty("dadosPisCofins")]
        public DadosPisCofinsDto DadosPisCofins
        {
            get { return this.ObterValor(c => c.DadosPisCofins); }
            set { this.AdicionarValor(c => c.DadosPisCofins, value); }
        }
    }
}
