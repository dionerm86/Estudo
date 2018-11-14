// <copyright file="DadosIpiDto.cs" company="Sync Softwares">
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
    [DataContract(Name = "DadosIpi")]
    public class DadosIpiDto : BaseCadastroAtualizacaoDto<DadosIpiDto>
    {
        /// <summary>
        /// Obtém ou define o CST de IPI da natureza de operação.
        /// </summary>
        [DataMember]
        [JsonProperty("cstIpi")]
        public Data.Model.ProdutoCstIpi? CstIpi
        {
            get { return this.ObterValor(c => c.CstIpi); }
            set { this.AdicionarValor(c => c.CstIpi, value); }
        }

        /// <summary>
        /// Obtém ou define um valor que indica se deve ser calculado IPI.
        /// </summary>
        [DataMember]
        [JsonProperty("calcularIpi")]
        public bool CalcularIpi
        {
            get { return this.ObterValor(c => c.CalcularIpi); }
            set { this.AdicionarValor(c => c.CalcularIpi, value); }
        }

        /// <summary>
        /// Obtém ou define um valor que indica se o frete deve integrar a base de cálculo do IPI.
        /// </summary>
        [DataMember]
        [JsonProperty("freteIntegraBcIpi")]
        public bool FreteIntegraBcIpi
        {
            get { return this.ObterValor(c => c.FreteIntegraBcIpi); }
            set { this.AdicionarValor(c => c.FreteIntegraBcIpi, value); }
        }

        /// <summary>
        /// Obtém ou define o código de enquadramento do IPI.
        /// </summary>
        [DataMember]
        [JsonProperty("codigoEnquadramentoIpi")]
        public string CodigoEnquadramentoIpi
        {
            get { return this.ObterValor(c => c.CodigoEnquadramentoIpi); }
            set { this.AdicionarValor(c => c.CodigoEnquadramentoIpi, value); }
        }
    }
}
