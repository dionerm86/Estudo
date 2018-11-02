// <copyright file="FiltroDto.cs" company="Sync Softwares">
// Copyright (c) Sync Softwares. Todos os direitos reservados.
// </copyright>

using Glass.API.Backend.Helper.Projetos.GruposProjeto;
using Glass.API.Backend.Models.Genericas.V1;

namespace Glass.API.Backend.Models.Projetos.V1.GruposProjeto.Lista
{
    /// <summary>
    /// Classe que encapsula os itens de filtro para a lista de grupos de projeto.
    /// </summary>
    public class FiltroDto : PaginacaoDto
    {
        /// <summary>
        /// Inicia uma nova instância da classe <see cref="FiltroDto"/>.
        /// </summary>
        public FiltroDto()
            : base(item => new TraducaoOrdenacaoListaGruposProjeto(item.Ordenacao))
        {
        }
    }
}
