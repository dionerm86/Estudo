// <copyright file="GetCoresVidroController.cs" company="Sync Softwares">
// Copyright (c) Sync Softwares. Todos os direitos reservados.
// </copyright>

using GDA;
using Glass.API.Backend.Models.Genericas;
using Glass.Data.DAL;
using Swashbuckle.Swagger.Annotations;
using System.Collections.Generic;
using System.Linq;
using System.Web.Http;

namespace Glass.API.Backend.Controllers.Produtos.V1.CoresVidro
{
    /// <summary>
    /// Controller de cores de vidro.
    /// </summary>
    public partial class CoresVidroController : BaseController
    {
        /// <summary>
        /// Recupera as cores de vidro para os controles de filtro das telas.
        /// </summary>
        /// <returns>Uma lista JSON com as cores de vidro encontradas.</returns>
        [HttpGet]
        [Route("filtro")]
        [SwaggerResponse(200, "Cores de vidro encontradas.", Type = typeof(IEnumerable<IdNomeDto>))]
        [SwaggerResponse(204, "Cores de vidro não encontradas.")]
        public IHttpActionResult ObterCoresVidroParaFiltro()
        {
            using (var sessao = new GDATransaction())
            {
                var coresVidro = CorVidroDAO.Instance.GetForFiltro()
                    .Select(c => new IdNomeDto
                    {
                        Id = c.IdCorVidro,
                        Nome = c.Descricao,
                    });

                return this.Lista(coresVidro);
            }
        }
    }
}
