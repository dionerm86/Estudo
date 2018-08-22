// <copyright file="GetPublicoController.cs" company="Sync Softwares">
// Copyright (c) Sync Softwares. Todos os direitos reservados.
// </copyright>

using Swashbuckle.Swagger.Annotations;
using System.Web.Http;

namespace Glass.API.Backend.Controllers.Publico.V1
{
    /// <summary>
    /// Controller público.
    /// </summary>
    public partial class PublicoController : BaseController
    {
        /// <summary>
        /// Verifica a disponibilidade dos métodos da API.
        /// </summary>
        /// <returns>Um status HTTP que indica que a API está carregada.</returns>
        [HttpGet]
        [Route("disponibilidade")]
        [SwaggerResponse(200, "API responde sem problemas.")]
        public IHttpActionResult VerificaDisponibilidade()
        {
            return this.Ok();
        }
    }
}
