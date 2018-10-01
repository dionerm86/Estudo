// <copyright file="FiltroDto.cs" company="Sync Softwares">
// Copyright (c) Sync Softwares. Todos os direitos reservados.
// </copyright>

using Glass.API.Backend.Helper.Producao;
using Glass.API.Backend.Models.Genericas;
using System;
using System.Collections.Generic;
using static Glass.Data.Model.Pedido;

namespace Glass.API.Backend.Models.Producao.V1.Lista
{
    /// <summary>
    /// Classe com os filtros para a tela de consulta de produção.
    /// </summary>
    public class FiltroDto : PaginacaoDto
    {
        /// <summary>
        /// Inicia uma nova instância da classe <see cref="FiltroDto"/>.
        /// </summary>
        public FiltroDto()
            : base(item => new TraducaoOrdenacaoListaProducao(item.Ordenacao))
        {
        }

        /// <summary>
        /// Obtém ou define o identificador do pedido.
        /// </summary>
        public int? IdPedido { get; set; }

        /// <summary>
        /// Obtém ou define o identificador da liberação de pedido.
        /// </summary>
        public int? IdLiberacaoPedido { get; set; }

        /// <summary>
        /// Obtém ou define o identificador do carregamento.
        /// </summary>
        public int? IdCarregamento { get; set; }

        /// <summary>
        /// Obtém ou define o identificador do pedido importado.
        /// </summary>
        public int? IdPedidoImportado { get; set; }

        /// <summary>
        /// Obtém ou define o código do pedido do cliente.
        /// </summary>
        public string CodigoPedidoCliente { get; set; }

        /// <summary>
        /// Obtém ou define os identificadores das rotas.
        /// </summary>
        public IEnumerable<int> IdsRotas { get; set; }

        /// <summary>
        /// Obtém ou define o identificador do cliente.
        /// </summary>
        public int? IdCliente { get; set; }

        /// <summary>
        /// Obtém ou define o nome do cliente.
        /// </summary>
        public string NomeCliente { get; set; }

        /// <summary>
        /// Obtém ou define o identificador da impressão.
        /// </summary>
        public int? IdImpressao { get; set; }

        /// <summary>
        /// Obtém ou define o número da etiqueta da peça.
        /// </summary>
        public string NumeroEtiquetaPeca { get; set; }

        /// <summary>
        /// Obtém ou define as situações de produção.
        /// </summary>
        public IEnumerable<SituacaoProducao> SituacoesProducao { get; set; }

        /// <summary>
        /// Obtém ou define o identificador do setor de produção.
        /// </summary>
        public int? IdSetor { get; set; }

        /// <summary>
        /// Obtém ou define a data de início para o período de setor.
        /// </summary>
        public DateTime? PeriodoSetorInicio { get; set; }

        /// <summary>
        /// Obtém ou define a data de término para o período de setor.
        /// </summary>
        public DateTime? PeriodoSetorFim { get; set; }

        /// <summary>
        /// Obtém ou define o identificador da loja.
        /// </summary>
        public int? IdLoja { get; set; }

        /// <summary>
        /// Obtém ou define o tipo de situação de produção.
        /// </summary>
        public TipoSituacaoProducao? TipoSituacaoProducao { get; set; }

        /// <summary>
        /// Obtém ou define a situação do pedido.
        /// </summary>
        public SituacaoPedido? SituacaoPedido { get; set; }

        /// <summary>
        /// Obtém ou define os identificadores dos subgrupos de vidro.
        /// </summary>
        public IEnumerable<int> IdsSubgrupos { get; set; }

        /// <summary>
        /// Obtém ou define os identificadores dos beneficiamentos.
        /// </summary>
        public IEnumerable<int> IdsBeneficiamentos { get; set; }

        /// <summary>
        /// Obtém ou define o identificador do funcionário vendedor do pedido.
        /// </summary>
        public int? IdVendedorPedido { get; set; }

        /// <summary>
        /// Obtém ou define o tipo de entrega do pedido.
        /// </summary>
        public TipoEntregaPedido? TipoEntregaPedido { get; set; }

