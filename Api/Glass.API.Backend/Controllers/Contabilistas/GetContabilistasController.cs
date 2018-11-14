// <copyright file="GetContabilistasController.cs" company="Sync Softwares">
// Copyright (c) Sync Softwares. Todos os direitos reservados.
// </copyright>

using GDA;
using Glass.API.Backend.Helper.Respostas;
using Glass.API.Backend.Models.Contabilistas.V1.Lista;
using Glass.Data.DAL;
using Swashbuckle.Swagger.Annotations;
using System.Collections.Generic;
using System.Linq;
using System.Web.Http;

namespace Glass.API.Backend.Controllers.Contabilistas.V1
{
    /// <summary>
    /// Controller de contabilistas.
    /// </summary>
    public partial class ContabilistasController : BaseController
    {
        /// <summary>
        /// Recupera a lista de contabilistas.
        /// </summary>
        /// <param name="filtro">Os filtros para a busca dos contabilistas.</param>
        /// <returns>Uma lista JSON com os dados dos contabilistas.</returns>
        [HttpGet]
        [Route("")]
        [SwaggerResponse(200, "Contabilistas sem paginação (apenas uma página de retorno) ou última página retornada.", Type = typeof(IEnumerable<ListaDto>))]
        [SwaggerResponse(204, "Contabilistas não encontrados para o filtro informado.")]
        [SwaggerResponse(206, "Contabilistas paginados (qualquer página, exceto a última).", Type = typeof(IEnumerable<ListaDto>))]
        [SwaggerResponse(400, "Filtro inválido informado (campo com valor ou formato inválido).", Type = typeof(MensagemDto))]
        public IHttpActionResult ObterListaContabilistas([FromUri] FiltroDto filtro)
        {
            using (var sessao = new GDATransaction())
            {
                filtro = filtro ?? new FiltroDto();

                var notasFiscais = ContabilistaDAO.Instance.GetList(
                    filtro.ObterTraducaoOrdenacao(),
                    filtro.ObterPrimeiroRegistroRetornar(),
                    filtro.NumeroRegistros);

                return this.ListaPaginada(
                    notasFiscais.Select(n => new ListaDto(n)),
                    filtro,
                    () => ContabilistaDAO.Instance.GetCount());
            }
        }
    }
}
