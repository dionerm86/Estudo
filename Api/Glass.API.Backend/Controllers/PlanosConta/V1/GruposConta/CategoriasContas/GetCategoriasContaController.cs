// <copyright file="GetCategoriasContaController.cs" company="Sync Softwares">
// Copyright (c) Sync Softwares. Todos os direitos reservados.
// </copyright>

using GDA;
using Glass.API.Backend.Models.Genericas.V1;
using Swashbuckle.Swagger.Annotations;
using System.Collections.Generic;
using System.Linq;
using System.Web.Http;

namespace Glass.API.Backend.Controllers.PlanosConta.V1.GruposConta.CategoriasConta
{
    /// <summary>
    /// Controller de categorias de conta.
    /// </summary>
    public partial class CategoriasContaController : BaseController
    {
        /// <summary>
        /// Recupera as categorias de conta para os controles de filtro das telas.
        /// </summary>
        /// <returns>Uma lista JSON com as categorias de conta encontradas.</returns>
        [HttpGet]
        [Route("filtro")]
        [SwaggerResponse(200, "Categorias de conta encontradas.", Type = typeof(IEnumerable<IdNomeDto>))]
        [SwaggerResponse(204, "Categorias de conta não encontradas.")]
        public IHttpActionResult ObterCategoriasContaParaFiltro()
        {
            using (var sessao = new GDATransaction())
            {
                var gruposConta = Microsoft.Practices.ServiceLocation.ServiceLocator
                    .Current.GetInstance<Financeiro.Negocios.IPlanoContasFluxo>()
                    .ObtemCategoriasConta(true)
                    .Select(g => new IdNomeDto
                    {
                        Id = g.Id,
                        Nome = g.Name,
                    });

                return this.Lista(gruposConta);
            }
        }
    }
}
