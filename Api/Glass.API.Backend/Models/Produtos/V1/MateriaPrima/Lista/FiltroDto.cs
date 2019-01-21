// <copyright file="FiltroDto.cs" company="Sync Softwares">
// Copyright (c) Sync Softwares. Todos os direitos reservados.
// </copyright>

using Glass.API.Backend.Helper.Produtos.MateriaPrima.Posicao.ChapaMateriaPrima;
using Glass.API.Backend.Models.Genericas.V1;
using Newtonsoft.Json;

namespace Glass.API.Backend.Models.Produtos.V1.MateriaPrima.Lista
{
    /// <summary>
    /// Classe com os itens para o filtro da lista de chapas de matéria prima.
    /// </summary>
    public class FiltroDto : PaginacaoDto
	{
        /// <summary>
        /// Inicia uma nova instância da classe <see cref="FiltroDto"/>.
        /// </summary>
        public FiltroDto()
            : base(item => new TraducaoOrdenacaoListaChapasMateriaPrima(item.Ordenacao))
        {
        }

        /// <summary>
        /// Obtém ou define o identificador da cor do vidro associado a posição de matéria prima.
        /// </summary>
        [JsonProperty("idCorVidro")]
        public int? IdCorVidro { get; set; }

        /// <summary>
        /// Obtém ou define a espessura do vidro associado a posição de matéria prima.
        /// </summary>
        [JsonProperty("espessura")]
        public decimal? Espessura { get; set; }
    }
}