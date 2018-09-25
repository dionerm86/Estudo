// <copyright file="FiltroDto.cs" company="Sync Softwares">
// Copyright (c) Sync Softwares. Todos os direitos reservados.
// </copyright>

using Glass.API.Backend.Helper.Produtos.CoresAluminio;
using Glass.API.Backend.Models.Genericas;

namespace Glass.API.Backend.Models.Produtos.CoresAluminio.Lista
{
    /// <summary>
    /// Classe que encapsula os itens de filtro para a lista de cores de alumínio.
    /// </summary>
    public class FiltroDto : PaginacaoDto
    {
        /// <summary>
        /// Inicia uma nova instância da classe <see cref="FiltroDto"/>.
        /// </summary>
        public FiltroDto()
            : base(item => new TraducaoOrdenacaoListaCoresAluminio(item.Ordenacao))
        {
        }
    }
}
