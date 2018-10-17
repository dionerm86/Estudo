// <copyright file="FiltroDto.cs" company="Sync Softwares">
// Copyright (c) Sync Softwares. Todos os direitos reservados.
// </copyright>

using Glass.API.Backend.Helper.NotasFiscais;
using Glass.API.Backend.Models.Genericas.V1;
using Newtonsoft.Json;
using System;

namespace Glass.API.Backend.Models.NotasFiscais.V1.Lista
{
    /// <summary>
    /// Classe com os itens para o filtro de lista de clientes.
    /// </summary>
    public class FiltroDto : PaginacaoDto
    {
        /// <summary>
        /// Inicia uma nova instância da classe <see cref="FiltroDto"/>.
        /// </summary>
        public FiltroDto()
            : base(item => new TraducaoOrdenacaoListaNotasFiscais(item.Ordenacao))
        {
        }

        /// <summary>
        /// Obtém ou define o número da nota fiscal.
        /// </summary>
        [JsonProperty("numeroNfe")]
        public int? NumeroNfe { get; set; }

        /// <summary>
        /// Obtém ou define o identificador do pedido.
        /// </summary>
        [JsonProperty("idPedido")]
        public int? IdPedido { get; set; }

        /// <summary>
        /// Obtém ou define o modelo da nota fiscal.
        /// </summary>
        [JsonProperty("modelo")]
        public string Modelo { get; set; }

        /// <summary>
        /// Obtém ou define o identificador da loja.
        /// </summary>
        [JsonProperty("idLoja")]
        public int? IdLoja { get; set; }

        /// <summary>
        /// Obtém ou define o identificador do cliente da nota fiscal.
        /// </summary>
        [JsonProperty("idCliente")]
        public int? IdCliente { get; set; }

        /// <summary>
        /// Obtém ou define o nome do cliente da nota fiscal.
        /// </summary>
        [JsonProperty("nomeCliente")]
        public string NomeCliente { get; set; }

        /// <summary>
        /// Obtém ou define o tipo fiscal do cliente da nota fiscal.
        /// </summary>
        [JsonProperty("tipoFiscal")]
        public Data.Model.TipoFiscalCliente? TipoFiscal { get; set; }

        /// <summary>
        /// Obtém ou define o identificador do fornecedor da nota fiscal.
        /// </summary>
        [JsonProperty("idFornecedor")]
        public int? IdFornecedor { get; set; }

        /// <summary>
        /// Obtém ou define o nome do fornecedor da nota fiscal.
        /// </summary>
        [JsonProperty("nomeFornecedor")]
        public string NomeFornecedor { get; set; }

        /// <summary>
        /// Obtém ou define o código da rota da nota fiscal.
        /// </summary>
        [JsonProperty("codigoRota")]
        public string CodigoRota { get; set; }

        /// <summary>
        /// Obtém ou define a situação da nota fiscal.
        /// </summary>
        [JsonProperty("situacao")]
        public Data.Model.NotaFiscal.SituacaoEnum? Situacao { get; set; }

        /// <summary>
        /// Obtém ou define a data de início da emissão da nota fiscal.
        /// </summary>
        [JsonProperty("periodoEmissaoInicio")]
        public DateTime? PeriodoEmissaoInicio { get; set; }

        /// <summary>
        /// Obtém ou define a data final da emissão da nota fiscal.
        /// </summary>
        [JsonProperty("periodoEmissaoFim")]
        public DateTime? PeriodoEmissaoFim { get; set; }

        /// <summary>
        /// Obtém ou define o identificador dos CFOPs da nota fiscal.
        /// </summary>
        [JsonProperty("idsCfop")]
        public int[] IdsCfop { get; set; }

        /// <summary>
        /// Obtém ou define os tipos de CFOP do CFOP da nota fiscal.
        /// </summary>
        [JsonProperty("tiposCfop")]
        public int[] TiposCfop { get; set; }

        /// <summary>
        /// Obtém ou define a data de início da entrada/saída da nota fiscal.
        /// </summary>
        [JsonProperty("periodoEntradaSaidaInicio")]
        public DateTime? PeriodoEntradaSaidaInicio { get; set; }

        /// <summary>
        /// Obtém ou define a data final da entrada/saída da nota fiscal.
        /// </summary>
        [JsonProperty("periodoEntradaSaidaFim")]
        public DateTime? PeriodoEntradaSaidaFim { get; set; }

        /// <summary>
        /// Obtém ou define o tipo de venda da nota fiscal.
        /// </summary>
        [JsonProperty("tipoVenda")]
        public int? TipoVenda { get; set; }

        /// <summary>
        /// Obtém ou define as formas de pagamento da nota fiscal.
        /// </summary>
        [JsonProperty("idsFormaPagamento")]
        public int[] IdsFormaPagamento { get; set; }

        /// <summary>
        /// Obtém ou define o tipo da nota fiscal.
        /// </summary>
        [JsonProperty("tipoDocumento")]
        public Data.Model.NotaFiscal.TipoDoc? TipoDocumento { get; set; }

        /// <summary>
        /// Obtém ou define a finalidade da nota fiscal.
        /// </summary>
        [JsonProperty("finalidade")]
        public Data.Model.NotaFiscal.FinalidadeEmissaoEnum? Finalidade { get; set; }

        /// <summary>
        /// Obtém ou define o tipo de emissão da nota fiscal.
        /// </summary>
        [JsonProperty("formaEmissao")]
        public Data.Model.NotaFiscal.TipoEmissao? TipoEmissao { get; set; }

        /// <summary>
        /// Obtém ou define a informação complementar da nota fiscal.
        /// </summary>
        [JsonProperty("informacaoComplementar")]
        public string InformacaoComplementar { get; set; }

        /// <summary>
        /// Obtém ou define o código interno do produto da nota fiscal.
        /// </summary>
        [JsonProperty("codigoInternoProduto")]
        public string CodigoInternoProduto { get; set; }

        /// <summary>
        /// Obtém ou define a descrição do produto da nota fiscal.
        /// </summary>
        [JsonProperty("descricaoProduto")]
        public string DescricaoProduto { get; set; }

        /// <summary>
        /// Obtém ou define o lote do produto da nota fiscal.
        /// </summary>
        [JsonProperty("lote")]
        public string Lote { get; set; }

        /// <summary>
        /// Obtém ou define o valor inicial para filtro do total da nota fiscal.
        /// </summary>
        [JsonProperty("valorNotaFiscalInicio")]
        public decimal? ValorNotaFiscalInicio { get; set; }

        /// <summary>
        /// Obtém ou define o valor final para filtro do total da nota fiscal.
        /// </summary>
        [JsonProperty("valorNotaFiscalFim")]
        public decimal? ValorNotaFiscalFim { get; set; }

        /// <summary>
        /// Obtém ou define a ordenação a ser usada na listagem.
        /// </summary>
        [JsonProperty("ordenacaoFiltro")]
        public int OrdenacaoFiltro { get; set; }
    }
}
