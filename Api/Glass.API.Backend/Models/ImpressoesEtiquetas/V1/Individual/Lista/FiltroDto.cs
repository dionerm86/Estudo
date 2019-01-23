// <copyright file="FiltroDto.cs" company="Sync Softwares">
// Copyright (c) Sync Softwares. Todos os direitos reservados.
// </copyright>

using Glass.API.Backend.Helper.ImpressoesEtiquetas.Individual;
using Glass.API.Backend.Models.Genericas.V1;
using Newtonsoft.Json;
using System;
using System.Runtime.Serialization;

namespace Glass.API.Backend.Models.ImpressoesEtiquetas.V1.Individual.Lista
{
    /// <summary>
    /// Classe com os itens para o filtro de lista de impressões de individuais de etiquetas.
    /// </summary>
    [DataContract(Name = "Filtro")]
    public class FiltroDto : PaginacaoDto
    {
        /// <summary>
        /// Inicia uma nova instância da classe <see cref="FiltroDto"/>.
        /// </summary>
        public FiltroDto()
            : base(item => new TraducaoOrdenacaoListaImpressoesIndividuaisEtiquetas(item.Ordenacao))
        {
        }

        /// <summary>
        /// Obtém ou define o identificador do pedido.
        /// </summary>
        [JsonProperty("idPedido")]
        public int? IdPedido { get; set; }

        /// <summary>
        /// Obtém ou define o número da nota fiscal.
        /// </summary>
        [JsonProperty("numeroNotaFiscal")]
        public int? NumeroNotaFiscal { get; set; }

        /// <summary>
        /// Obtém ou define o código da etiqueta.
        /// </summary>
        [JsonProperty("codigoEtiqueta")]
        public string CodigoEtiqueta { get; set; }

        /// <summary>
        /// Obtém ou define a descrição do produto.
        /// </summary>
        [JsonProperty("descricaoProduto")]
        public string DescricaoProduto { get; set; }

        /// <summary>
        /// Obtém ou define o início da faixa de altura para o filtro.
        /// </summary>
        [JsonProperty("alturaInicio")]
        public int? AlturaInicio { get; set; }

        /// <summary>
        /// Obtém ou define o fim da faixa de altura para o filtro.
        /// </summary>
        [JsonProperty("alturaFim")]
        public int? AlturaFim { get; set; }

        /// <summary>
        /// Obtém ou define início da faixa de largura para o filtro.
        /// </summary>
        [JsonProperty("larguraInicio")]
        public int? LarguraInicio { get; set; }

        /// <summary>
        /// Obtém ou define o fim da faixa de largura para o filtro.
        /// </summary>
        [JsonProperty("larguraFim")]
        public int? LarguraFim { get; set; }

        /// <summary>
        /// Obtém ou define o código do processo.
        /// </summary>
        [JsonProperty("codigoProcesso")]
        public string CodigoProcesso { get; set; }

        /// <summary>
        /// Obtém ou define o código da aplicação.
        /// </summary>
        [JsonProperty("codigoAplicacao")]
        public string CodigoAplicacao { get; set; }
    }
}