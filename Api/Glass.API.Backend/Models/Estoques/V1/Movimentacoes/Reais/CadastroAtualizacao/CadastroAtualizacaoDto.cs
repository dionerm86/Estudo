// <copyright file="CadastroAtualizacaoDto.cs" company="Sync Softwares">
// Copyright (c) Sync Softwares. Todos os direitos reservados.
// </copyright>

using Glass.API.Backend.Models.Genericas.V1.CadastroAtualizacao;
using Glass.Data.Model;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Runtime.Serialization;

namespace Glass.API.Backend.Models.Estoques.V1.Movimentacoes.Reais.CadastroAtualizacao
{
    /// <summary>
    /// Classe que encapsula os dados de cadastro ou atualização de uma movimentação do estoque real.
    /// </summary>
    [DataContract(Name = "CadastroAtualizacao")]
    public class CadastroAtualizacaoDto : BaseCadastroAtualizacaoDto<CadastroAtualizacaoDto>
    {
        /// <summary>
        /// Obtém ou define a data da movimentação.
        /// </summary>
        [DataMember]
        [JsonProperty("dataMovimentacao")]
        public DateTime DataMovimentacao
        {
            get { return this.ObterValor(c => c.DataMovimentacao); }
            set { this.AdicionarValor(c => c.DataMovimentacao, value); }
        }

        /// <summary>
        /// Obtém ou define a quantidade movimentada.
        /// </summary>
        [DataMember]
        [JsonProperty("quantidade")]
        public decimal Quantidade
        {
            get { return this.ObterValor(c => c.Quantidade); }
            set { this.AdicionarValor(c => c.Quantidade, value); }
        }

        /// <summary>
        /// Obtém ou define o tipo de movimentação (entrada/saída).
        /// </summary>
        [DataMember]
        [JsonProperty("tipoMovimentacao")]
        public int TipoMovimentacao
        {
            get { return this.ObterValor(c => c.TipoMovimentacao); }
            set { this.AdicionarValor(c => c.TipoMovimentacao, value); }
        }

        /// <summary>
        /// Obtém ou define o valor total da movimentação.
        /// </summary>
        [DataMember]
        [JsonProperty("valor")]
        public decimal Valor
        {
            get { return this.ObterValor(c => c.Valor); }
            set { this.AdicionarValor(c => c.Valor, value); }
        }

        /// <summary>
        /// Obtém ou define uma observacao para a movimentação.
        /// </summary>
        [DataMember]
        [JsonProperty("observacao")]
        public string Observacao
        {
            get { return this.ObterValor(c => c.Observacao); }
            set { this.AdicionarValor(c => c.Observacao, value); }
        }

        /// <summary>
        /// Obtém ou define o produto associado a movimentação.
        /// </summary>
        [DataMember]
        [JsonProperty("idProduto")]
        public int IdProduto
        {
            get { return this.ObterValor(c => c.IdProduto); }
            set { this.AdicionarValor(c => c.IdProduto, value); }
        }

        /// <summary>
        /// Obtém ou define a loja em que será realizada a movimentação.
        /// </summary>
        [DataMember]
        [JsonProperty("idLoja")]
        public int IdLoja
        {
            get { return this.ObterValor(c => c.IdLoja); }
            set { this.AdicionarValor(c => c.IdLoja, value); }
        }
    }
}
