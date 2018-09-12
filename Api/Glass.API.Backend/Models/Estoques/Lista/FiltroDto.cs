// <copyright file="FiltroDto.cs" company="Sync Softwares">
// Copyright (c) Sync Softwares. Todos os direitos reservados.
// </copyright>

using Glass.API.Backend.Helper.Estoques;
using Glass.API.Backend.Models.Genericas;
using Newtonsoft.Json;
using System;

namespace Glass.API.Backend.Models.Estoques.Lista
{
    /// <summary>
    /// Classe com os itens para o filtro de lista de estoques.
    /// </summary>
    public class FiltroDto : PaginacaoDto
    {
        /// <summary>
        /// Inicia uma nova instância da classe <see cref="FiltroDto"/>.
        /// </summary>
        public FiltroDto()
            : base(item => new TraducaoOrdenacaoListaEstoques(item.Ordenacao))
        {
        }

        /// <summary>
        /// Obtém ou define o identificador da loja.
        /// </summary>
        [JsonProperty("idLoja")]
        public int? IdLoja { get; set; }

        /// <summary>
        /// Obtém ou define o código interno do produto.
        /// </summary>
        [JsonProperty("codigoInternoProduto")]
        public string CodigoInternoProduto { get; set; }

        /// <summary>
        /// Obtém ou define a descrição do produto.
        /// </summary>
        [JsonProperty("descricaoProduto ")]
        public string DescricaoProduto { get; set; }

        /// <summary>
        /// Obtém ou define o identificador do grupo do produto.
        /// </summary>
        [JsonProperty("idGrupoProduto")]
        public int? IdGrupoProduto { get; set; }

        /// <summary>
        /// Obtém ou define o identificador do subgrupo do produto.
        /// </summary>
        [JsonProperty("idSubgrupoProduto")]
        public int? IdSubgrupoProduto { get; set; }

        /// <summary>
        /// Obtém ou define um valor que indica se serão retornados apenas produtos com estoque.
        /// </summary>
        [JsonProperty("apenasComEstoque")]
        public bool ApenasComEstoque { get; set; }

        /// <summary>
        /// Obtém ou define um valor que indica se serão retornados apenas produtos em posse de terceiros.
        /// </summary>
        [JsonProperty("apenasPosseTerceiros")]
        public bool ApenasPosseTerceiros { get; set; }

        /// <summary>
        /// Obtém ou define um valor que indica se serão retornados apenas produtos produtos de projeto.
        /// </summary>
        [JsonProperty("apenasProdutosProjeto")]
        public bool ApenasProdutosProjeto { get; set; }

        /// <summary>
        /// Obtém ou define a cor do vidro.
        /// </summary>
        [JsonProperty("idCorVidro")]
        public int? IdCorVidro { get; set; }

        /// <summary>
        /// Obtém ou define a cor da ferragem.
        /// </summary>
        [JsonProperty("idCorFerragem")]
        public int? IdCorFerragem { get; set; }

        /// <summary>
        /// Obtém ou define a cor do alumínio.
        /// </summary>
        [JsonProperty("idCorAluminio")]
        public int? IdCorAluminio { get; set; }

        /// <summary>
        /// Obtém ou define a situação do produto.
        /// </summary>
        [JsonProperty("situacao")]
        public Situacao? Situacao { get; set; }

        /// <summary>
        /// Obtém ou define um valor que indica se deverá retornar o estoque fiscal dos produtos.
        /// </summary>
        [JsonProperty("estoqueFiscal")]
        public bool EstoqueFiscal { get; set; }

        /// <summary>
        /// Obtém ou define um valor que indica se deverá retornar produtos aguardando sair do estoque.
        /// </summary>
        [JsonProperty("aguardandoSaidaEstoque")]
        public bool AguardandoSaidaEstoque { get; set; }

        /// <summary>
        /// Obtém ou define a ordenação a ser usada na listagem.
        /// </summary>
        [JsonProperty("ordenacaoFiltro")]
        public int OrdenacaoFiltro { get; set; }
    }
}
