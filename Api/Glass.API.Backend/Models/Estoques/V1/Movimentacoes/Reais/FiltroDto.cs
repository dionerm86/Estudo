// <copyright file="FiltroDto.cs" company="Sync Softwares">
// Copyright (c) Sync Softwares. Todos os direitos reservados.
// </copyright>

using Glass.API.Backend.Helper.Estoques.Movimentacoes.Reais;
using Glass.API.Backend.Models.Genericas.V1;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Runtime.Serialization;

namespace Glass.API.Backend.Models.Estoques.V1.Movimentacoes.Reais
{
    /// <summary>
    /// Classe com os itens para o filtro da lista de movimentação do estoque real.
    /// </summary>
    [DataContract(Name = "Filtro")]
    public class FiltroDto : PaginacaoDto
    {
        /// <summary>
        /// Inicia uma nova instância da classe <see cref="FiltroDto"/>.
        /// </summary>
        public FiltroDto()
            : base(item => new TraducaoOrdenacaoMovimentacaoEstoqueReal(item.Ordenacao))
        {
        }

        /// <summary>
        /// Obtém ou define o identificador da loja.
        /// </summary>
        [JsonProperty("idLoja")]
        public int? IdLoja { get; set; }

        /// <summary>
        /// Obtém ou define o código do produto.
        /// </summary>
        [JsonProperty("codigoProduto")]
        public string CodigoProduto { get; set; }

        /// <summary>
        /// Obtém ou define a descrição do produto.
        /// </summary>
        [JsonProperty("descricaoProduto")]
        public string DescricaoProduto { get; set; }

        /// <summary>
        /// Obtém ou define o período inicial da movimentação.
        /// </summary>
        [JsonProperty("periodoMovimentacaoInicio")]
        public DateTime? PeriodoMovimentacaoInicio { get; set; }

        /// <summary>
        /// Obtém ou define o período final da movimentação.
        /// </summary>
        [JsonProperty("periodoMovimentacaoFim")]
        public DateTime? PeriodoMovimentacaoFim { get; set; }

        /// <summary>
        /// Obtém ou define o tipo da movimentação.
        /// </summary>
        [JsonProperty("tipoMovimentacao")]
        public int? TipoMovimentacao { get; set; }

        /// <summary>
        /// Obtém ou define a situação do produto.
        /// </summary>
        [JsonProperty("situacaoProduto")]
        public Situacao? SituacaoProduto { get; set; }

        /// <summary>
        /// Obtém ou define os identificadores dos grupos de produtos.
        /// </summary>
        [JsonProperty("idsGrupoProduto")]
        public IEnumerable<int> IdsGrupoProduto { get; set; }

        /// <summary>
        /// Obtém ou define os identificadores dos subgrupos de produtos.
        /// </summary>
        [JsonProperty("idsSubgrupoProduto")]
        public IEnumerable<int> IdsSubgrupoProduto { get; set; }

        /// <summary>
        /// Obtém ou define o código de otimização do produto.
        /// </summary>
        [JsonProperty("codigoOtimizacao")]
        public string CodigoOtimizacao { get; set; }

        /// <summary>
        /// Obtém ou define o identificador da cor do vidro.
        /// </summary>
        [JsonProperty("idCorVidro")]
        public int? IdCorVidro { get; set; }

        /// <summary>
        /// Obtém ou define o identificador da cor da ferragem.
        /// </summary>
        [JsonProperty("idCorFerragem")]
        public int? IdCorFerragem { get; set; }

        /// <summary>
        /// Obtém ou define o identificador da cor do aluminio.
        /// </summary>
        [JsonProperty("idCorAluminio")]
        public int? IdCorAluminio { get; set; }

        /// <summary>
        /// Obtém ou define um valor que indica se serão exibidos somente lançamentos manuais.
        /// </summary>
        [JsonProperty("apenasLancamentosManuais")]
        public bool? ApenasLancamentosManuais { get; set; }

        /// <summary>
        /// Obtém ou define um valor que indica se serão exibidos somente produtos com saldo de estoque.
        /// </summary>
        [JsonProperty("naoBuscarProdutosComEstoqueZerado")]
        public bool? NaoBuscarProdutosComEstoqueZerado { get; set; }
    }
}
