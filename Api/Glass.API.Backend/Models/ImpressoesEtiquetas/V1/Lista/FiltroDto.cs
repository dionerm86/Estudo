// <copyright file="FiltroDto.cs" company="Sync Softwares">
// Copyright (c) Sync Softwares. Todos os direitos reservados.
// </copyright>

using Glass.API.Backend.Helper.ImpressoesEtiquetas;
using Glass.API.Backend.Models.Genericas.V1;
using Newtonsoft.Json;
using System;
using System.Runtime.Serialization;

namespace Glass.API.Backend.Models.ImpressoesEtiquetas.V1.Lista
{
    /// <summary>
    /// Classe com os itens para o filtro de lista de impressões de etiquetas.
    /// </summary>
    [DataContract(Name = "Filtro")]
    public class FiltroDto : PaginacaoDto
    {
        /// <summary>
        /// Inicia uma nova instância da classe <see cref="FiltroDto"/>.
        /// </summary>
        public FiltroDto()
            : base(item => new TraducaoOrdenacaoListaImpressoesEtiquetas(item.Ordenacao))
        {
        }

        /// <summary>
        /// Obtém ou define o identificador da impressão de etiqueta.
        /// </summary>
        [JsonProperty("id")]
        public int? Id { get; set; }

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
        /// Obtém ou define o plano de corte da impressão de etiqueta.
        /// </summary>
        [JsonProperty("planoCorte")]
        public string PlanoCorte { get; set; }

        /// <summary>
        /// Obtém ou define o lote do produto da nota fiscal.
        /// </summary>
        [JsonProperty("loteProdutoNotaFiscal")]
        public string LoteProdutoNotaFiscal { get; set; }

        /// <summary>
        /// Obtém ou define o período inicial de cadastro da impressão de etiqueta.
        /// </summary>
        [JsonProperty("periodoCadastroInicio")]
        public DateTime? PeriodoCadastroInicio { get; set; }

        /// <summary>
        /// Obtém ou define o período final de cadastro da impressão de etiqueta.
        /// </summary>
        [JsonProperty("periodoCadastroFim")]
        public DateTime? PeriodoCadastroFim { get; set; }

        /// <summary>
        /// Obtém ou define o tipo da impressão de etiqueta.
        /// </summary>
        [JsonProperty("tipoImpressao")]
        public Data.DAL.ProdutoImpressaoDAO.TipoEtiqueta? TipoImpressao { get; set; }

        /// <summary>
        /// Obtém ou define o código da etiqueta.
        /// </summary>
        [JsonProperty("codigoEtiqueta")]
        public string CodigoEtiqueta { get; set; }
    }
}
