// <copyright file="FiltroDto.cs" company="Sync Softwares">
// Copyright (c) Sync Softwares. Todos os direitos reservados.
// </copyright>

using Glass.API.Backend.Helper.Produtos.MateriaPrima.Posicoes;
using Glass.API.Backend.Models.Genericas.V1;
using Glass.API.Backend.Models.Pedidos.V1.TipoPedidoPCPEnum;
using Glass.Data.Model;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;

namespace Glass.API.Backend.Models.Produtos.V1.MateriaPrima.Posicao.Lista
{
    /// <summary>
    /// Classe com os itens para o filtro de lista de posições de matéria prima.
    /// </summary>
    public class FiltroDto : PaginacaoDto
    {
        /// <summary>
        /// Inicia uma nova instância da classe <see cref="FiltroDto"/>.
        /// </summary>
        public FiltroDto()
            : base(item => new TraducaoOrdenacaoListaPosicoesMateriaPrima(item.Ordenacao))
        {
        }

        /// <summary>
        /// Obtém ou define os identificadores das rotas.
        /// </summary>
        [JsonProperty("idsRota")]
        public IEnumerable<int> IdsRota { get; set; }

        /// <summary>
        /// Obtém ou define os tipos de Pedido.
        /// </summary>
        [JsonProperty("tiposPedido")]
        public TipoPedidoPCPEnum TiposPedido { get; set; }

        /// <summary>
        /// Obtém ou define as situações de pedido.
        /// </summary>
        [JsonProperty("situacoesPedido")]
        public Data.Model.Pedido.SituacaoPedido SituacoesPedido { get; set; }

        /// <summary>
        /// Obtém ou define o nome do cliente.
        /// </summary>
        [JsonProperty("nomeCliente")]
        public string NomeCliente { get; set; }

        /// <summary>
        /// Obtém ou define a data inicial no período de entrega do pedido.
        /// </summary>
        [JsonProperty("periodoEntregaPedidoInicio")]
        public DateTime? PeriodoEntregaPedidoInicio { get; set; }

        /// <summary>
        /// Obtém ou define a data final no período de entrega do pedido.
        /// </summary>
        [JsonProperty("periodoEntregaPedidoFim")]
        public DateTime? PeriodoEntregaPedidoFim { get; set; }

        /// <summary>
        /// Obtém ou define os identificadores das cores do vidro.
        /// </summary>
        [JsonProperty("idsCorVidro")]
        public IEnumerable<int> IdsCorVidro { get; set; }

        /// <summary>
        /// Obtém ou define a espessura do vidro.
        /// </summary>
        [JsonProperty("espessura")]
        public decimal? Espessura { get; set; }

        /// <summary>
        /// Obtém ou define um valor que indica se serão buscadas apenas posições de matéria prima com estoque negativo.
        /// </summary>
        [JsonProperty("buscarApenasEstoqueDisponivelNegativo")]
        public bool? BuscarApenasEstoqueDisponivelNegativo { get; set; }
    }
}
