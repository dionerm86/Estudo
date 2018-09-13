// <copyright file="FiltroDto.cs" company="Sync Softwares">
// Copyright (c) Sync Softwares. Todos os direitos reservados.
// </copyright>

using Glass.API.Backend.Helper.Produtos;
using Glass.API.Backend.Models.Genericas;
using Newtonsoft.Json;

namespace Glass.API.Backend.Models.Produtos.Lista
{
    /// <summary>
    /// Classe com os itens para o filtro de lista de produtos.
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
        /// Obtém ou define o código do produto.
        /// </summary>
        [JsonProperty("codigo")]
        public string Codigo { get; set; }

        /// <summary>
        /// Obtém ou define a descrição do produto.
        /// </summary>
        [JsonProperty("descricao")]
        public string Descricao { get; set; }

        /// <summary>
        /// Obtém ou define a situação do produto.
        /// </summary>
        [JsonProperty("situacao")]
        public Situacao? Situacao { get; set; }

        /// <summary>
        /// Obtém ou define o identificador do grupo do produto.
        /// </summary>
        [JsonProperty("idGrupo")]
        public int? IdGrupo { get; set; }

        /// <summary>
        /// Obtém ou define o identificador do subgrupo do produto.
        /// </summary>
        [JsonProperty("idSubgrupo")]
        public int? IdSubgrupo { get; set; }

        /// <summary>
        /// Obtém ou define o valor inicial do filtro de altura.
        /// </summary>
        [JsonProperty("valorAlturaInicio")]
        public decimal ValorAlturaInicio { get; set; }

        /// <summary>
        /// Obtém ou define o valor final do filtro de altura.
        /// </summary>
        [JsonProperty("valorAlturaFim")]
        public decimal? ValorAlturaFim { get; set; }

        /// <summary>
        /// Obtém ou define o valor inicial do filtro de largura.
        /// </summary>
        [JsonProperty("valorLarguraInicio")]
        public decimal? ValorLarguraInicio { get; set; }

        /// <summary>
        /// Obtém ou define o valor final do filtro de largura.
        /// </summary>
        [JsonProperty("valorLarguraFim")]
        public decimal? ValorLarguraFim { get; set; }

        /// <summary>
        /// Obtém ou define o tipo de ordenação que será feito no relatório de produtos.
        /// </summary>
        [JsonProperty("ordenacaoFiltro")]
        public int OrdenacaoFiltro { get; set; }
    }
}
