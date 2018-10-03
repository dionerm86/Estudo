// <copyright file="FiltroDto.cs" company="Sync Softwares">
// Copyright (c) Sync Softwares. Todos os direitos reservados.
// </copyright>

using Glass.API.Backend.Helper.Produtos.CoresVidro;
using Glass.API.Backend.Models.Genericas;

namespace Glass.API.Backend.Models.Clientes.Tipos.Lista
{
    /// <summary>
    /// Classe que encapsula os itens de filtro para a lista de tipos de cliente.
    /// </summary>
    public class FiltroDto : PaginacaoDto
    {
        /// <summary>
        /// Inicia uma nova instância da classe <see cref="FiltroDto"/>.
        /// </summary>
        public FiltroDto()
            : base(item => new TraducaoOrdenacaoListaTipos(item.Ordenacao))
        {
        }
    }
}
