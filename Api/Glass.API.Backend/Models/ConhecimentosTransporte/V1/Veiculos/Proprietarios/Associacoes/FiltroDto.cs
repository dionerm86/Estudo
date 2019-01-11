// <copyright file="FiltroDto.cs" company="Sync Softwares">
// Copyright (c) Sync Softwares. Todos os direitos reservados.
// </copyright>

using Glass.API.Backend.Helper.ConhecimentosTrasporte.Veiculos.Proprietarios.Associacoes;
using Glass.API.Backend.Models.Genericas.V1;

namespace Glass.API.Backend.Models.ConhecimentosTransporte.V1.Veiculos.Proprietarios.Associacoes
{
    /// <summary>
    /// Classe que encapsula os itens de filtro para a lista de associação de proprietários com veículos.
    /// </summary>
    public class FiltroDto : PaginacaoDto
    {
        /// <summary>
        /// Inicia uma nova instância da classe <see cref="FiltroDto"/>.
        /// </summary>
        public FiltroDto()
            : base(item => new TraducaoOrdenacaoListaAssociacaoProprietariosVeiculos(item.Ordenacao))
        {
        }
    }
}