// <copyright file="FiltroDto.cs" company="Sync Softwares">
// Copyright (c) Sync Softwares. Todos os direitos reservados.
// </copyright>

using Glass.API.Backend.Helper.Producao.Roteiros;
using Glass.API.Backend.Models.Genericas.V1;

namespace Glass.API.Backend.Models.Producao.V1.Roteiros.Lista
{
    /// <summary>
    /// Classe que encapsula os itens de filtro para a lista de roteiros.
    /// </summary>
    public class FiltroDto : PaginacaoDto
    {
        /// <summary>
        /// Inicia uma nova instância da classe <see cref="FiltroDto"/>.
        /// </summary>
        public FiltroDto()
            : base(item => new TraducaoOrdenacaoListaRoteiros(item.Ordenacao))
        {
        }

        /// <summary>
        /// Obtém ou define o identificador do grupo de produto.
        /// </summary>
        public int? IdGrupoProduto { get; set; }

        /// <summary>
        /// Obtém ou define o identificador do subgrupo de produto.
        /// </summary>
        public int? IdSubgrupoProduto { get; set; }

        /// <summary>
        /// Obtém ou define o identificador do processo.
        /// </summary>
        public int? IdProcesso { get; set; }
    }
}
