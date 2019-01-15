// <copyright file="FiltroDto.cs" company="Sync Softwares">
// Copyright (c) Sync Softwares. Todos os direitos reservados.
// </copyright>

using Glass.API.Backend.Helper.Producao.TiposPerda;
using Glass.API.Backend.Models.Genericas.V1;

namespace Glass.API.Backend.Models.Producao.V1.TiposPerda.Lista
{
    /// <summary>
    /// Classe que encapsula os itens de filtro para a lista de tipos de perda.
    /// </summary>
    public class FiltroDto : PaginacaoDto
    {
        /// <summary>
        /// Inicia uma nova instância da classe <see cref="FiltroDto"/>.
        /// </summary>
        public FiltroDto()
            : base(item => new TraducaoOrdenacaoListaTiposPerda(item.Ordenacao))
        {
        }
    }
}
