// <copyright file="FiltroDto.cs" company="Sync Softwares">
// Copyright (c) Sync Softwares. Todos os direitos reservados.
// </copyright>

using Glass.API.Backend.Helper.Parcelas;
using Glass.API.Backend.Models.Genericas.V1;

namespace Glass.API.Backend.Models.Parcelas.V1.Lista
{
    public class FiltroDto : PaginacaoDto
    {
        /// <summary>
        /// Inicia uma nova instância da classe <see cref="FiltroDto"/>.
        /// </summary>
        public FiltroDto()
            : base(item => new TraducaoOrdenacaoListaParcelas(item.Ordenacao))
        {
        }
    }
}
