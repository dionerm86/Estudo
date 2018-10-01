// <copyright file="GetProducaoController.cs" company="Sync Softwares">
// Copyright (c) Sync Softwares. Todos os direitos reservados.
// </copyright>

using GDA;
using Glass.API.Backend.Helper.Respostas;
using Glass.API.Backend.Models.Genericas;
using Glass.API.Backend.Models.Producao.V1.Lista;
using Glass.Data.DAL;
using Glass.Data.Helper;
using Swashbuckle.Swagger.Annotations;
using System.Collections.Generic;
using System.Linq;
using System.Web.Http;

namespace Glass.API.Backend.Controllers.Producao.V1
{
    /// <summary>
    /// Controller de produção.
    /// </summary>
    public partial class ProducaoController : BaseController
    {
        /// <summary>
        /// Recupera a lista de peças para a tela de consulta de produção.
        /// </summary>
        /// <param name="filtro">Os dados informados para filtro na tela.</param>
        /// <returns>Uma lista JSON com as peças em produção.</returns>
        [HttpGet]
        [Route("situacoes")]
        [SwaggerResponse(200, "Peças em produção sem paginação (apenas uma página de retorno) ou última página retornada.", Type = typeof(IEnumerable<IdNomeDto>))]
        [SwaggerResponse(204, "Peças em produção não encontradas para o filtro informado.")]
        [SwaggerResponse(206, "Peças em produção paginadas (qualquer página, exceto a última).", Type = typeof(IEnumerable<IdNomeDto>))]
        [SwaggerResponse(400, "Filtro inválido informado (campo com valor ou formato inválido).", Type = typeof(MensagemDto))]
        public IHttpActionResult ObterPecasProducao([FromUri] FiltroDto filtro)
        {
            filtro = filtro ?? new FiltroDto();

            using (var sessao = new GDATransaction())
            {
                var pecas = ProdutoPedidoProducaoDAO.Instance.GetListConsulta(
                    filtro.IdCarregamento.GetValueOrDefault(),
                    filtro.IdLiberacaoPedido.ToString(),
                    (uint)filtro.IdPedido.GetValueOrDefault(),
                    filtro.IdPedidoImportado.ToString(),
                    (uint)filtro.IdImpressao.GetValueOrDefault(),
                    filtro.CodigoPedidoCliente,
                    filtro.IdsRotas.ObterComoString(),
                    (uint)filtro.IdCliente.GetValueOrDefault(),
                    filtro.NomeCliente,
                    filtro.NumeroEtiquetaPeca,
                    filtro.PeriodoSetorInicio.FormatarData(),
                    filtro.PeriodoSetorFim.FormatarData(),
                    filtro.PeriodoEntregaInicio.FormatarData(),
                    filtro.PeriodoEntregaFim.FormatarData(),
                    filtro.PeriodoFabricaInicio.FormatarData(),
                    filtro.PeriodoFabricaFim.FormatarData(),
                    filtro.PeriodoConferenciaPedidoInicio.FormatarData(),
                    filtro.PeriodoConferenciaPedidoFim.FormatarData(),
                    filtro.IdSetor.GetValueOrDefault(),
                    filtro.SituacoesProducao.Cast<int>().ObterComoString(),
                    (int)filtro.SituacaoPedido.GetValueOrDefault(),
                    (int)filtro.TipoSituacaoProducao.GetValueOrDefault(),
                    filtro.IdsSubgrupos.ObterComoString(),
                    (uint)filtro.TipoEntregaPedido.GetValueOrDefault(),
                    filtro.TiposPecasExibir.Cast<int>().ObterComoString(),
                    (uint)filtro.IdVendedorPedido.GetValueOrDefault(),
                    filtro.TiposPedidos.Cast<int>().ObterComoString(),
                    (uint)filtro.IdCorVidro.GetValueOrDefault(),
                    (int)filtro.AlturaPeca.GetValueOrDefault(),
                    filtro.LarguraPeca.GetValueOrDefault(),
                    (float)filtro.EspessuraPeca.GetValueOrDefault(),
                    filtro.IdsProcessos.ObterComoString(),
                    filtro.IdsAplicacoes.ObterComoString(),
                    filtro.ApenasPecasAguardandoExpedicao,
                    filtro.ApenasPecasAguardandoEntradaEstoque,
                    filtro.IdsBeneficiamentos.ObterComoString(),
                    filtro.PlanoCorte,
                    filtro.NumeroEtiquetaChapa,
                    (uint)filtro.TipoFastDelivery.GetValueOrDefault(),
                    filtro.ApenasPecasParadasNaProducao,
                    filtro.ApenasPecasRepostas,
                    (uint)filtro.IdLoja.GetValueOrDefault(),
                    (int?)filtro.TipoProdutosComposicao,
                    (uint)filtro.IdPecaPai.GetValueOrDefault(),
                    filtro.Pagina,
                    filtro.ObterTraducaoOrdenacao(),
                    filtro.ObterPrimeiroRegistroRetornar(),
                    filtro.NumeroRegistros);

                return this.ListaPaginada(
                    pecas.Select(p => new ListaDto(p)),
                    filtro,
                    () => ProdutoPedidoProducaoDAO.Instance.GetCountConsulta(
                        filtro.IdCarregamento.GetValueOrDefault(),
                        filtro.IdLiberacaoPedido.ToString(),
                        (uint)filtro.IdPedido.GetValueOrDefault(),
                        filtro.IdPedidoImportado.ToString(),
                        (uint)filtro.IdImpressao.GetValueOrDefault(),
                        filtro.CodigoPedidoCliente,
                        filtro.IdsRotas.ObterComoString(),
                        (uint)filtro.IdCliente.GetValueOrDefault(),
                        filtro.NomeCliente,
                        filtro.NumeroEtiquetaPeca,
                        filtro.PeriodoSetorInicio.FormatarData(),
                        filtro.PeriodoSetorFim.FormatarData(),
                        filtro.PeriodoEntregaInicio.FormatarData(),
                        filtro.PeriodoEntregaFim.FormatarData(),
                        filtro.PeriodoFabricaInicio.FormatarData(),
                        filtro.PeriodoFabricaFim.FormatarData(),
                        filtro.PeriodoConferenciaPedidoInicio.FormatarData(),
                        filtro.PeriodoConferenciaPedidoFim.FormatarData(),
                        filtro.IdSetor.GetValueOrDefault(),
                        filtro.SituacoesProducao.Cast<int>().ObterComoString(),
                        (int)filtro.SituacaoPedido.GetValueOrDefault(),
                        (int)filtro.TipoSituacaoProducao.GetValueOrDefault(),
                        filtro.IdsSubgrupos.ObterComoString(),
                        (uint)filtro.TipoEntregaPedido.GetValueOrDefault(),
                        filtro.TiposPecasExibir.Cast<int>().ObterComoString(),
                        (uint)filtro.IdVendedorPedido.GetValueOrDefault(),
                        filtro.TiposPedidos.Cast<int>().ObterComoString(),
                        (uint)filtro.IdCorVidro.GetValueOrDefault(),
                        (int)filtro.AlturaPeca.GetValueOrDefault(),
                        filtro.LarguraPeca.GetValueOrDefault(),
                        (float)filtro.EspessuraPeca.GetValueOrDefault(),
                        filtro.IdsProcessos.ObterComoString(),
                        filtro.IdsAplicacoes.ObterComoString(),
                        filtro.ApenasPecasAguardandoExpedicao,
                        filtro.ApenasPecasAguardandoEntradaEstoque,
                        filtro.IdsBeneficiamentos.ObterComoString(),
                        filtro.PlanoCorte,
                        filtro.NumeroEtiquetaChapa,
                        (uint)filtro.TipoFastDelivery.GetValueOrDefault(),
                        filtro.ApenasPecasParadasNaProducao,
                        filtro.ApenasPecasRepostas,
                        (uint)filtro.IdLoja.GetValueOrDefault(),
                        (int?)filtro.TipoProdutosComposicao,
                        (uint)filtro.IdPecaPai.GetValueOrDefault(),
                        filtro.Pagina));
            }
        }

        /// <summary>
        /// Recupera a lista de situações de produção possíveis.
        /// </summary>
        /// <returns>Uma lista JSON com os dados básicos das situações de produção.</returns>
        [HttpGet]
        [Route("situacoes")]
        [SwaggerResponse(200, "Situações de produção encontradas.", Type = typeof(IEnumerable<IdNomeDto>))]
        [SwaggerResponse(204, "Situações de produção não encontradas.")]
        public IHttpActionResult ObterSituacoes()
        {
            using (var sessao = new GDATransaction())
            {
                var situacoes = DataSources.Instance.GetSituacaoProducao()
                    .Select(s => new IdNomeDto
                    {
                        Id = (int)(s.Id ?? 0),
                        Nome = s.Descr,
                    });

                return this.Lista(situacoes);
            }
        }
    }
}
