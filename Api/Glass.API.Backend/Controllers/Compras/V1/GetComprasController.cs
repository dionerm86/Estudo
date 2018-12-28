// <copyright file="GetComprasController.cs" company="Sync Softwares">
// Copyright (c) Sync Softwares. Todos os direitos reservados.
// </copyright>

using GDA;
using Glass.API.Backend.Helper;
using Glass.API.Backend.Helper.Respostas;
using Glass.API.Backend.Models.Compras.V1.Lista;
using Glass.API.Backend.Models.Genericas.V1;
using Glass.Configuracoes;
using Glass.Data.DAL;
using Swashbuckle.Swagger.Annotations;
using System.Collections.Generic;
using System.Linq;
using System.Web.Http;

namespace Glass.API.Backend.Controllers.Compras.V1
{
    /// <summary>
    /// Controller de compras.
    /// </summary>
    public partial class ComprasController : BaseController
    {
        /// <summary>
        /// Recupera as configurações usadas pela tela de listagem de compras.
        /// </summary>
        /// <returns>Um objeto JSON com as configurações da tela.</returns>
        [HttpGet]
        [Route("configuracoes")]
        [SwaggerResponse(200, "Configurações recuperadas.", Type = typeof(Models.Compras.V1.Configuracoes.ListaDto))]
        public IHttpActionResult ObterConfiguracoes()
        {
            using (var sessao = new GDATransaction())
            {
                var configuracoes = new Models.Compras.V1.Configuracoes.ListaDto();
                return this.Item(configuracoes);
            }
        }

        /// <summary>
        /// Recupera a lista de compras.
        /// </summary>
        /// <param name="filtro">Os filtros para a busca das compras.</param>
        /// <returns>Uma lista JSON com os dados dos compras.</returns>
        [HttpGet]
        [Route("")]
        [SwaggerResponse(200, "Compras encontradas sem paginação (apenas uma página de retorno) ou última página retornada.", Type = typeof(IEnumerable<Models.Compras.V1.Lista.ListaDto>))]
        [SwaggerResponse(204, "Compras não encontradas.")]
        [SwaggerResponse(206, "Compras paginadas (qualquer página, exceto a última).", Type = typeof(IEnumerable<Models.Compras.V1.Lista.ListaDto>))]
        public IHttpActionResult ObterListaCompras([FromUri] FiltroDto filtro)
        {
            using (var sessao = new GDATransaction())
            {
                filtro = filtro ?? new FiltroDto();

                var compras = CompraDAO.Instance.GetList(
                    (uint)(filtro.Id ?? 0),
                    (uint)(filtro.IdPedido ?? 0),
                    (uint)(filtro.IdCotacaoCompra ?? 0),
                    filtro.NotaFiscal,
                    (uint)(filtro.IdFornecedor ?? 0),
                    filtro.NomeFornecedor,
                    filtro.Observacao,
                    filtro.Situacao ?? 0,
                    filtro.Atrasada.GetValueOrDefault(false),
                    filtro.PeriodoCadastroInicio?.ToShortDateString(),
                    filtro.PeriodoCadastroFim?.ToShortDateString(),
                    filtro.PeriodoEntregaFabricaInicio?.ToShortDateString(),
                    filtro.PeriodoEntregaFabricaFim?.ToShortDateString(),
                    filtro.PeriodoSaidaInicio?.ToShortDateString(),
                    filtro.PeriodoSaidaFim?.ToShortDateString(),
                    filtro.PeriodoFinalizacaoInicio?.ToShortDateString(),
                    filtro.PeriodoFinalizacaoFim?.ToShortDateString(),
                    filtro.PeriodoEntradaInicio?.ToShortDateString(),
                    filtro.PeriodoEntradaFim?.ToShortDateString(),
                    filtro.IdsGrupoProduto != null && filtro.IdsGrupoProduto.Any() ? string.Join(",", filtro.IdsGrupoProduto) : null,
                    (uint)(filtro.IdSubgrupoProduto ?? 0),
                    filtro.CodigoProduto,
                    filtro.DescricaoProduto,
                    filtro.CentroDeCustoDivergente.GetValueOrDefault(false),
                    filtro.IdLoja ?? 0,
                    filtro.ObterTraducaoOrdenacao(),
                    filtro.ObterPrimeiroRegistroRetornar(),
                    filtro.NumeroRegistros);

                return this.ListaPaginada(
                    compras.Select(c => new ListaDto(c)),
                    filtro,
                    () => CompraDAO.Instance.GetCount(
                        (uint)(filtro.Id ?? 0),
                        (uint)(filtro.IdPedido ?? 0),
                        (uint)(filtro.IdCotacaoCompra ?? 0),
                        filtro.NotaFiscal,
                        (uint)(filtro.IdFornecedor ?? 0),
                        filtro.NomeFornecedor,
                        filtro.Observacao,
                        filtro.Situacao ?? 0,
                        filtro.Atrasada.GetValueOrDefault(false),
                        filtro.PeriodoCadastroInicio?.ToShortDateString(),
                        filtro.PeriodoCadastroFim?.ToShortDateString(),
                        filtro.PeriodoEntregaFabricaInicio?.ToShortDateString(),
                        filtro.PeriodoEntregaFabricaFim?.ToShortDateString(),
                        filtro.PeriodoSaidaInicio?.ToShortDateString(),
                        filtro.PeriodoSaidaFim?.ToShortDateString(),
                        filtro.PeriodoFinalizacaoInicio?.ToShortDateString(),
                        filtro.PeriodoFinalizacaoFim?.ToShortDateString(),
                        filtro.PeriodoEntradaInicio?.ToShortDateString(),
                        filtro.PeriodoEntradaFim?.ToShortDateString(),
                        filtro.IdsGrupoProduto != null && filtro.IdsGrupoProduto.Any() ? string.Join(",", filtro.IdsGrupoProduto) : null,
                        (uint)(filtro.IdSubgrupoProduto ?? 0),
                        filtro.CodigoProduto,
                        filtro.DescricaoProduto,
                        filtro.CentroDeCustoDivergente.GetValueOrDefault(false),
                        filtro.IdLoja ?? 0));
            }
        }

        /// <summary>
        /// Recupera as situações de compra.
        /// </summary>
        /// <returns>Uma lista JSON com as situações.</returns>
        [HttpGet]
        [Route("situacoes")]
        [SwaggerResponse(200, "Situações de compra encontradas.", Type = typeof(IEnumerable<IdNomeDto>))]
        [SwaggerResponse(204, "Situações de compra não encontradas.")]
        public IHttpActionResult ObterSituacoes()
        {
            using (var sessao = new GDATransaction())
            {
                var situacoes = new ConversorEnum<Data.Model.Compra.SituacaoEnum>()
                    .ObterTraducao();

                return this.Lista(situacoes);
            }
        }
    }
}