        /// <summary>
        /// Obtém ou define a data de início para o período de entrega.
        /// </summary>
        public DateTime? PeriodoEntregaInicio { get; set; }

        /// <summary>
        /// Obtém ou define a data de término para o período de entrega.
        /// </summary>
        public DateTime? PeriodoEntregaFim { get; set; }

        /// <summary>
        /// Obtém ou define a largura da peça.
        /// </summary>
        public int? LarguraPeca { get; set; }

        /// <summary>
        /// Obtém ou define a altura da peça.
        /// </summary>
        public decimal? AlturaPeca { get; set; }

        /// <summary>
        /// Obtém ou define os tipos de pedidos.
        /// </summary>
        public IEnumerable<TipoPedidoEnum> TiposPedidos { get; set; }

        /// <summary>
        /// Obtém ou define os tipos de peças a serem exibidas.
        /// </summary>
        public IEnumerable<TipoPecaExibir> TiposPecasExibir { get; set; }

        /// <summary>
        /// Obtém ou define a data de início para o período de fábrica.
        /// </summary>
        public DateTime? PeriodoFabricaInicio { get; set; }

        /// <summary>
        /// Obtém ou define a data de término para o período de fábrica.
        /// </summary>
        public DateTime? PeriodoFabricaFim { get; set; }

        /// <summary>
        /// Obtém ou define a espessura da peça.
        /// </summary>
        public decimal? EspessuraPeca { get; set; }

        /// <summary>
        /// Obtém ou define o identificador da cor do vidro.
        /// </summary>
        public int? IdCorVidro { get; set; }

        /// <summary>
        /// Obtém ou define os identificadores para os processos.
        /// </summary>
        public IEnumerable<int> IdsProcessos { get; set; }

        /// <summary>
        /// Obtém ou define os identificadores para as aplicações.
        /// </summary>
        public IEnumerable<int> IdsAplicacoes { get; set; }

        /// <summary>
        /// Obtém ou define o plano de corte da peça.
        /// </summary>
        public string PlanoCorte { get; set; }

        /// <summary>
        /// Obtém ou define o número de etiqueta da chapa de corte.
        /// </summary>
        public string NumeroEtiquetaChapa { get; set; }

        /// <summary>
        /// Obtém ou define o tipo de produto de composição.
        /// </summary>
        public TipoProdutoComposicao? TipoProdutosComposicao { get; set; }

        /// <summary>
        /// Obtém ou define um valor que indica se serão retornadas apenas as peças que
        /// estão aguardando expedição.
        /// </summary>
        public bool ApenasPecasAguardandoExpedicao { get; set; }

        /// <summary>
        /// Obtém ou define um valor que indica se serão retornadas apenas as peças que
        /// estão aguardando entrada no estoque.
        /// </summary>
        public bool ApenasPecasAguardandoEntradaEstoque { get; set; }

        /// <summary>
        /// Obtém ou define um valor que indica se serão retornadas apenas as peças que
        /// estão paradas na produção.
        /// </summary>
        public bool ApenasPecasParadasNaProducao { get; set; }

        /// <summary>
        /// Obtém ou define um valor que indica se serão retornadas apenas as peças repostas.
        /// </summary>
        public bool ApenasPecasRepostas { get; set; }

        /// <summary>
        /// Obtém ou define a data de início para o período de conferência de pedido.
        /// </summary>
        public DateTime? PeriodoConferenciaPedidoInicio { get; set; }

        /// <summary>
        /// Obtém ou define a data de término para o período de conferência de pedido.
        /// </summary>
        public DateTime? PeriodoConferenciaPedidoFim { get; set; }

        /// <summary>
        /// Obtém ou define o tipo de fast delivery.
        /// </summary>
        public TipoFastDelivery? TipoFastDelivery { get; set; }

        /// <summary>
        /// Obtém ou define o identificador da peça "pai" na produção.
        /// </summary>
        public int? IdPecaPai { get; set; }
    }
}
