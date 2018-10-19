// <copyright file="GetCondutoresController.cs" company="Sync Softwares">
// Copyright (c) Sync Softwares. Todos os direitos reservados.
// </copyright>

using GDA;
using Glass.API.Backend.Helper;
using Glass.API.Backend.Helper.Respostas;
using Glass.API.Backend.Models.Condutores.Lista;
using Glass.API.Backend.Models.Genericas;
using Glass.Data.DAL;
using Swashbuckle.Swagger.Annotations;
using System.Collections.Generic;
using System.Linq;
using System.Web.Http;

namespace Glass.API.Backend.Controllers.Condutores.V1
{
    /// <summary>
    /// Controller de condutores.
    /// </summary>
    public partial class CondutoresController : BaseController
    {
        /// <summary>
        /// Recupera a lista de condutores para a tela de listagem.
        /// </summary>
        /// <param name="filtro">O filtro para a busca de condutores.</param>
        /// <returns>Uma lista JSON com os dados dos condutores.</returns>
        [HttpGet]
        [Route("")]
        [SwaggerResponse(200, "Condutores encontrados sem paginação (apenas uma página de retorno) ou última página retornada.", Type = typeof(IEnumerable<ListaDto>))]
        [SwaggerResponse(204, "Condutores não encontrados para o filtro informado.")]
        [SwaggerResponse(206, "Condutores paginados (qualquer página, exceto a última).", Type = typeof(IEnumerable<ListaDto>))]
        public IHttpActionResult ObterLista([FromUri] FiltroDto filtro)
        {
            using (var sessao = new GDATransaction())
            {
                var condutores = CondutoresDAO.Instance.GetList();

                return this.ListaPaginada(
                    condutores
                        .Skip(filtro.ObterPrimeiroRegistroRetornar())
                        .Take(filtro.NumeroRegistros)
                        .Select(dao => new ListaDto(dao)),
                    filtro,
                    () => condutores.Count);
            }
        }
    }
}
