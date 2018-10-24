// <copyright file="FiltroDto.cs" company="Sync Softwares">
// Copyright (c) Sync Softwares. Todos os direitos reservados.
// </copyright>

using Glass.API.Backend.Helper.Condutores;
using Glass.API.Backend.Models.Genericas;
using Glass.API.Backend.Models.Genericas.V1;

namespace Glass.API.Backend.Models.Condutores.Lista
{
    /// <summary>
    /// Classe que encapsula os itens de filtro para a lista de condutores.
    /// </summary>
    public class FiltroDto : PaginacaoDto
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="FiltroDto"/> class.
        /// </summary>
        public FiltroDto()
            : base(item => new TraducaoOrdenacaoListaCondutores(item.Ordenacao))
        {
        }
    }
}
