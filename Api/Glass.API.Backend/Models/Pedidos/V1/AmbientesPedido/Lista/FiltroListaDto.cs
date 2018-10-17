// <copyright file="FiltroListaDto.cs" company="Sync Softwares">
// Copyright (c) Sync Softwares. Todos os direitos reservados.
// </copyright>

using Glass.API.Backend.Helper.Pedidos.AmbientesPedido;
using Glass.API.Backend.Models.Genericas.V1;

namespace Glass.API.Backend.Models.Pedidos.V1.AmbientesPedido.Lista
{
    /// <summary>
    /// Classe que contém os parâmetros de entrada para a busca de ambientes de pedido.
    /// </summary>
    public class FiltroListaDto : PaginacaoDto
    {
        /// <summary>
        /// Inicia uma nova instância da classe <see cref="FiltroListaDto"/>.
        /// </summary>
        public FiltroListaDto()
            : base(item => new TraducaoOrdenacaoListaAmbientesPedido(item.Ordenacao))
        {
        }
    }
}
