// <copyright file="CadastroAtualizacaoDto.cs" company="Sync Softwares">
// Copyright (c) Sync Softwares. Todos os direitos reservados.
// </copyright>

using Glass.API.Backend.Models.Genericas.V1.CadastroAtualizacao;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.Runtime.Serialization;

namespace Glass.API.Backend.Models.Cfops.V1.NaturezasOperacao.RegrasNaturezaOperacao.CadastroAtualizacao
{
    /// <summary>
    /// Classe que encapsula os dados de cadastro ou atualização de uma regra de natureza de operação.
    /// </summary>
    [DataContract(Name = "CadastroAtualizacao")]
    public class CadastroAtualizacaoDto : BaseCadastroAtualizacaoDto<CadastroAtualizacaoDto>
    {
        /// <summary>
        /// Obtém ou define o identificador da loja da regra de natureza de operação.
        /// </summary>
        [DataMember]
        [JsonProperty("idLoja")]
        public int? IdLoja
        {
            get { return this.ObterValor(c => c.IdLoja); }
            set { this.AdicionarValor(c => c.IdLoja, value); }
        }

        /// <summary>
        /// Obtém ou define o identificador do tipo de cliente da regra de natureza de operação.
        /// </summary>
        [DataMember]
        [JsonProperty("idTipoCliente")]
        public int? IdTipoCliente
        {
            get { return this.ObterValor(c => c.IdTipoCliente); }
            set { this.AdicionarValor(c => c.IdTipoCliente, value); }
        }

        /// <summary>
        /// Obtém ou define dados do produto associados à regra de natureza de operação.
        /// </summary>
        [DataMember]
        [JsonProperty("produto")]
        public ProdutoDto Produto
        {
            get { return this.ObterValor(c => c.Produto); }
            set { this.AdicionarValor(c => c.Produto, value); }
        }

        /// <summary>
        /// Obtém ou define as ufs de destino da regra de natureza de operação.
        /// </summary>
        [DataMember]
        [JsonProperty("ufsDestino")]
        public IEnumerable<string> UfsDestino
        {
            get { return this.ObterValor(c => c.UfsDestino); }
            set { this.AdicionarValor(c => c.UfsDestino, value); }
        }

        /// <summary>
        /// Obtém ou define as naturezas de operação de produção da regra de natureza de operação.
        /// </summary>
        [DataMember]
        [JsonProperty("naturezaOperacaoProducao")]
        public NaturezasOperacaoDto NaturezaOperacaoProducao
        {
            get { return this.ObterValor(c => c.NaturezaOperacaoProducao); }
            set { this.AdicionarValor(c => c.NaturezaOperacaoProducao, value); }
        }

        /// <summary>
        /// Obtém ou define as naturezas de operação de revenda da regra de natureza de operação.
        /// </summary>
        [DataMember]
        [JsonProperty("naturezaOperacaoRevenda")]
        public NaturezasOperacaoDto NaturezaOperacaoRevenda
        {
            get { return this.ObterValor(c => c.NaturezaOperacaoRevenda); }
            set { this.AdicionarValor(c => c.NaturezaOperacaoRevenda, value); }
        }
    }
}
