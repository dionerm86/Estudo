// <copyright file="FiltroDto.cs" company="Sync Softwares">
// Copyright (c) Sync Softwares. Todos os direitos reservados.
// </copyright>

using Glass.API.Backend.Helper.Producao;
using Glass.API.Backend.Models.Genericas;

namespace Glass.API.Backend.Models.Producao.V1.Composicao
{
    /// <summary>
    /// Classe que encapsula os filtros para a busca de produtos de composição.
    /// </summary>
    public class FiltroDto : PaginacaoDto
    {
        /// <summary>
        /// Inicia uma nova instância da classe <see cref="FiltroDto"/>.
        /// </summary>
        public FiltroDto()
            : base(item => new TraducaoOrdenacaoListaProducao(item.Ordenacao))
        {
        }
    }
}
