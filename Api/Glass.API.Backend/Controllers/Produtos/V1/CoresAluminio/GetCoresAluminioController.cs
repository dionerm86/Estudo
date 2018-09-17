// <copyright file="GetCoresAluminioController.cs" company="Sync Softwares">
// Copyright (c) Sync Softwares. Todos os direitos reservados.
// </copyright>

using GDA;
using Glass.API.Backend.Models.Genericas;
using Glass.Data.DAL;
using Swashbuckle.Swagger.Annotations;
using System.Collections.Generic;
using System.Linq;
using System.Web.Http;

namespace Glass.API.Backend.Controllers.Produtos.V1.CoresAluminio
{
    /// <summary>
    /// Controller de cores de alumínio.
    /// </summary>
    public partial class CoresAluminioController : BaseController
    {
        /// <summary>
        /// Recupera as cores de alumínio para os controles de filtro das telas.
        /// </summary>
        /// <returns>Uma lista JSON com as cores de alumínio encontradas.</returns>
        [HttpGet]
        [Route("filtro")]
        [SwaggerResponse(200, "Cores de alumínio encontradas.", Type = typeof(IEnumerable<IdNomeDto>))]
        [SwaggerResponse(204, "Cores de alumínio não encontradas.")]
        public IHttpActionResult ObterCoresAluminioParaFiltro()
        {
            using (var sessao = new GDATransaction())
            {
                var coresAluminio = CorAluminioDAO.Instance.GetAll()
                    .Select(c => new IdNomeDto
                    {
                        Id = c.IdCorAluminio,
                        Nome = c.Descricao,
                    });

                return this.Lista(coresAluminio);
            }
        }
    }
}
