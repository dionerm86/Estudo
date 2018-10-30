// <copyright file="FiltroDto.cs" company="Sync Softwares">
// Copyright (c) Sync Softwares. Todos os direitos reservados.
// </copyright>

using Glass.API.Backend.Helper.Veiculos;
using Glass.API.Backend.Models.Genericas.V1;
using System.Runtime.Serialization;

namespace Glass.API.Backend.Models.Veiculos.V1.Lista
{
    /// <summary>
    /// Classe com os itens para o filtro de lista de veículos.
    /// </summary>
    [DataContract(Name = "Filtro")]
    public class FiltroDto : PaginacaoDto
    {
        /// <summary>
        /// Inicia uma nova instância da classe <see cref="FiltroDto"/>.
        /// </summary>
        public FiltroDto()
            : base(item => new TraducaoOrdenacaoListaVeiculos(item.Ordenacao))
        {
        }
    }
}
