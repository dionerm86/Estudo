// <copyright file="FiltroDto.cs" company="Sync Softwares">
// Copyright (c) Sync Softwares. Todos os direitos reservados.
// </copyright>

using Glass.API.Backend.Helper.Seguradoras;
using Glass.API.Backend.Models.Genericas.V1;

namespace Glass.API.Backend.Models.Seguradoras.V1.Lista
{
    /// <summary>
    /// Classe que encapsula os itens de filtro para a lista de seguradoras.
    /// </summary>
    public class FiltroDto : PaginacaoDto
    {
        /// <summary>
        /// Inicia uma nova instância da classe <see cref="FiltroDto"/>.
        /// </summary>
        public FiltroDto()
            : base(item => new TraducaoOrdenacaoListaSeguradoras(item.Ordenacao))
        {
        }
    }
}
