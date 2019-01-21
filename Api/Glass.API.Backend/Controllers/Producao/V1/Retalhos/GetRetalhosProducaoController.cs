// <copyright file="GetRetalhosProducaoController.cs" company="Sync Softwares">
// Copyright (c) Sync Softwares. Todos os direitos reservados.
// </copyright>

using GDA;
using Glass.API.Backend.Models.Genericas.V1;
using Glass.API.Backend.Models.Producao.V1.Retalhos.Lista;
using Glass.Data.DAL;
using Glass.Data.Model;
using Swashbuckle.Swagger.Annotations;
using System.Collections.Generic;
using System.Linq;
using System.Web.Http;

namespace Glass.API.Backend.Controllers.Producao.V1.Retalhos
{
    /// <summary>
    /// Controller de retalhos de produção.
    /// </summary>
    public partial class RetalhosProducaoController : BaseController
    {
        /// <summary>
        /// Recupera a lista de retalhos de produção.
        /// </summary>
        /// <param name="filtro">Os dados informados para filtro na tela.</param>
        /// <returns>Uma lista JSON com os retalhos de produção.</returns>
        [HttpGet]
        [Route("")]
        [SwaggerResponse(200, "Retalhos de produção encontrados sem paginação (apenas uma página de retorno) ou última página retornada.", Type = typeof(IEnumerable<ListaDto>))]
        [SwaggerResponse(204, "Retalhos de produção não encontrados.")]
        [SwaggerResponse(206, "Retalhos de produção paginados (qualquer página, exceto a última).", Type = typeof(IEnumerable<ListaDto>))]
        public IHttpActionResult ObterRetalhosProducao([FromUri] FiltroDto filtro)
        {
            filtro = filtro ?? new FiltroDto();

            using (var sessao = new GDATransaction())
            {
                var retalhosProducao = RetalhoProducaoDAO.Instance.ObterLista(
                    filtro.CodigoProduto,
                    filtro.DescricaoProduto,
                    filtro.PeriodoCadastroInicio?.ToShortDateString(),
                    filtro.PeriodoCadastroFim?.ToShortDateString(),
                    filtro.PeriodoUsoInicio?.ToShortDateString(),
                    filtro.PeriodoUsoFim?.ToShortDateString(),
                    filtro.Situacao,
                    filtro.IdsCorVidro != null && filtro.IdsCorVidro.Any() ? string.Join(",", filtro.IdsCorVidro) : string.Empty,
                    (double)(filtro.Espessura ?? 0),
                    (double)(filtro.AlturaInicio ?? 0),
                    (double)(filtro.AlturaFim ?? 0),
                    (double)(filtro.LarguraInicio ?? 0),
                    (double)(filtro.LarguraFim ?? 0),
                    filtro.CodigoEtiqueta,
                    filtro.Observacao,
                    filtro.ObterTraducaoOrdenacao(),
                    filtro.ObterPrimeiroRegistroRetornar(),
                    filtro.NumeroRegistros);

                return this.ListaPaginada(
                    retalhosProducao.Select(rp => new ListaDto(rp)),
                    filtro,
                    () => RetalhoProducaoDAO.Instance.ObterCount(
                        filtro.CodigoProduto,
                        filtro.DescricaoProduto,
                        filtro.PeriodoCadastroInicio?.ToShortDateString(),
                        filtro.PeriodoCadastroFim?.ToShortDateString(),
                        filtro.PeriodoUsoInicio?.ToShortDateString(),
                        filtro.PeriodoUsoFim?.ToShortDateString(),
                        filtro.Situacao,
                        filtro.IdsCorVidro != null && filtro.IdsCorVidro.Any() ? string.Join(",", filtro.IdsCorVidro) : string.Empty,
                        (double)(filtro.Espessura ?? 0),
                        (double)(filtro.AlturaInicio ?? 0),
                        (double)(filtro.AlturaFim ?? 0),
                        (double)(filtro.LarguraInicio ?? 0),
                        (double)(filtro.LarguraFim ?? 0),
                        filtro.CodigoEtiqueta,
                        filtro.Observacao));
            }
        }

        /// <summary>
        /// Recupera a lista de situações de retalhos de produção.
        /// </summary>
        /// <returns>Uma lista JSON com as situações dos retalhos de produção.</returns>
        [HttpGet]
        [Route("situacoes")]
        [SwaggerResponse(200, "Situações encontradas.", Type = typeof(IEnumerable<IdNomeDto>))]
        [SwaggerResponse(204, "Situações não encontradas.")]
        public IHttpActionResult ObterSituacoesRetalhoProducao()
        {
            var situacoesRetalhosProducao = new Helper.ConversorEnum<SituacaoRetalhoProducao>()
                .ObterTraducao();

            return this.Lista(situacoesRetalhosProducao);
        }
    }
}