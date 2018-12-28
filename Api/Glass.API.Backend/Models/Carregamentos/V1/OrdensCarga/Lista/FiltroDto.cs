// <copyright file="FiltroDto.cs" company="Sync Softwares">
// Copyright (c) Sync Softwares. Todos os direitos reservados.
// </copyright>

using Glass.API.Backend.Helper.Carregamentos.OrdensCarga;
using Glass.API.Backend.Models.Genericas.V1;
using Glass.Data.Model;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;

namespace Glass.API.Backend.Models.Carregamentos.V1.OrdensCarga.Lista
{
    /// <summary>
    /// Classe que encapsula os itens de filtro para a lista de ordens de carga.
    /// </summary>
    public class FiltroDto : PaginacaoDto
    {
        /// <summary>
        /// Inicia uma nova instância da classe <see cref="FiltroDto"/>.
        /// </summary>
        public FiltroDto()
            : base(item => new TraducaoOrdenacaoListaOrdensCarga(item.Ordenacao))
        {
        }

        /// <summary>
        /// Obtém ou define o identificador do carregamento.
        /// </summary>
        [JsonProperty("idCarregamento")]
        public int? IdCarregamento { get; set; }

        /// <summary>
        /// Obtém ou define o identificador da ordem de carga.
        /// </summary>
        [JsonProperty("id")]
        public int? Id { get; set; }

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
        /// Obtém ou define o identificador da loja.
        /// </summary>
        [JsonProperty("idLoja")]
        public int? IdLoja { get; set; }

        /// <summary>
        /// Obtém ou define o identificador da rota.
        /// </summary>
        [JsonProperty("idRota")]
        public int? IdRota { get; set; }

        /// <summary>
        /// Obtém ou define o identificador do pedido.
        /// </summary>
        [JsonProperty("idPedido")]
        public int? IdPedido { get; set; }

        /// <summary>
        /// Obtém ou define o período de entrega inicial do pedido.
        /// </summary>
        [JsonProperty("periodoEntregaPedidoInicio")]
        public DateTime? PeriodoEntregaPedidoInicio { get; set; }

        /// <summary>
        /// Obtém ou define o período de entrega final do pedido.
        /// </summary>
        [JsonProperty("periodoEntregaPedidoFim")]
        public DateTime? PeriodoEntregaPedidoFim { get; set; }

        /// <summary>
        /// Obtém ou define as situaçãoes da ordem de carga.
        /// </summary>
        [JsonProperty("situacoesOrdemCarga")]
        public IEnumerable<OrdemCarga.SituacaoOCEnum> SituacoesOrdemCarga { get; set; }

        /// <summary>
        /// Obtém ou define os tipos de ordem de carga.
        /// </summary>
        [JsonProperty("tiposOrdemCarga")]
        public IEnumerable<OrdemCarga.TipoOCEnum> TiposOrdemCarga { get; set; }

        /// <summary>
        /// Obtém ou define o identificador do cliente externo.
        /// </summary>
        [JsonProperty("idClienteExterno")]
        public int? IdClienteExterno { get; set; }

        /// <summary>
        /// Obtém ou define o nome do cliente externo.
        /// </summary>
        [JsonProperty("nomeClienteExterno")]
        public string NomeClienteExterno { get; set; }

        /// <summary>
        /// Obtém ou define os identificadores das rotas externas.
        /// </summary>
        [JsonProperty("idsRotaExterna")]
        public IEnumerable<int> IdsRotaExterna { get; set; }
    }
}