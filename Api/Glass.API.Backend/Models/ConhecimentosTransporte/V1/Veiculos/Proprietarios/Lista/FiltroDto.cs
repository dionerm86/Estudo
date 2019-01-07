// <copyright file="FiltroDto.cs" company="Sync Softwares">
// Copyright (c) Sync Softwares. Todos os direitos reservados.
// </copyright>

using Glass.API.Backend.Helper.ConhecimentosTrasporte.Veiculos.Proprietarios;
using Glass.API.Backend.Models.Genericas.V1;

namespace Glass.API.Backend.Models.ConhecimentosTransporte.V1.Veiculos.Proprietarios.Lista
{
    /// <summary>
    /// Classe que encapsula os itens de filtro para a lista de proprietários de veículos.
    /// </summary>
    public class FiltroDto : PaginacaoDto
    {
        /// <summary>
        /// Inicia uma nova instância da classe <see cref="FiltroDto"/>.
        /// </summary>
        public FiltroDto()
            : base(item => new TraducaoOrdenacaoListaProprietariosDeVeiculos(item.Ordenacao))
        {
        }
    }
}