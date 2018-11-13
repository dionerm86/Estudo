// <copyright file="GetCarregamentosController.cs" company="Sync Softwares">
// Copyright (c) Sync Softwares. Todos os direitos reservados.
// </copyright>

using GDA;
using Glass.API.Backend.Helper;
using Glass.API.Backend.Helper.Respostas;
using Glass.API.Backend.Models.Carregamentos.V1.Lista;
using Glass.API.Backend.Models.Genericas.V1;
using Glass.Configuracoes;
using Glass.Data.DAL;
using Swashbuckle.Swagger.Annotations;
using System.Collections.Generic;
using System.Linq;
using System.Web.Http;

namespace Glass.API.Backend.Controllers.Carregamentos.V1
{
    /// <summary>
    /// Controller de carregamentos.
    /// </summary>
    public partial class CarregamentosController : BaseController
    {
        /// <summary>
        /// Recupera as configurações usadas pela tela de listagem de carregamentos.
        /// </summary>
        /// <returns>Um objeto JSON com as configurações da tela.</returns>
        [HttpGet]
        [Route("configuracoes")]
        [SwaggerResponse(200, "Configurações recuperadas.", Type = typeof(Models.Carregamentos.V1.Configuracoes.ListaDto))]
        public IHttpActionResult ObterConfiguracoes()
        {
            using (var sessao = new GDATransaction())
            {
                var configuracoes = new Models.Carregamentos.V1.Configuracoes.ListaDto();
                return this.Item(configuracoes);
            }
        }

        /// <summary>
        /// Recupera a lista de carregamentos.
        /// </summary>
        /// <param name="filtro">Os filtros para a busca dos carregamentos.</param>
        /// <returns>Uma lista JSON com os dados dos carregamentos.</returns>
        [HttpGet]
        [Route("")]
        [SwaggerResponse(200, "Carregamentos sem paginação (apenas uma página de retorno) ou última página retornada.", Type = typeof(IEnumerable<Models.Carregamentos.V1.Lista.ListaDto>))]
        [SwaggerResponse(204, "Carregamentos não encontrados para o filtro informado.")]
        [SwaggerResponse(206, "Carregamentos paginados (qualquer página, exceto a última).", Type = typeof(IEnumerable<Models.Carregamentos.V1.Lista.ListaDto>))]
        [SwaggerResponse(400, "Filtro inválido informado (campo com valor ou formato inválido).", Type = typeof(MensagemDto))]
        public IHttpActionResult ObterListaCarregamentos([FromUri] FiltroDto filtro)
        {
            using (var sessao = new GDATransaction())
            {
                filtro = filtro ?? new FiltroDto();

                var carregamentos = CarregamentoDAO.Instance.GetListWithExpression(
                    (uint)(filtro.Id ?? 0),
                    (uint)(filtro.IdOrdemCarga ?? 0),
                    (uint)(filtro.IdPedido ?? 0),
                    filtro.IdRota ?? 0,
                    (uint)(filtro.IdMotorista ?? 0),
                    filtro.Placa,
                    ((int?)filtro.SituacaoCarregamento)?.ToString(),
                    filtro.PeriodoPrevisaoSaidaInicio?.ToShortDateString(),
                    filtro.PeriodoPrevisaoSaidaFim?.ToShortDateString(),
                    (uint)(filtro.IdLoja ?? 0),
                    filtro.ObterTraducaoOrdenacao(),
                    filtro.ObterPrimeiroRegistroRetornar(),
                    filtro.NumeroRegistros);

                return this.ListaPaginada(
                    carregamentos.Select(c => new ListaDto(c)),
                    filtro,
                    () => CarregamentoDAO.Instance.GetListWithExpressionCount(
                        (uint)(filtro.Id ?? 0),
                        (uint)(filtro.IdOrdemCarga ?? 0),
                        (uint)(filtro.IdPedido ?? 0),
                        filtro.IdRota ?? 0,
                        (uint)(filtro.IdMotorista ?? 0),
                        filtro.Placa,
                        ((int?)filtro.SituacaoCarregamento)?.ToString(),
                        filtro.PeriodoPrevisaoSaidaInicio?.ToShortDateString(),
                        filtro.PeriodoPrevisaoSaidaFim?.ToShortDateString(),
                        (uint)(filtro.IdLoja ?? 0)));
            }
        }

        /// <summary>
        /// Recupera as situações de carregamento.
        /// </summary>
        /// <returns>Uma lista JSON com as situações.</returns>
        [HttpGet]
        [Route("situacoes")]
        [SwaggerResponse(200, "Situações de carregamento encontradas.", Type = typeof(IEnumerable<IdNomeDto>))]
        [SwaggerResponse(204, "Situações não encontradas.")]
        public IHttpActionResult ObterSituacoes()
        {
            using (var sessao = new GDATransaction())
            {
                var situacoes = new ConversorEnum<Data.Model.Carregamento.SituacaoCarregamentoEnum>()
                    .ObterTraducao();

                return this.Lista(situacoes);
            }
        }

        /// <summary>
        /// Recupera as pendências de faturamento.
        /// </summary>
        /// <param name="id">O identificador </param>
        /// <returns>Uma lista JSON com as pendências de faturamento.</returns>
        [HttpGet]
        [Route("{id:int}/pendenciasFaturamento")]
        [SwaggerResponse(200, "Pendências de faturamento encontradas.", Type = typeof(IEnumerable<IdNomeDto>))]
        [SwaggerResponse(204, "Pendências de faturamento não encontradas.")]
        public IHttpActionResult ObterPendenciasFaturamento(int id)
        {
            using (var sessao = new GDATransaction())
            {
                var pendencias = new ConversorEnum<Data.Model.Carregamento.SituacaoCarregamentoEnum>()
                    .ObterTraducao();

                return this.Lista(pendencias);
            }
        }

        /// <summary>
        /// Recupera dados do faturamento.
        /// </summary>
        /// <param name="id">O identificador do carregamento.</param>
        /// <returns>Uma lista JSON com dados do faturamento.</returns>
        [HttpGet]
        [Route("{id:int}/dadosFaturamento")]
        [SwaggerResponse(200, "Dados do faturamento encontradas.", Type = typeof(IEnumerable<DadosFaturamentoDto>))]
        [SwaggerResponse(204, "Dados do faturamento não encontradas.")]
        public IHttpActionResult ObterDadosFaturamento(int id)
        {
            using (var sessao = new GDATransaction())
            {
                var dadosFaturamento = WebGlass.Business.OrdemCarga.Fluxo.CarregamentoFluxo.Instance.BuscarFaturamento((uint)id)
                    .Select(f => new DadosFaturamentoDto
                    {
                        IdLiberacao = f.Item1,
                        IdNotaFiscal = f.Item2,
                    });

                return this.Lista(dadosFaturamento);
            }
        }
    }
}
