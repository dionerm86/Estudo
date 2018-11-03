// <copyright file="GetFabricantesFerragemController.cs" company="Sync Softwares">
// Copyright (c) Sync Softwares. Todos os direitos reservados.
// </copyright>

using GDA;
using Glass.API.Backend.Models.Genericas.V1;
using Swashbuckle.Swagger.Annotations;
using System.Collections.Generic;
using System.Linq;
using System.Web.Http;

namespace Glass.API.Backend.Controllers.Projetos.V1.Ferragens.FabricantesFerragem
{
    /// <summary>
    /// Controller de fabricantes de ferragem.
    /// </summary>
    public partial class FabricantesFerragemController : BaseController
    {
        /// <summary>
        /// Recupera os fabricantes de ferragem para os controles de filtro das telas.
        /// </summary>
        /// <returns>Uma lista JSON com os itens encontrados.</returns>
        [HttpGet]
        [Route("filtro")]
        [SwaggerResponse(200, "Fabricantes de ferragens encontrados.", Type = typeof(IEnumerable<IdNomeDto>))]
        [SwaggerResponse(204, "Fabricantes de ferragens não encontrados.")]
        public IHttpActionResult ObterFabricantes()
        {
            using (var sessao = new GDATransaction())
            {
                var itens = Microsoft.Practices.ServiceLocation.ServiceLocator
                    .Current.GetInstance<Projeto.Negocios.IFerragemFluxo>()
                    .ObterFabricantesFerragem()
                    .Select(f => new IdNomeDto
                    {
                        Id = f.Id,
                        Nome = f.Name,
                    });

                return this.Lista(itens);
            }
        }
    }
}
