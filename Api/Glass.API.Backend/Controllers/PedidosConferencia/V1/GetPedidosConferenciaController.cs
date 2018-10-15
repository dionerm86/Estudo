// <copyright file="GetPedidosConferenciaController.cs" company="Sync Softwares">
// Copyright (c) Sync Softwares. Todos os direitos reservados.
// </copyright>

using GDA;
using Glass.API.Backend.Helper.Respostas;
using Glass.API.Backend.Models.Genericas;
using Glass.API.Backend.Models.PedidosConferencia.V1.DadosProducao;
using Glass.Data.DAL;
using Swashbuckle.Swagger.Annotations;
using System.Collections.Generic;
using System.Linq;
using System.Web.Http;

namespace Glass.API.Backend.Controllers.PedidosConferencia.V1
{
    /// <summary>
    /// Controller de pedidos em conferência.
    /// </summary>
    public partial class PedidosConferenciaController : BaseController
    {
        /// <summary>
        /// Recupera as configurações usadas pela tela de listagem de pedidos em conferência.
        /// </summary>
        /// <returns>Um objeto JSON com as configurações da tela.</returns>
        [HttpGet]
        [Route("configuracoes")]
        [SwaggerResponse(200, "Configurações recuperadas.", Type = typeof(Models.PedidosConferencia.Configuracoes.ListaDto))]
        public IHttpActionResult ObterConfiguracoesListaPedidosConferencia()
        {
            using (var sessao = new GDATransaction())
            {
                var configuracoes = new Models.PedidosConferencia.Configuracoes.ListaDto();
                return this.Item(configuracoes);
            }
        }

        /// <summary>
        /// Recupera a lista de pedidos em conferência.
        /// </summary>
        /// <param name="filtro">Os filtros para a busca dos pedidos em conferência.</param>
        /// <returns>Uma lista JSON com os dados dos pedidos em conferência.</returns>
        [HttpGet]
        [Route("")]
        [SwaggerResponse(200, "Pedidos em conferência sem paginação (apenas uma página de retorno) ou última página retornada.", Type = typeof(IEnumerable<Models.PedidosConferencia.Lista.ListaDto>))]
        [SwaggerResponse(204, "Pedidos em conferência não encontradas para o filtro informado.")]
        [SwaggerResponse(206, "Pedidos em conferência paginados (qualquer página, exceto a última).", Type = typeof(IEnumerable<Models.PedidosConferencia.Lista.ListaDto>))]
        [SwaggerResponse(400, "Filtro inválido informado (campo com valor ou formato inválido).", Type = typeof(MensagemDto))]
        public IHttpActionResult ObterListaPedidosConferencia([FromUri] Models.PedidosConferencia.Lista.FiltroDto filtro)
        {
            using (var sessao = new GDATransaction())
            {
                filtro = filtro ?? new Models.PedidosConferencia.Lista.FiltroDto();

                var notasFiscais = PedidoEspelhoDAO.Instance.GetList(
                    (uint)(filtro.IdPedido ?? 0),
                    (uint)(filtro.IdCliente ?? 0),
                    filtro.NomeCliente,
                    (uint)(filtro.IdLoja ?? 0),
                    (uint)(filtro.IdVendedor ?? 0),
                    (uint)(filtro.IdConferente ?? 0),
                    (int)(filtro.Situacao ?? 0),
                    filtro.SituacaoPedidoComercial != null && filtro.SituacaoPedidoComercial.Any() ? string.Join(",", filtro.SituacaoPedidoComercial.Select(f => (int)f)) : null,
                    filtro.IdsProcesso != null && filtro.IdsProcesso.Any() ? string.Join(",", filtro.IdsProcesso) : null,
                    filtro.PeriodoEntregaInicio?.ToShortDateString(),
                    filtro.PeriodoEntregaFim?.ToShortDateString(),
                    filtro.PeriodoFabricaInicio?.ToShortDateString(),
                    filtro.PeriodoFabricaFim?.ToShortDateString(),
                    filtro.PeriodoFinalizacaoConferenciaInicio?.ToShortDateString(),
                    filtro.PeriodoFinalizacaoConferenciaFim?.ToShortDateString(),
                    filtro.PeriodoCadastroConferenciaInicio?.ToShortDateString(),
                    filtro.PeriodoCadastroConferenciaFim?.ToShortDateString(),
                    filtro.PeriodoCadastroPedidoInicio?.ToShortDateString(),
                    filtro.PeriodoCadastroPedidoFim?.ToShortDateString(),
                    false,
                    filtro.PedidosSemAnexo,
                    filtro.SituacaoCnc != null && filtro.SituacaoCnc.Any() ? string.Join(",", filtro.SituacaoCnc.Select(f => (int)f)) : null,
                    filtro.PeriodoProjetoCncInicio?.ToShortDateString(),
                    filtro.PeriodoProjetoCncFim?.ToShortDateString(),
                    filtro.PedidosAComprar,
                    filtro.TiposPedido != null && filtro.TiposPedido.Any() ? string.Join(",", filtro.TiposPedido.Select(f => (int)f)) : null,
                    filtro.IdsRota != null && filtro.IdsRota.Any() ? string.Join(",", filtro.IdsRota) : null,
                    filtro.OrigemPedido ?? 0,
                    filtro.PedidosConferidos,
                    (int?)filtro.TipoVenda,
                    filtro.ObterTraducaoOrdenacao(),
                    filtro.ObterPrimeiroRegistroRetornar(),
                    filtro.NumeroRegistros);

                return this.ListaPaginada(
                    notasFiscais.Select(n => new Models.PedidosConferencia.Lista.ListaDto(n)),
                    filtro,
                    () => PedidoEspelhoDAO.Instance.GetCount(
                        (uint)(filtro.IdPedido ?? 0),
                        (uint)(filtro.IdCliente ?? 0),
                        filtro.NomeCliente,
                        (uint)(filtro.IdLoja ?? 0),
                        (uint)(filtro.IdVendedor ?? 0),
                        (uint)(filtro.IdConferente ?? 0),
                        (int)(filtro.Situacao ?? 0),
                        filtro.SituacaoPedidoComercial != null && filtro.SituacaoPedidoComercial.Any() ? string.Join(",", filtro.SituacaoPedidoComercial.Select(f => (int)f)) : null,
                        filtro.IdsProcesso != null && filtro.IdsProcesso.Any() ? string.Join(",", filtro.IdsProcesso) : null,
                        filtro.PeriodoEntregaInicio?.ToShortDateString(),
                        filtro.PeriodoEntregaFim?.ToShortDateString(),
                        filtro.PeriodoFabricaInicio?.ToShortDateString(),
                        filtro.PeriodoFabricaFim?.ToShortDateString(),
                        filtro.PeriodoFinalizacaoConferenciaInicio?.ToShortDateString(),
                        filtro.PeriodoFinalizacaoConferenciaFim?.ToShortDateString(),
                        filtro.PeriodoCadastroConferenciaInicio?.ToShortDateString(),
                        filtro.PeriodoCadastroConferenciaFim?.ToShortDateString(),
                        filtro.PeriodoCadastroPedidoInicio?.ToShortDateString(),
                        filtro.PeriodoCadastroPedidoFim?.ToShortDateString(),
                        false,
                        filtro.PedidosSemAnexo,
                        filtro.SituacaoCnc != null && filtro.SituacaoCnc.Any() ? string.Join(",", filtro.SituacaoCnc.Select(f => (int)f)) : null,
                        filtro.PeriodoProjetoCncInicio?.ToShortDateString(),
                        filtro.PeriodoProjetoCncFim?.ToShortDateString(),
                        filtro.PedidosAComprar,
                        filtro.TiposPedido != null && filtro.TiposPedido.Any() ? string.Join(",", filtro.TiposPedido.Select(f => (int)f)) : null,
                        filtro.IdsRota != null && filtro.IdsRota.Any() ? string.Join(",", filtro.IdsRota) : null,
                        filtro.OrigemPedido ?? 0,
                        filtro.PedidosConferidos,
                        (int?)filtro.TipoVenda));
            }
        }

