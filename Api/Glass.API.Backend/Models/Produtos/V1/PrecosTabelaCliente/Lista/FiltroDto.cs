// <copyright file="FiltroDto.cs" company="Sync Softwares">
// Copyright (c) Sync Softwares. Todos os direitos reservados.
// </copyright>

using Glass.API.Backend.Helper.Produtos;
using Glass.API.Backend.Models.Genericas.V1;
using Newtonsoft.Json;
using System.Collections.Generic;

namespace Glass.API.Backend.Models.Produtos.V1.PrecosTabelaCliente.Lista
{
    /// <summary>
    /// Classe com os itens para o filtro de lista de preços de tabela por cliente.
    /// </summary>
    public class FiltroDto : PaginacaoDto
    {
        /// <summary>
        /// Inicia uma nova instância da classe <see cref="FiltroDto"/>.
        /// </summary>
        public FiltroDto()
            : base(item => new TraducaoOrdenacaoListaProdutos(item.Ordenacao))
        {
        }

        /// <summary>
        /// Obtém ou define o identificador do cliente.
        /// </summary>
        [JsonProperty("idCliente")]
        public int? IdCliente { get; set; }

        /// <summary>
        /// Obtém ou define o nome do cliente.
        /// </summary>
        [JsonProperty("nomeCliente")]
        public string NomeCliente { get; set; }

        /// <summary>
        /// Obtém ou define o identificador do grupo do produto.
        /// </summary>
        [JsonProperty("idGrupoProduto")]
        public int? IdGrupoProduto { get; set; }

        /// <summary>
        /// Obtém ou define os identificadores dos subgrupos de produto.
        /// </summary>
        [JsonProperty("idsSubgrupoProduto")]
        public IEnumerable<int> IdsSubgrupoProduto { get; set; }

        /// <summary>
        /// Obtém ou define o codigo do produto.
        /// </summary>
        [JsonProperty("codigoProduto")]
        public string CodigoProduto { get; set; }

        /// <summary>
        /// Obtém ou define a descrição do produto.
        /// </summary>
        [JsonProperty("descricaoProduto")]
        public string DescricaoProduto { get; set; }

        /// <summary>
        /// Obtém ou define o tipo de valor de tabela.
        /// </summary>
        [JsonProperty("tipoValorTabela")]
        public int? TipoValorTabela { get; set; }

        /// <summary>
        /// Obtém ou define o valor inicial de altura do produto no período.
        /// </summary>
        [JsonProperty("valorAlturaInicio")]
        public decimal? ValorAlturaInicio { get; set; }

        /// <summary>
        /// Obtém ou define o valor final de altura do produto no período.
        /// </summary>
        [JsonProperty("valorAlturaFim")]
        public decimal? ValorAlturaFim { get; set; }

        /// <summary>
        /// Obtém ou define o valor inicial de largura do produto no período.
        /// </summary>
        [JsonProperty("valorLarguraInicio")]
        public decimal? ValorLarguraInicio { get; set; }

        /// <summary>
        /// Obtém ou define o valor final de largura do produto no período.
        /// </summary>
        [JsonProperty("valorLarguraFim")]
        public decimal? ValorLarguraFim { get; set; }

        /// <summary>
        /// Obtém ou define a ordenação manual utilizada na tela.
        /// </summary>
        [JsonProperty("ordenacaoManual")]
        public int? OrdenacaoManual { get; set; }

        /// <summary>
        /// Obtém ou define o codigo do produto.
        /// </summary>
        [JsonProperty("apenasComDesconto")]
        public bool? ApenasComDesconto { get; set; }
    }
}