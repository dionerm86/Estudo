// <copyright file="FiltroLogAlteracaoDto.cs" company="Sync Softwares">
// Copyright (c) Sync Softwares. Todos os direitos reservados.
// </copyright>

using Glass.API.Backend.Models.Genericas.V1;
using Glass.Data.Model;

namespace Glass.API.Backend.Models.Log.V1.Alteracao
{
    /// <summary>
    /// Classe com os dados do filtro para a lista de logs de alteração.
    /// </summary>
    public class FiltroLogAlteracaoDto : PaginacaoDto
    {
        /// <summary>
        /// Inicia uma nova instância da classe <see cref="FiltroLogAlteracaoDto"/>.
        /// </summary>
        public FiltroLogAlteracaoDto()
            : base(item => null)
        {
        }

        /// <summary>
        /// Obtém ou define a tabela que contém o item.
        /// </summary>
        public LogAlteracao.TabelaAlteracao? Tabela { get; set; }

        /// <summary>
        /// Obtém ou define o identificador do item no qual o log foi feito.
        /// </summary>
        public int IdItem { get; set; }

        /// <summary>
        /// Obtém ou define o nome do campo no qual o log foi feito.
        /// </summary>
        public string Campo { get; set; }
    }
}
