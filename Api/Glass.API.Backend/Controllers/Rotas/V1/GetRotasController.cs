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
        /// <returns>Uma lista JSON com as rotas encontradas.</returns>
        [HttpGet]
        [Route("filtro")]
        [SwaggerResponse(200, "Rotas encontradas.", Type = typeof(IEnumerable<IdNomeDto>))]
        [SwaggerResponse(204, "Rotas não encontradas.")]
        public IHttpActionResult ObterFiltro()
        {
            using (var sessao = new GDATransaction())
            {
                var rotas = RotaDAO.Instance.ObtemAtivas()
                    .Select(p => new IdNomeDto
                    {
                        Id = (int)p.IdRota,
                        Nome = p.Descricao,
                    });

                return this.Lista(rotas);
            }
        }
    }
}
