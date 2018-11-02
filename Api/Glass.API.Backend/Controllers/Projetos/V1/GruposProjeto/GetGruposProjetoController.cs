// <copyright file="GetGruposProjetoController.cs" company="Sync Softwares">
// Copyright (c) Sync Softwares. Todos os direitos reservados.
// </copyright>

using GDA;
using Glass.API.Backend.Models.Genericas.V1;
using Glass.Data.DAL;
using Swashbuckle.Swagger.Annotations;
using System.Collections.Generic;
using System.Linq;
using System.Web.Http;

namespace Glass.API.Backend.Controllers.Projetos.V1.GruposProjeto
{
    /// <summary>
    /// Controller de grupos de projeto.
    /// </summary>
    public partial class GruposProjetoController : BaseController
    {
        /// <summary>
        /// Recupera as situações de grupo de projeto para os controles de filtro das telas.
        /// </summary>
        /// <returns>Uma lista JSON com os itens encontrados.</returns>
        [HttpGet]
        [Route("filtro")]
        [SwaggerResponse(200, "Situações de grupo de projeto encontradas.", Type = typeof(IEnumerable<IdNomeDto>))]
        [SwaggerResponse(204, "Situações de grupo de projeto não encontradas.")]
        public IHttpActionResult ObterFiltro()
        {
            using (var sessao = new GDATransaction())
            {
                var grupos = GrupoModeloDAO.Instance.GetOrdered()
                    .Select(f => new IdNomeDto
                    {
                        Id = (int)f.IdGrupoModelo,
                        Nome = f.Descricao,
                    });

                return this.Lista(grupos);
            }
        }
    }
}
