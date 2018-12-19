// <copyright file="FiltroDto.cs" company="Sync Softwares">
// Copyright (c) Sync Softwares. Todos os direitos reservados.
// </copyright>

using Glass.API.Backend.Helper.Projetos.Medidas.Grupos;
using Glass.API.Backend.Models.Genericas.V1;

namespace Glass.API.Backend.Models.Projetos.V1.Medidas.Grupos.Lista
{
    /// <summary>
    /// Classe que encapsula os itens de filtro para a lista de grupos de medida de projetos.
    /// </summary>
    public class FiltroDto : PaginacaoDto
    {
        /// <summary>
        /// Inicia uma nova instância da classe <see cref="FiltroDto"/>.
        /// </summary>
        public FiltroDto()
            : base(item => new TraducaoOrdenacaoGrupoMedidaProjeto(item.Ordenacao))
        {
            this.NumeroRegistros = 15;
        }
    }
}