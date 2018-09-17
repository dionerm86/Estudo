// <copyright file="GetCoresFerragemController.cs" company="Sync Softwares">
// Copyright (c) Sync Softwares. Todos os direitos reservados.
// </copyright>

using GDA;
using Glass.API.Backend.Models.Genericas;
using Glass.Data.DAL;
using Swashbuckle.Swagger.Annotations;
using System.Collections.Generic;
using System.Linq;
using System.Web.Http;

namespace Glass.API.Backend.Controllers.Produtos.V1.CoresFerragem
{
    /// <summary>
    /// Controller de cores de ferragem.
    /// </summary>
    public partial class CoresFerragemController : BaseController
    {
        /// <summary>
        /// Recupera as cores de ferragem para os controles de filtro das telas.
        /// </summary>
        /// <returns>Uma lista JSON com as cores de ferragem encontradas.</returns>
        [HttpGet]
        [Route("filtro")]
        [SwaggerResponse(200, "Cores de ferragem encontradas.", Type = typeof(IEnumerable<IdNomeDto>))]
        [SwaggerResponse(204, "Cores de ferragem não encontradas.")]
        public IHttpActionResult ObterCoresFerragemParaFiltro()
        {
            using (var sessao = new GDATransaction())
            {
                var coresFerragem = CorFerragemDAO.Instance.GetAll()
                    .Select(c => new IdNomeDto
                    {
                        Id = c.IdCorFerragem,
                        Nome = c.Descricao,
                    });

                return this.Lista(coresFerragem);
            }
        }
    }
}
