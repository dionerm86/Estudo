// <copyright file="GetRoteirosController.cs" company="Sync Softwares">
// Copyright (c) Sync Softwares. Todos os direitos reservados.
// </copyright>

using GDA;
using Glass.API.Backend.Helper.Respostas;
using Glass.Data.DAL;
using Swashbuckle.Swagger.Annotations;
using System.Collections.Generic;
using System.Linq;
using System.Web.Http;

namespace Glass.API.Backend.Controllers.Producao.V1.Roteiros
{
    /// <summary>
    /// Controller de roteiros.
    /// </summary>
    public partial class RoteirosController : BaseController
    {
        /// <summary>
        /// Recupera a lista de roteiros.
        /// </summary>
        /// <param name="filtro">Os filtros para a busca dos roteiros.</param>
        /// <returns>Uma lista JSON com os dados dos roteiros.</returns>
        [HttpGet]
        [Route("")]
        [SwaggerResponse(200, "Roteiros sem paginação (apenas uma página de retorno) ou última página retornada.", Type = typeof(IEnumerable<Models.Producao.V1.Roteiros.Lista.ListaDto>))]
        [SwaggerResponse(204, "Roteiros não encontrados para o filtro informado.")]
        [SwaggerResponse(206, "Roteiros paginados (qualquer página, exceto a última).", Type = typeof(IEnumerable<Models.Producao.V1.Roteiros.Lista.ListaDto>))]
        [SwaggerResponse(400, "Filtro inválido informado (campo com valor ou formato inválido).", Type = typeof(MensagemDto))]
        public IHttpActionResult ObterListaRoteiros([FromUri] Models.Producao.V1.Roteiros.Lista.FiltroDto filtro)
        {
            using (var sessao = new GDATransaction())
            {
                filtro = filtro ?? new Models.Producao.V1.Roteiros.Lista.FiltroDto();

                var roteiros = RoteiroProducaoDAO.Instance.ObtemLista(
                    0,
                    (uint)(filtro.IdGrupoProduto ?? 0),
                    (uint)(filtro.IdSubgrupoProduto ?? 0),
                    (uint)(filtro.IdProcesso ?? 0),
                    null,
                    0,
                    false,
                    filtro.ObterTraducaoOrdenacao(),
                    filtro.ObterPrimeiroRegistroRetornar(),
                    filtro.NumeroRegistros);

                return this.ListaPaginada(
                    roteiros.Select(r => new Models.Producao.V1.Roteiros.Lista.ListaDto(r)),
                    filtro,
                    () => RoteiroProducaoDAO.Instance.ObtemNumeroRegistros(
                        0,
                        (uint)(filtro.IdGrupoProduto ?? 0),
                        (uint)(filtro.IdSubgrupoProduto ?? 0),
                        (uint)(filtro.IdProcesso ?? 0),
                        null,
                        0,
                        false));
            }
        }
    }
}
