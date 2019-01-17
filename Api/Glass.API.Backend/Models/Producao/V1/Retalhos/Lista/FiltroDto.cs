// <copyright file="FiltroDto.cs" company="Sync Softwares">
// Copyright (c) Sync Softwares. Todos os direitos reservados.
// </copyright>

using Glass.API.Backend.Helper.Producao.Retalhos;
using Glass.API.Backend.Models.Genericas.V1;
using Glass.Data.Model;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;

namespace Glass.API.Backend.Models.Producao.V1.Retalhos.Lista
{
    /// <summary>
    /// Classe que encapsula os itens de filtro para a lista de retalhos de produção.
    /// </summary>
    public class FiltroDto : PaginacaoDto
    {
        /// <summary>
        /// Inicia uma nova instância da classe <see cref="FiltroDto"/>.
        /// </summary>
        public FiltroDto()
            : base(item => new TraducaoOrdenacaoListaRetalhosProducao(item.Ordenacao))
        {
        }

        /// <summary>
        /// Obtém ou define o código do produto.
        /// </summary>
        [JsonProperty("codigoProduto")]
        public string CodigoProduto { get; set; }

        /// <summary>
        /// Obtém ou define a descrição do produto.
        /// </summary>
        [JsonProperty("descricaoProduto")]
        public string DescricaoProduto { get; set; }

        /// <summary>
        /// Obtém ou define o inicio do período de cadastro.
        /// </summary>
        [JsonProperty("periodoCadastroInicio")]
        public DateTime? PeriodoCadastroInicio { get; set; }

        /// <summary>
        /// Obtém ou define o fim do período de cadastro.
        /// </summary>
        [JsonProperty("periodoCadastroFim")]
        public DateTime? PeriodoCadastroFim { get; set; }

        /// <summary>
        /// Obtém ou define o inicio do período de uso.
        /// </summary>
        [JsonProperty("periodoUsoInicio")]
        public DateTime? PeriodoUsoInicio { get; set; }

        /// <summary>
        /// Obtém ou define o fim do período de uso.
        /// </summary>
        [JsonProperty("periodoUsoFim")]
        public DateTime? PeriodoUsoFim { get; set; }

        /// <summary>
        /// Obtém ou define a situação do retalho.
        /// </summary>
        [JsonProperty("situacao")]
        public SituacaoRetalhoProducao Situacao { get; set; }

        /// <summary>
        /// Obtém ou define os identificadores de cor dos vidros.
        /// </summary>
        [JsonProperty("idsCorVidro")]
        public IEnumerable<int> IdsCorVidro { get; set; }

        /// <summary>
        /// Obtém ou define a espessura do retalho.
        /// </summary>
        [JsonProperty("espessura")]
        public decimal? Espessura { get; set; }

        /// <summary>
        /// Obtém ou define a faixa inicial de altura.
        /// </summary>
        [JsonProperty("alturaInicio")]
        public decimal? AlturaInicio { get; set; }

        /// <summary>
        /// Obtém ou define a faixa final de altura.
        /// </summary>
        [JsonProperty("alturaFim")]
        public decimal? AlturaFim { get; set; }

        /// <summary>
        /// Obtém ou define a faixa inicial de largura.
        /// </summary>
        [JsonProperty("larguraInicio")]
        public decimal? LarguraInicio { get; set; }

        /// <summary>
        /// Obtém ou define a faixa final de largura.
        /// </summary>
        [JsonProperty("larguraFim")]
        public decimal? LarguraFim { get; set; }

        /// <summary>
        /// Obtém ou define o código da etiqueta do retalho.
        /// </summary>
        [JsonProperty("codigoEtiqueta")]
        public string CodigoEtiqueta { get; set; }

        /// <summary>
        /// Obtém ou define a observação referente ao retalho.
        /// </summary>
        [JsonProperty("observacao")]
        public string Observacao { get; set; }
    }
}