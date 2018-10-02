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
    /// Classe com os filtros para a tela de consulta de produ��o.
    /// </summary>
    public class FiltroDto : PaginacaoDto
    {
        /// <summary>
        /// Inicia uma nova inst�ncia da classe <see cref="FiltroDto"/>.
        /// </summary>
        public FiltroDto()
            : base(item => new TraducaoOrdenacaoListaProducao(item.Ordenacao))
        {
        }

        /// <summary>
        /// Obt�m ou define o identificador do pedido.
        /// </summary>
        public int? IdPedido { get; set; }

        /// <summary>
        /// Obt�m ou define o identificador da libera��o de pedido.
        /// </summary>
        public int? IdLiberacaoPedido { get; set; }

        /// <summary>
        /// Obt�m ou define o identificador do carregamento.
        /// </summary>
        public int? IdCarregamento { get; set; }

        /// <summary>
        /// Obt�m ou define o identificador do pedido importado.
        /// </summary>
        public int? IdPedidoImportado { get; set; }

        /// <summary>
        /// Obt�m ou define o c�digo do pedido do cliente.
        /// </summary>
        public string CodigoPedidoCliente { get; set; }

        /// <summary>
        /// Obt�m ou define os identificadores das rotas.
        /// </summary>
        public IEnumerable<int> IdsRotas { get; set; }

        /// <summary>
        /// Obt�m ou define o identificador do cliente.
        /// </summary>
        public int? IdCliente { get; set; }

        /// <summary>
        /// Obt�m ou define o nome do cliente.
        /// </summary>
        public string NomeCliente { get; set; }

        /// <summary>
        /// Obt�m ou define o identificador da impress�o.
        /// </summary>
        public int? IdImpressao { get; set; }

        /// <summary>
        /// Obt�m ou define o n�mero da etiqueta da pe�a.
        /// </summary>
        public string NumeroEtiquetaPeca { get; set; }

        /// <summary>
        /// Obt�m ou define as situa��es de produ��o.
        /// </summary>
        public IEnumerable<SituacaoProducao> SituacoesProducao { get; set; }

        /// <summary>
        /// Obt�m ou define o identificador do setor de produ��o.
        /// </summary>
        public int? IdSetor { get; set; }

        /// <summary>
        /// Obt�m ou define a data de in�cio para o per�odo de setor.
        /// </summary>
        public DateTime? PeriodoSetorInicio { get; set; }

        /// <summary>
        /// Obt�m ou define a data de t�rmino para o per�odo de setor.
        /// </summary>
        public DateTime? PeriodoSetorFim { get; set; }

        /// <summary>
        /// Obt�m ou define o identificador da loja.
        /// </summary>
        public int? IdLoja { get; set; }

        /// <summary>
        /// Obt�m ou define o tipo de situa��o de produ��o.
        /// </summary>
        public TipoSituacaoProducao? TipoSituacaoProducao { get; set; }

        /// <summary>
        /// Obt�m ou define a situa��o do pedido.
        /// </summary>
        public SituacaoPedido? SituacaoPedido { get; set; }

        /// <summary>
        /// Obt�m ou define os identificadores dos subgrupos de vidro.
        /// </summary>
        public IEnumerable<int> IdsSubgrupos { get; set; }

        /// <summary>
        /// Obt�m ou define os identificadores dos beneficiamentos.
        /// </summary>
        public IEnumerable<int> IdsBeneficiamentos { get; set; }

        /// <summary>
        /// Obt�m ou define o identificador do funcion�rio vendedor do pedido.
        /// </summary>
        public int? IdVendedorPedido { get; set; }

        /// <summary>
        /// Obt�m ou define o tipo de entrega do pedido.
        /// </summary>
        public TipoEntregaPedido? TipoEntregaPedido { get; set; }

        /// <summary>
        /// Obt�m ou define a data de in�cio para o per�odo de entrega.
        /// </summary>
        public DateTime? PeriodoEntregaInicio { get; set; }

        /// <summary>
        /// Obt�m ou define a data de t�rmino para o per�odo de entrega.
        /// </summary>
        public DateTime? PeriodoEntregaFim { get; set; }

        /// <summary>
        /// Obt�m ou define a largura da pe�a.
        /// </summary>
        public int? LarguraPeca { get; set; }

        /// <summary>
        /// Obt�m ou define a altura da pe�a.
        /// </summary>
        public decimal? AlturaPeca { get; set; }

        /// <summary>
        /// Obt�m ou define os tipos de pedidos.
        /// </summary>
        public IEnumerable<TipoPedidoEnum> TiposPedidos { get; set; }

        /// <summary>
        /// Obt�m ou define os tipos de pe�as a serem exibidas.
        /// </summary>
        public IEnumerable<TipoPecaExibir> TiposPecasExibir { get; set; }

        /// <summary>
        /// Obt�m ou define a data de in�cio para o per�odo de f�brica.
        /// </summary>
        public DateTime? PeriodoFabricaInicio { get; set; }

        /// <summary>
        /// Obt�m ou define a data de t�rmino para o per�odo de f�brica.
        /// </summary>
        public DateTime? PeriodoFabricaFim { get; set; }

        /// <summary>
        /// Obt�m ou define a espessura da pe�a.
        /// </summary>
        public decimal? EspessuraPeca { get; set; }

        /// <summary>
        /// Obt�m ou define o identificador da cor do vidro.
        /// </summary>
        public int? IdCorVidro { get; set; }

        /// <summary>
        /// Obt�m ou define os identificadores para os processos.
        /// </summary>
        public IEnumerable<int> IdsProcessos { get; set; }

        /// <summary>
        /// Obt�m ou define os identificadores para as aplica��es.
        /// </summary>
        public IEnumerable<int> IdsAplicacoes { get; set; }

        /// <summary>
        /// Obt�m ou define o plano de corte da pe�a.
        /// </summary>
        public string PlanoCorte { get; set; }

        /// <summary>
        /// Obt�m ou define o n�mero de etiqueta da chapa de corte.
        /// </summary>
        public string NumeroEtiquetaChapa { get; set; }

        /// <summary>
        /// Obt�m ou define o tipo de produto de composi��o.
        /// </summary>
        public TipoProdutoComposicao? TipoProdutosComposicao { get; set; }

        /// <summary>
        /// Obt�m ou define um valor que indica se ser�o retornadas apenas as pe�as que
        /// est�o aguardando expedi��o.
        /// </summary>
        public bool ApenasPecasAguardandoExpedicao { get; set; }

        /// <summary>
        /// Obt�m ou define um valor que indica se ser�o retornadas apenas as pe�as que
        /// est�o aguardando entrada no estoque.
        /// </summary>
        public bool ApenasPecasAguardandoEntradaEstoque { get; set; }

        /// <summary>
        /// Obt�m ou define um valor que indica se ser�o retornadas apenas as pe�as que
        /// est�o paradas na produ��o.
        /// </summary>
        public bool ApenasPecasParadasNaProducao { get; set; }

        /// <summary>
        /// Obt�m ou define um valor que indica se ser�o retornadas apenas as pe�as repostas.
        /// </summary>
        public bool ApenasPecasRepostas { get; set; }

        /// <summary>
        /// Obt�m ou define a data de in�cio para o per�odo de confer�ncia de pedido.
        /// </summary>
        public DateTime? PeriodoConferenciaPedidoInicio { get; set; }

        /// <summary>
        /// Obt�m ou define a data de t�rmino para o per�odo de confer�ncia de pedido.
        /// </summary>
        public DateTime? PeriodoConferenciaPedidoFim { get; set; }

        /// <summary>
        /// Obt�m ou define o tipo de fast delivery.
        /// </summary>
        public TipoFastDelivery? TipoFastDelivery { get; set; }

        /// <summary>
        /// Obt�m ou define o identificador da pe�a "pai" na produ��o.
        /// </summary>
        public int? IdPecaPai { get; set; }
    }
}
