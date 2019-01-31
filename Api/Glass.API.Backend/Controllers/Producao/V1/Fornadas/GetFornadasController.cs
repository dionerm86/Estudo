// <copyright file="GetFornadasController.cs" company="Sync Softwares">
// Copyright (c) Sync Softwares. Todos os direitos reservados.
// </copyright>

using GDA;
using Glass.API.Backend.Models.Producao.V1.Fornadas.Lista;
using Glass.Data.DAL;
using Swashbuckle.Swagger.Annotations;
using System.Collections.Generic;
using System.Linq;
using System.Web.Http;

namespace Glass.API.Backend.Controllers.Producao.V1.Fornadas
{
    /// <summary>
    /// Controller de fornadas.
    /// </summary>
    public partial class FornadasController : BaseController
    {
        /// <summary>
        /// Recupera os itens para exibição na tela de listagem de fornadas.
        /// </summary>
        /// <param name="filtro">Os dados informados para filtro na tela.</param>
        /// <returns>Uma lista JSON com as fornadas.</returns>
        [HttpGet]
        [Route("")]
        [SwaggerResponse(200, "Fornadas encontradas sem paginação (apenas uma página de retorno) ou última página retornada.", Type = typeof(IEnumerable<ListaDto>))]
        [SwaggerResponse(204, "Fornadas não encontradas para o filtro informado.")]
        [SwaggerResponse(206, "Fornadas paginadas (qualquer página, exceto a última).", Type = typeof(IEnumerable<ListaDto>))]
        public IHttpActionResult ObterFornadas([FromUri] FiltroDto filtro)
        {
            filtro = filtro ?? new FiltroDto();

            using (var sessao = new GDATransaction())
            {
                var fornadas = FornadaDAO.Instance.PesquisarFornadas(
                    filtro.Id ?? 0,
                    filtro.IdPedido ?? 0,
                    filtro.PeriodoCadastroInicio?.ToShortDateString(),
                    filtro.PeriodoCadastroFim?.ToShortDateString(),
                    filtro.CodigoEtiqueta,
                    filtro.Espessura ?? 0,
                    filtro.IdsCorVidro != null && filtro.IdsCorVidro.Any() ? string.Join(",", filtro.IdsCorVidro) : string.Empty,
                    filtro.ObterTraducaoOrdenacao(),
                    filtro.ObterPrimeiroRegistroRetornar(),
                    filtro.NumeroRegistros);

                return this.ListaPaginada(
                    fornadas.Select(f => new ListaDto(f)),
                    filtro,
                    () => FornadaDAO.Instance.PesquisarFornadasCount(
                        filtro.Id ?? 0,
                        filtro.IdPedido ?? 0,
                        filtro.PeriodoCadastroInicio?.ToShortDateString(),
                        filtro.PeriodoCadastroFim?.ToShortDateString(),
                        filtro.CodigoEtiqueta,
                        filtro.Espessura ?? 0,
                        filtro.IdsCorVidro != null && filtro.IdsCorVidro.Any() ? string.Join(",", filtro.IdsCorVidro) : string.Empty));
            }
        }

        /// <summary>
        /// Recupera os itens para exibição na tela de listagem de fornadas.
        /// </summary>
        /// <param name="idFornada">Os dados informados para filtro na tela.</param>
        /// <returns>Uma lista JSON com as fornadas.</returns>
        [HttpGet]
        [Route("{idFornada:int}/pecas")]
        [SwaggerResponse(200, "Peças da fornada encontradas.", Type = typeof(IEnumerable<Models.Producao.V1.Fornadas.PecasFornada.ListaDto>))]
        [SwaggerResponse(204, "Peças da fornada não encontradas para o filtro informado.")]
        public IHttpActionResult ObterPecasFornada(int idFornada)
        {
            using (var sessao = new GDATransaction())
            {
                var pecasFornada = ProdutoPedidoProducaoDAO.Instance.ObterPecasFornada(idFornada);

                return this.Lista(pecasFornada.Select(pf => new Models.Producao.V1.Fornadas.PecasFornada.ListaDto(pf)));
            }
        }
    }
}