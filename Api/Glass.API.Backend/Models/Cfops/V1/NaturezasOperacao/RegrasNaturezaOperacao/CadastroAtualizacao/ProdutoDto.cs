// <copyright file="ProdutoDto.cs" company="Sync Softwares">
// Copyright (c) Sync Softwares. Todos os direitos reservados.
// </copyright>

using Glass.API.Backend.Models.Genericas.V1.CadastroAtualizacao;
using Newtonsoft.Json;
using System.Runtime.Serialization;

namespace Glass.API.Backend.Models.Cfops.V1.NaturezasOperacao.RegrasNaturezaOperacao.CadastroAtualizacao
{
    /// <summary>
    /// Classe que encapsula dados do produto da regra de natureza de operação.
    /// </summary>
    [DataContract(Name = "Produto")]
    public class ProdutoDto : BaseCadastroAtualizacaoDto<ProdutoDto>
    {
        /// <summary>
        /// Obtém ou define o código do cliente da regra de natureza de operação.
        /// </summary>
        [DataMember]
        [JsonProperty("idGrupoProduto")]
        public int? IdGrupoProduto
        {
            get { return this.ObterValor(c => c.IdGrupoProduto); }
            set { this.AdicionarValor(c => c.IdGrupoProduto, value); }
        }

        /// <summary>
        /// Obtém ou define o subgrupo de produto da regra de natureza de operação.
        /// </summary>
        [DataMember]
        [JsonProperty("idSubgrupoProduto")]
        public int? IdSubgrupoProduto
        {
            get { return this.ObterValor(c => c.IdSubgrupoProduto); }
            set { this.AdicionarValor(c => c.IdSubgrupoProduto, value); }
        }

        /// <summary>
        /// Obtém ou define a espessura da regra de natureza de operação.
        /// </summary>
        [DataMember]
        [JsonProperty("espessura")]
        public float? Espessura
        {
            get { return this.ObterValor(c => c.Espessura); }
            set { this.AdicionarValor(c => c.Espessura, value); }
        }

        /// <summary>
        /// Obtém ou define as cores da regra de natureza de operação.
        /// </summary>
        [DataMember]
        [JsonProperty("cores")]
        public CoresDto Cores
        {
            get { return this.ObterValor(c => c.Cores); }
            set { this.AdicionarValor(c => c.Cores, value); }
        }
    }
}
