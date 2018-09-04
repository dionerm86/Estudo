// <copyright file="FiltroDto.cs" company="Sync Softwares">
// Copyright (c) Sync Softwares. Todos os direitos reservados.
// </copyright>

using Glass.API.Backend.Helper;
using Glass.API.Backend.Helper.Processos;
using Glass.API.Backend.Models.Genericas;
using System;

namespace Glass.API.Backend.Models.Processos.Lista
{
    /// <summary>
    /// Classe que encapsula os itens de filtro para a lista de processos de etiqueta.
    /// </summary>
    public class FiltroDto : PaginacaoDto
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="FiltroDto"/> class.
        /// </summary>
        /// <param name="tradutorOrdenacao">O tradutor de ordenação que será usado pelo objeto.</param>
        public FiltroDto(Func<PaginacaoDto, ITraducaoOrdenacao> tradutorOrdenacao)
            : base(item => new TraducaoOrdenacaoListaProcessos(item.Ordenacao))
        {
        }
    }
}
