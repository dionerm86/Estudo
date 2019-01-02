// <copyright file="FiltroDto.cs" company="Sync Softwares">
// Copyright (c) Sync Softwares. Todos os direitos reservados.
// </copyright>

using Glass.API.Backend.Helper.Compras;
using Glass.API.Backend.Models.Genericas.V1;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;

namespace Glass.API.Backend.Models.Compras.V1.Lista
{
    /// <summary>
    /// Classe que encapsula os itens de filtro para a lista de compras.
    /// </summary>
    public class FiltroDto : PaginacaoDto
    {
        /// <summary>
        /// Inicia uma nova instância da classe <see cref="FiltroDto"/>.
        /// </summary>
        public FiltroDto()
            : base(item => new TraducaoOrdenacaoListaCompras(item.Ordenacao))
        {
        }

        /// <summary>
        /// Obtém ou define o identificador da compra.
        /// </summary>
        [JsonProperty("id")]
        public int? Id { get; set; }

        /// <summary>
        /// Obtém ou define o identificador do pedido.
        /// </summary>
        [JsonProperty("idPedido")]
        public int? IdPedido { get; set; }

        /// <summary>
        /// Obtém ou define o identificador da cotação de compra.
        /// </summary>
        [JsonProperty("idCotacaoCompra")]
        public int? IdCotacaoCompra { get; set; }

        /// <summary>
        /// Obtém ou define o número da nota fiscal.
        /// </summary>
        [JsonProperty("notaFiscal")]
        public string NotaFiscal { get; set; }

        /// <summary>
        /// Obtém ou define o identificador do fornecedor.
        /// </summary>
        [JsonProperty("idFornecedor")]
        public int? IdFornecedor { get; set; }

        /// <summary>
        /// Obtém ou define o nome do fornecedor.
        /// </summary>
        [JsonProperty("nomeFornecedor")]
        public string NomeFornecedor { get; set; }

        /// <summary>
        /// Obtém ou define a observação da compra.
        /// </summary>
        [JsonProperty("observacao")]
        public string Observacao { get; set; }

        /// <summary>
        /// Obtém ou define a situação da compra.
        /// </summary>
        [JsonProperty("situacao")]
        public int? Situacao { get; set; }

        /// <summary>
        /// Obtém ou define um valor que indica se serão buscadas compras em atraso.
        /// </summary>
        [JsonProperty("atrasada")]
        public bool? Atrasada { get; set; }

        /// <summary>
        /// Obtém ou define a data inicial referente ao período de cadastro.
        /// </summary>
        [JsonProperty("periodoCadastroInicio")]
        public DateTime? PeriodoCadastroInicio { get; set; }

        /// <summary>
        /// Obtém ou define a data final referente ao período de cadastro.
        /// </summary>
        [JsonProperty("periodoCadastroFim")]
        public DateTime? PeriodoCadastroFim { get; set; }

        /// <summary>
        /// Obtém ou define a data inicial referente ao período de entrega/fábrica.
        /// </summary>
        [JsonProperty("periodoEntregaFabricaInicio")]
        public DateTime? PeriodoEntregaFabricaInicio { get; set; }

        /// <summary>
        /// Obtém ou define a data final referente ao período de entrega/fábrica.
        /// </summary>
        [JsonProperty("periodoEntregaFabricaFim")]
        public DateTime? PeriodoEntregaFabricaFim { get; set; }

        /// <summary>
        /// Obtém ou define a data inicial referente ao período de saída.
        /// </summary>
        [JsonProperty("periodoSaidaInicio")]
        public DateTime? PeriodoSaidaInicio { get; set; }

        /// <summary>
        /// Obtém ou define a data final referente ao período de saída.
        /// </summary>
        [JsonProperty("periodoSaidaFim")]
        public DateTime? PeriodoSaidaFim { get; set; }

        /// <summary>
        /// Obtém ou define a data inicial referente ao período de finalização.
        /// </summary>
        [JsonProperty("periodoFinalizacaoInicio")]
        public DateTime? PeriodoFinalizacaoInicio { get; set; }

        /// <summary>
        /// Obtém ou define a data final referente ao período de finalização.
        /// </summary>
        [JsonProperty("periodoFinalizacaoFim")]
        public DateTime? PeriodoFinalizacaoFim { get; set; }

        /// <summary>
        /// Obtém ou define a data inicial referente ao período de entrada.
        /// </summary>
        [JsonProperty("periodoEntradaInicio")]
        public DateTime? PeriodoEntradaInicio { get; set; }

        /// <summary>
        /// Obtém ou define a data final referente ao período de entrada.
        /// </summary>
        [JsonProperty("periodoEntradaFim")]
        public DateTime? PeriodoEntradaFim { get; set; }

        /// <summary>
        /// Obtém ou define os identificadores dos grupos de produto.
        /// </summary>
        [JsonProperty("idsGrupoProduto")]
        public IEnumerable<int> IdsGrupoProduto { get; set; }

        /// <summary>
        /// Obtém ou define os identificadores dos subgrupos de produto.
        /// </summary>
        [JsonProperty("idSubgrupoProduto")]
        public int? IdSubgrupoProduto { get; set; }

        /// <summary>
        /// Obtém ou define o código do produto.
        /// </summary>
        [JsonProperty("codigoProduto")]
        public string CodigoProduto { get; set; }

        /// <summary>
        /// Obtém ou define a descrição do produto.
        /// </summary>
        [JsonProperty("DescricaoProduto")]
        public string DescricaoProduto { get; set; }

        /// <summary>
        /// Obtém ou define o identificador da loja.
        /// </summary>
        [JsonProperty("idLoja")]
        public int? IdLoja { get; set; }

        /// <summary>
        /// Obtém ou define um valor que indica se serão buscadas compras com o centro de custo divergente.
        /// </summary>
        [JsonProperty("centroDeCustoDivergente")]
        public bool? CentroDeCustoDivergente { get; set; }
    }
}