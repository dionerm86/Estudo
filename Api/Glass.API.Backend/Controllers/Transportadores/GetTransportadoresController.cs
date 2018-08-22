// <copyright file="GetTransportadoresController.cs" company="Sync Softwares">
// Copyright (c) Sync Softwares. Todos os direitos reservados.
// </copyright>

using GDA;
using Glass.API.Backend.Models.Genericas;
using Glass.Data.DAL;
using Swashbuckle.Swagger.Annotations;
using System.Collections.Generic;
using System.Linq;
using System.Web.Http;

namespace Glass.API.Backend.Controllers.Transportadores.V1
{
    /// <summary>
    /// Controller de transportadores.
    /// </summary>
    public partial class TransportadoresController : BaseController
    {
        /// <summary>
        /// Obtém uma lista com todos os transportadores
        /// </summary>
        /// <returns>Uma lista JSON com os dados básicos de todos transportadores</returns>
        [HttpGet]
        [Route("filtro")]
        [SwaggerResponse(200, "Transportadores encontrados.", Type = typeof(IEnumerable<IdNomeDto>))]
        [SwaggerResponse(204, "Transportadores não encontrados.")]
        public IHttpActionResult ObterTransportadores()
        {
            using (var sessao = new GDATransaction())
            {
                var transportadores = TransportadorDAO.Instance.GetOrdered()
                    .Select(f => new IdNomeDto
                    {
                        Id = f.IdTransportador,
                        Nome = f.Nome,
                    });

                return this.Lista(transportadores);
            }
        }
    }
}
