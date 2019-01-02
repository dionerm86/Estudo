// <copyright file="CadastroAtualizacaoDto.cs" company="Sync Softwares">
// Copyright (c) Sync Softwares. Todos os direitos reservados.
// </copyright>

using Glass.API.Backend.Models.Genericas.V1.CadastroAtualizacao;
using Newtonsoft.Json;
using System;
using System.Runtime.Serialization;

namespace Glass.API.Backend.Models.ContasPagar.V1.CadastroAtualizacao
{
    /// <summary>
    /// Classe que encapsula os dados de cadastro ou atualização de uma conta a pagar.
    /// </summary>
    [DataContract(Name = "CadastroAtualizacao")]
    public class CadastroAtualizacaoDto : BaseCadastroAtualizacaoDto<CadastroAtualizacaoDto>
    {
        /// <summary>
        /// Obtém ou define o identificador do plano de contas da conta a pagar.
        /// </summary>
        [DataMember]
        [JsonProperty("idPlanoConta")]
        public int? IdPlanoConta
        {
            get { return this.ObterValor(c => c.IdPlanoConta); }
            set { this.AdicionarValor(c => c.IdPlanoConta, value); }
        }

        /// <summary>
        /// Obtém ou define o identificador da forma de pagamento da conta a pagar.
        /// </summary>
        [DataMember]
        [JsonProperty("idFormaPagamento")]
        public int? IdFormaPagamento
        {
            get { return this.ObterValor(c => c.IdFormaPagamento); }
            set { this.AdicionarValor(c => c.IdFormaPagamento, value); }
        }

        /// <summary>
        /// Obtém ou define a data de vencimento da conta a pagar.
        /// </summary>
        [DataMember]
        [JsonProperty("dataVencimento")]
        public DateTime? DataVencimento
        {
            get { return this.ObterValor(c => c.DataVencimento); }
            set { this.AdicionarValor(c => c.DataVencimento, value); }
        }

        /// <summary>
        /// Obtém ou define a observação da conta a pagar.
        /// </summary>
        [DataMember]
        [JsonProperty("observacao")]
        public string Observacao
        {
            get { return this.ObterValor(c => c.Observacao); }
            set { this.AdicionarValor(c => c.Observacao, value); }
        }
    }
}