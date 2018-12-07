// <copyright file="FiltroDto.cs" company="Sync Softwares">
// Copyright (c) Sync Softwares. Todos os direitos reservados.
// </copyright>

using Glass.API.Backend.Helper.Carregamentos.Itens.Pendencias;
using Glass.API.Backend.Models.Genericas.V1;
using System;
using System.Collections.Generic;

namespace Glass.API.Backend.Models.Carregamentos.V1.Itens.Pendencias
{
    /// <summary>
    /// Classe com os itens para o filtro da lista de carregamentos pendentes.
    /// </summary>
    public class FiltroDto : PaginacaoDto
    {
        /// <summary>
        /// Inicia uma nova instância da classe <see cref="FiltroDto"/>.
        /// </summary>
        public FiltroDto()
            : base(item => new TraducaoOrdenacaoListaPendenciasCarregamentos(item.Ordenacao))
        {
        }

        /// <summary>
        /// Obtém ou define o identificador do carregamento pendente.
        /// </summary>
        public int? IdCarregamento { get; set; }

        /// <summary>
        /// Obtém ou define o identificador do cliente.
        /// </summary>
        public int? IdCliente { get; set; }

        /// <summary>
        /// Obtém ou define o nome do cliente.
        /// </summary>
        public string NomeCliente { get; set; }

        /// <summary>
        /// Obtém ou define o identificador da loja.
        /// </summary>
        public int? IdLoja { get; set; }

        /// <summary>
        /// Obtém ou define a data inicial do período previsto de saída.
        /// </summary>
        public DateTime? PeriodoPrevisaoSaidaInicio { get; set; }

        /// <summary>
        /// Obtém ou define a data final do período previsto de saída.
        /// </summary>
        public DateTime? PeriodoPrevisaoSaidaFim { get; set; }

        /// <summary>
        /// Obtém ou define a lista de rotas associadas ao carregamento pendente.
        /// </summary>
        public IEnumerable<int> IdsRota { get; set; }

        /// <summary>
        /// Obtém ou define um valor que indica se serão buscados pedidos de transferencia.
        /// </summary>
        public bool? IgnorarPedidosVendaTransferencia { get; set; }

        /// <summary>
        /// Obtém ou define o identificador do cliente associado a um pedido importado.
        /// </summary>
        public int? IdClienteExterno { get; set; }

        /// <summary>
        /// Obtém ou define o nome do cliente associado a um pedido importado.
        /// </summary>
        public string NomeClienteExterno { get; set; }

        /// <summary>
        /// Obtém ou define a lista de rotas associadas a pedidos importados.
        /// </summary>
        public IEnumerable<int> IdsRotaExterna { get; set; }
    }
}
