// <copyright file="GetRotasController.cs" company="Sync Softwares">
// Copyright (c) Sync Softwares. Todos os direitos reservados.
// </copyright>

using GDA;
using Glass.API.Backend.Models.Genericas;
using Glass.Data.DAL;
using Swashbuckle.Swagger.Annotations;
using System.Collections.Generic;
using System.Linq;
using System.Web.Http;

namespace Glass.API.Backend.Controllers.Rotas.V1
{
    /// <summary>
    /// Controller de rotas.
    /// </summary>
    public partial class RotasController : BaseController
    {
        /// <summary>
        /// Recupera as rotas para os controles de filtro das telas.
        /// </summary>
        /// <param name="id">O identificador da rota.</param>
        /// <param name="codigo">O código da rota.</param>
        /// <returns>Uma lista JSON com as rotas encontradas.</returns>
        [HttpGet]
        [Route("filtro")]
        [SwaggerResponse(200, "Rotas encontradas.", Type = typeof(IEnumerable<IdNomeDto>))]
        [SwaggerResponse(204, "Rotas não encontradas.")]
        public IHttpActionResult ObterFiltro(int? id = null, string codigo = null)
        {
            using (var sessao = new GDATransaction())
            {
                var rotas = RotaDAO.Instance.ObtemAtivasPorIdCodigo(id, codigo)
                    .Select(p => new IdNomeDto
                    {
                        Id = (int)p.IdRota,
                        Nome = p.CodInterno,
                    });

                return this.Lista(rotas);
            }
        }
    }
}
