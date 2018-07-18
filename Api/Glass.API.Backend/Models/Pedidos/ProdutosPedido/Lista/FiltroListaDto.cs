// <copyright file="FiltroListaDto.cs" company="Sync Softwares">
// Copyright (c) Sync Softwares. Todos os direitos reservados.
// </copyright>

using Glass.API.Backend.Helper.Pedidos.ProdutosPedido;
using Glass.API.Backend.Models.Genericas;

namespace Glass.API.Backend.Models.Pedidos.ProdutosPedido.Lista
{
    /// <summary>
    /// Classe que contém os parâmetros de entrada para a busca de produtos de pedido.
    /// </summary>
    public class FiltroListaDto : PaginacaoDto
    {
        /// <summary>
        /// Inicia uma nova instância da classe <see cref="FiltroListaDto"/>.
        /// </summary>
        public FiltroListaDto()
            : base(item => new TraducaoOrdenacaoListaProdutosPedido(item.Ordenacao))
        {
        }

        /// <summary>
        /// Obtém ou define o filtro pelo ambiente do pedido.
        /// </summary>
        public int? IdAmbiente { get; set; }

        /// <summary>
        /// Obtém ou define o identificador do produto pai (para produtos laminados/compostos).
        /// </summary>
        public int? IdProdutoPai { get; set; }
    }
}
