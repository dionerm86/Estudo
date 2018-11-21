// <copyright file="FiltroDto.cs" company="Sync Softwares">
// Copyright (c) Sync Softwares. Todos os direitos reservados.
// </copyright>

using Glass.API.Backend.Helper.Pedidos;
using Glass.API.Backend.Models.Genericas.V1;
using System;
using System.Collections.Generic;

namespace Glass.API.Backend.Models.Pedidos.V1.ListaVolumes
{
    /// <summary>
    /// Classe com os itens para o filtro de lista de pedidos.
    /// </summary>
    public class FiltroDto : PaginacaoDto
    {
        /// <summary>
        /// Inicia uma nova instância da classe <see cref="FiltroDto"/>.
        /// </summary>
        public FiltroDto()
            : base(item => new TraducaoOrdenacaoListaPedidosVolumes(item.Ordenacao))
        {
        }

        /// <summary>
        /// Obtém ou define o identificador do pedido.
        /// </summary>
        public int? Id { get; set; }

        /// <summary>
        /// Obtém ou define o identificador do cliente.
        /// </summary>
        public int? IdCliente { get; set; }

        /// <summary>
        /// Obtém ou define o nome do cliente.
        /// </summary>
        public string NomeCliente { get; set; }

        /// <summary>
        /// Obtém ou define a data inicial de entrega do pedido.
        /// </summary>
        public DateTime? PeriodoEntregaPedidoInicio { get; set; }

        /// <summary>
        /// Obtém ou define a data final de entrega do pedido.
        /// </summary>
        public DateTime? PeriodoEntregaPedidoFim { get; set; }

        /// <summary>
        /// Obtém ou define a data inicial de liberação do pedido.
        /// </summary>
        public DateTime? PeriodoLiberacaoPedidoInicio { get; set; }

        /// <summary>
        /// Obtém ou define a data final de liberação do pedido.
        /// </summary>
        public DateTime? PeriodoLiberacaoPedidoFim { get; set; }

        /// <summary>
        /// Obtém ou define o identificador da loja.
        /// </summary>
        public int? IdLoja { get; set; }

        /// <summary>
        /// Obtém ou define o identificador da rota.
        /// </summary>
        public int? IdRota { get; set; }

        /// <summary>
        /// Obtém ou define a situação do pedido em relação ao volume.
        /// </summary>
        public IEnumerable<Data.Model.Pedido.SituacaoVolumeEnum> SituacoesPedidoVolume { get; set; }

        /// <summary>
        /// Obtém ou define o tipo de entrega do pedido.
        /// </summary>
        public Data.Model.Pedido.TipoEntregaPedido? TipoEntrega { get; set; }

        /// <summary>
        /// Obtém ou define o identificador do cliente externo.
        /// </summary>
        public int? IdClienteExterno { get; set; }

        /// <summary>
        /// Obtém ou define o nome do cliente externo.
        /// </summary>
        public string NomeClienteExterno { get; set; }

        /// <summary>
        /// Obtém ou define identificadores de rota externa.
        /// </summary>
        public IEnumerable<string> IdsRotaExterna { get; set; }
    }
}
