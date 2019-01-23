// <copyright file="FiltroDto.cs" company="Sync Softwares">
// Copyright (c) Sync Softwares. Todos os direitos reservados.
// </copyright>

using Glass.API.Backend.Helper.Producao.Fornadas;
using Glass.API.Backend.Models.Genericas.V1;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;

namespace Glass.API.Backend.Models.Producao.V1.Fornadas.Lista
{
    /// <summary>
    /// Classe que encapsula os dados dos filtros para a tela de listagem de fornadas.
    /// </summary>
    public class FiltroDto : PaginacaoDto
    {
        /// <summary>
        /// Inicia uma nova instância da classe <see cref="FiltroDto"/>.
        /// </summary>
        public FiltroDto()
            : base(item => new TraducaoOrdenacaoListaFornadas(item.Ordenacao))
        {
        }

        /// <summary>
        /// Obtém ou define o identificador da fornada.
        /// </summary>
        [JsonProperty("id")]
        public int? Id { get; set; }

        /// <summary>
        /// Obtém ou define o identificador do pedido da fornada.
        /// </summary>
        [JsonProperty("idPedido")]
        public int? IdPedido { get; set; }

        /// <summary>
        /// Obtém ou define o código da etiqueta associada a fornada.
        /// </summary>
        [JsonProperty("codigoEtiqueta")]
        public string CodigoEtiqueta { get; set; }

        /// <summary>
        /// Obtém ou define a faixa inicial no período de cadastro.
        /// </summary>
        [JsonProperty("periodoCadastroInicio")]
        public DateTime? PeriodoCadastroInicio { get; set; }

        /// <summary>
        /// Obtém ou define a faixa final no período de cadastro.
        /// </summary>
        [JsonProperty("periodoCadastroFim")]
        public DateTime? PeriodoCadastroFim { get; set; }

        /// <summary>
        /// Obtém ou define os identificadores das cores de vidro.
        /// </summary>
        [JsonProperty("idsCorVidro")]
        public IEnumerable<int> IdsCorVidro { get; set; }

        /// <summary>
        /// Obtém ou define a espessura das peças de fornada.
        /// </summary>
        [JsonProperty("espessura")]
        public int? Espessura { get; set; }
    }
}