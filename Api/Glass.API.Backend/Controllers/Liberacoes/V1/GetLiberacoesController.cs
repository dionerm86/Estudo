// <copyright file="GetLiberacoesController.cs" company="Sync Softwares">
// Copyright (c) Sync Softwares. Todos os direitos reservados.
// </copyright>

using GDA;
using Glass.API.Backend.Helper.Respostas;
using Glass.API.Backend.Models.Genericas.V1;
using Glass.API.Backend.Models.Liberacoes.V1.Lista;
using Glass.Configuracoes;
using Glass.Data.DAL;
using Swashbuckle.Swagger.Annotations;
using System.Collections.Generic;
using System.Linq;
using System.Web.Http;

namespace Glass.API.Backend.Controllers.Liberacoes.V1
{
    /// <summary>
    /// Controller de liberações.
    /// </summary>
    public partial class LiberacoesController : BaseController
    {
        /// <summary>
        /// Recupera as configurações usadas pela tela de listagem de liberações.
        /// </summary>
        /// <returns>Um objeto JSON com as configurações da tela.</returns>
        [HttpGet]
        [Route("configuracoes")]
        [SwaggerResponse(200, "Configurações recuperadas.", Type = typeof(Models.Liberacoes.V1.Configuracoes.ListaDto))]
        public IHttpActionResult ObterConfiguracoesListaLiberacoes()
        {
            using (var sessao = new GDATransaction())
            {
                var configuracoes = new Models.Liberacoes.V1.Configuracoes.ListaDto();
                return this.Item(configuracoes);
            }
        }

        /// <summary>
        /// Recupera a lista de liberações.
        /// </summary>
        /// <param name="filtro">Os filtros para a busca das liberações.</param>
        /// <returns>Uma lista JSON com os dados das liberações.</returns>
        [HttpGet]
        [Route("")]
        [SwaggerResponse(200, "Liberações sem paginação (apenas uma página de retorno) ou última página retornada.", Type = typeof(IEnumerable<Models.Liberacoes.V1.Lista.ListaDto>))]
        [SwaggerResponse(204, "Liberações não encontradas para o filtro informado.")]
        [SwaggerResponse(206, "Liberações paginados (qualquer página, exceto a última).", Type = typeof(IEnumerable<Models.Liberacoes.V1.Lista.ListaDto>))]
        [SwaggerResponse(400, "Filtro inválido informado (campo com valor ou formato inválido).", Type = typeof(MensagemDto))]
        public IHttpActionResult ObterListaLiberacoes([FromUri] Models.Liberacoes.V1.Lista.FiltroDto filtro)
        {
            using (var sessao = new GDATransaction())
            {
                filtro = filtro ?? new Models.Liberacoes.V1.Lista.FiltroDto();

                var liberacoes = LiberarPedidoDAO.Instance.GetList(
                    (uint)(filtro.Id ?? 0),
                    (uint)(filtro.IdPedido ?? 0),
                    filtro.NumeroNfe,
                    (uint)(filtro.IdFuncionario ?? 0),
                    (uint)(filtro.IdCliente ?? 0),
                    filtro.NomeCliente,
                    filtro.LiberacaoComSemNotaFiscal,
                    filtro.PeriodoCadastroInicio?.ToShortDateString(),
                    filtro.PeriodoCadastroFim?.ToShortDateString(),
                    filtro.Situacao,
                    (uint)(filtro.IdLoja ?? 0),
                    filtro.PeriodoCancelamentoInicio?.ToShortDateString(),
                    filtro.PeriodoCancelamentoFim?.ToShortDateString(),
                    filtro.ObterTraducaoOrdenacao(),
                    filtro.ObterPrimeiroRegistroRetornar(),
                    filtro.NumeroRegistros);

                return this.ListaPaginada(
                    liberacoes.Select(o => new Models.Liberacoes.V1.Lista.ListaDto(o)),
                    filtro,
                    () => LiberarPedidoDAO.Instance.GetCount(
                        (uint)(filtro.Id ?? 0),
                        (uint)(filtro.IdPedido ?? 0),
                        filtro.NumeroNfe,
                        (uint)(filtro.IdFuncionario ?? 0),
                        (uint)(filtro.IdCliente ?? 0),
                        filtro.NomeCliente,
                        filtro.LiberacaoComSemNotaFiscal,
                        filtro.PeriodoCadastroInicio?.ToShortDateString(),
                        filtro.PeriodoCadastroFim?.ToShortDateString(),
                        filtro.Situacao,
                        (uint)(filtro.IdLoja ?? 0),
                        filtro.PeriodoCancelamentoInicio?.ToShortDateString(),
                        filtro.PeriodoCancelamentoFim?.ToShortDateString()));
            }
        }

        /// <summary>
        /// Recupera as situações das liberações.
        /// </summary>
        /// <returns>Uma lista JSON com as situações das liberações.</returns>
        [HttpGet]
        [Route("situacoes")]
        [SwaggerResponse(200, "Situações de liberações encontradas.", Type = typeof(IEnumerable<IdNomeDto>))]
        [SwaggerResponse(204, "Situações de liberações não encontradas.")]
        public IHttpActionResult ObterSituacoesLiberacoes()
        {
            using (var sessao = new GDATransaction())
            {
                var situacoes = new Helper.ConversorEnum<Tipo>()
                    .ObterTraducao();

                return this.Lista(situacoes);
            }
        }
    }
}
