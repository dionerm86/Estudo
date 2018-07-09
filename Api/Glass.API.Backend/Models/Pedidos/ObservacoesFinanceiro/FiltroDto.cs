// <copyright file="FiltroDto.cs" company="Sync Softwares">
// Copyright (c) Sync Softwares. Todos os direitos reservados.
// </copyright>

using Glass.API.Backend.Helper.Pedidos;
using Glass.API.Backend.Models.Genericas;

namespace Glass.API.Backend.Models.Pedidos.ObservacoesFinanceiro
{
    /// <summary>
    /// Classe que contém os dados de filtro de pedido.
    /// </summary>
    public class FiltroDto : PaginacaoDto
    {
        /// <summary>
        /// Inicia uma nova instância da classe <see cref="FiltroDto"/>.
        /// </summary>
        public FiltroDto()
            : base(item => new TraducaoOrdenacaoListaObservacoesFinanceiro(item.Ordenacao))
        {
        }

        /// <summary>
        /// Obtém ou define o identificador do pedido.
        /// </summary>
        public int IdPedido { get; set; }
    }
}
