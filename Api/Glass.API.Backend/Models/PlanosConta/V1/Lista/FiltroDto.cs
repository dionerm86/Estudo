// <copyright file="FiltroDto.cs" company="Sync Softwares">
// Copyright (c) Sync Softwares. Todos os direitos reservados.
// </copyright>

using Glass.API.Backend.Helper.PlanosConta;
using Glass.API.Backend.Models.Genericas.V1;

namespace Glass.API.Backend.Models.PlanosConta.V1.Lista
{
    /// <summary>
    /// Classe que encapsula os itens de filtro para a lista de turnos.
    /// </summary>
    public class FiltroDto : PaginacaoDto
    {
        /// <summary>
        /// Inicia uma nova instância da classe <see cref="FiltroDto"/>.
        /// </summary>
        public FiltroDto()
            : base(item => new TraducaoOrdenacaoListaPlanosConta(item.Ordenacao))
        {
        }

        /// <summary>
        /// Obtém ou define o identificador do grupo de conta.
        /// </summary>
        public int? IdGrupoConta { get; set; }

        /// <summary>
        /// Obtém ou define o identificador da situação.
        /// </summary>
        public Situacao? Situacao { get; set; }
    }
}