        /// <summary>
        /// Recupera as situações de pedido comercial que podem ser filtradas na tela de pedidos em conferência para o controle de pesquisa.
        /// </summary>
        /// <returns>Uma lista JSON com os dados das situações encontradas.</returns>
        [HttpGet]
        [Route("situacoesPedidoComercial")]
        [SwaggerResponse(200, "Situações encontradas.", Type = typeof(IEnumerable<IdNomeDto>))]
        [SwaggerResponse(204, "Situações não encontradas.")]
        public IHttpActionResult ObterSituacoesPedidoComercial()
        {
            using (var sessao = new GDATransaction())
            {
                var situacoes = Data.Helper.DataSources.Instance.GetSituacaoPedidoPCP()
                    .Select(c => new IdNomeDto()
                    {
                        Id = (int?)c.Id,
                        Nome = c.Descr,
                    });

                return this.Lista(situacoes);
            }
        }

        /// <summary>
        /// Recupera as situações de pedidos em conferência para o controle de pesquisa.
        /// </summary>
        /// <returns>Uma lista JSON com os dados das situações encontradas.</returns>
        [HttpGet]
        [Route("situacoes")]
        [SwaggerResponse(200, "Situações encontradas.", Type = typeof(IEnumerable<IdNomeDto>))]
        [SwaggerResponse(204, "Situações não encontradas.")]
        public IHttpActionResult ObterSituacoes()
        {
            using (var sessao = new GDATransaction())
            {
                var situacoes = Data.Helper.DataSources.Instance.GetSituacaoPedidoConferencia()
                    .Where(f => f.Id > 0)
                    .Select(c => new IdNomeDto()
                    {
                        Id = (int?)c.Id,
                        Nome = c.Descr,
                    });

                return this.Lista(situacoes);
            }
        }

        /// <summary>
        /// Recupera as situações de pedidos em conferência para o controle de pesquisa.
        /// </summary>
        /// <returns>Uma lista JSON com os dados das situações encontradas.</returns>
        [HttpGet]
        [Route("{id}/dadosProducao")]
        [SwaggerResponse(200, "Dados de produção encontrados.", Type = typeof(DadosProducaoDto))]
        [SwaggerResponse(404, "Pedido de conferência não encontrado para o id informado.", Type = typeof(MensagemDto))]
        public IHttpActionResult ObterDadosProducao(int id)
        {
            using (var sessao = new GDATransaction())
            {
                var validacao = this.ValidarExistenciaIdPedidoConferencia(sessao, id);

                if (validacao != null)
                {
                    return validacao;
                }

                var dadosProducao = new DadosProducaoDto
                {
                    PedidoProducao = PedidoDAO.Instance.IsProducao(sessao, (uint)id),
                    QuantidadePecasVidroParaEstoque = ProdutosPedidoEspelhoDAO.Instance.ObtemQtdPecasVidroEstoquePedido((uint)id),
                    PossuiEtiquetasNaoImpressas = ProdutosPedidoEspelhoDAO.Instance.PossuiPecaASerImpressa((uint)id),
                };

                return this.Item(dadosProducao);
            }
        }
    }
}
