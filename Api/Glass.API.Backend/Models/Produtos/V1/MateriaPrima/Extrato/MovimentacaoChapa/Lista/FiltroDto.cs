// <copyright file="FiltroDto.cs" company="Sync Softwares">
// Copyright (c) Sync Softwares. Todos os direitos reservados.
// </copyright>

using Glass.API.Backend.Helper.Produtos.MateriaPrima.Extrato.MovimentacaoChapa;
using Glass.API.Backend.Models.Genericas.V1;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;

namespace Glass.API.Backend.Models.Produtos.V1.MateriaPrima.Extrato.MovimentacaoChapa.Lista
{
    /// <summary>
    /// Classe com os itens para o filtro da lista extrato de movimentações de chapa.
    /// </summary>
    public class FiltroDto : PaginacaoDto
    {
        /// <summary>
        /// Inicia uma nova instância da classe <see cref="FiltroDto"/>.
        /// </summary>
        public FiltroDto()
            : base(item => new TraducaoOrdenacaoExtratoMovimentacoesChapa(item.Ordenacao))
        {
        }

        /// <summary>
        /// Obtém ou define os identificadores das cores de vidro.
        /// </summary>
        [JsonProperty("idsCorVidro")]
        public IEnumerable<int> IdsCorVidro { get; set; }

        /// <summary>
        /// Obtém ou define a espessura do vidro.
        /// </summary>
        [JsonProperty("espessura")]
        public decimal? Espessura { get; set; }

        /// <summary>
        /// Obtém ou define a altura do vidro.
        /// </summary>
        [JsonProperty("altura")]
        public int? Altura { get; set; }

        /// <summary>
        /// Obtém ou define a largura do vidro.
        /// </summary>
        [JsonProperty("largura")]
        public int? Largura { get; set; }

        /// <summary>
        /// Obtém ou define a faixa inicial no período de movimentação.
        /// </summary>
        [JsonProperty("periodoMovimentacaoInicio")]
        public DateTime? PeriodoMovimentacaoInicio { get; set; }

        /// <summary>
        /// Obtém ou define a faixa final no período de movimentação.
        /// </summary>
        [JsonProperty("periodoMovimentacaoFim")]
        public DateTime? PeriodoMovimentacaoFim { get; set; }
    }
}