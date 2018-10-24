// <copyright file="GetGruposContaController.cs" company="Sync Softwares">
// Copyright (c) Sync Softwares. Todos os direitos reservados.
// </copyright>

using GDA;
using Glass.API.Backend.Models.Genericas.V1;
using Swashbuckle.Swagger.Annotations;
using System.Collections.Generic;
using System.Linq;
using System.Web.Http;

namespace Glass.API.Backend.Controllers.PlanosConta.V1.GruposConta
{
    /// <summary>
    /// Controller de grupos de conta.
    /// </summary>
    public partial class GruposContaController : BaseController
    {
        /// <summary>
        /// Recupera os grupos de conta para os controles de filtro das telas.
        /// </summary>
        /// <returns>Uma lista JSON com os grupos de conta encontrados.</returns>
        [HttpGet]
        [Route("filtro")]
        [SwaggerResponse(200, "grupos de conta encontrados.", Type = typeof(IEnumerable<IdNomeDto>))]
        [SwaggerResponse(204, "grupos de conta não encontrados.")]
        public IHttpActionResult ObterGruposContaParaFiltro()
        {
            using (var sessao = new GDATransaction())
            {
                var gruposConta = Microsoft.Practices.ServiceLocation.ServiceLocator
                    .Current.GetInstance<Financeiro.Negocios.IPlanoContasFluxo>()
                    .ObtemGruposContaCadastro()
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
