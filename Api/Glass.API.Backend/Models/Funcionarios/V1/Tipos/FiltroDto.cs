// <copyright file="FiltroDto.cs" company="Sync Softwares">
// Copyright (c) Sync Softwares. Todos os direitos reservados.
// </copyright>

using Glass.API.Backend.Helper.Funcionarios.Tipos;
using Glass.API.Backend.Models.Genericas.V1;

namespace Glass.API.Backend.Models.Funcionarios.V1.Tipos
{
    /// <summary>
    /// Classe que encapsula os itens de filtro para a lista de tipos de funcionário.
    /// </summary>
    public class FiltroDto : PaginacaoDto
    {
        /// <summary>
        /// Inicia uma nova instância da classe <see cref="FiltroDto"/>.
        /// </summary>
        public FiltroDto()
            : base(item => new TraducaoOrdenacaoListaTiposFuncionario(item.Ordenacao))
        {
        }
    }
}