// <copyright file="GetIntegradoresController.cs" company="Sync Softwares">
// Copyright (c) Sync Softwares. Todos os direitos reservados.
// </copyright>

using Glass.API.Backend.Helper.Respostas;
using Swashbuckle.Swagger.Annotations;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Http;

namespace Glass.API.Backend.Controllers.Integracao.V1.Integradores
{
    /// <summary>
    /// Controller de integradores.
    /// </summary>
    public partial class IntegradoresController : BaseController
    {
        /// <summary>
        /// Obtém a lista de integradores.
        /// </summary>
        /// <returns>Uma lista JSON com os dados dos produtos.</returns>
        [HttpGet]
        [Route("")]
        [SwaggerResponse(200, "Integradores.", Type = typeof(IEnumerable<Models.Integracao.V1.Integradores.Lista.IntegradorDto>))]
        public IHttpActionResult ObterListaIntegradores()
        {
            var integradores = this.GerenciadorIntegradores.Integradores
                .Select(c => new Models.Integracao.V1.Integradores.Lista.IntegradorDto(c));

            return this.Json(integradores);
        }

        /// <summary>
        /// Obtém o logger do integrador.
        /// </summary>
        /// <param name="integrador">Nome do integrador de onde será recuperado o logger.</param>
        /// <returns>JSON com os dados do logger.</returns>
        [HttpGet]
        [Route("{integrador}/logger")]
        [SwaggerResponse(200, "Logger do integrador.", Type = typeof(string))]
        [SwaggerResponse(404, "Quando o integrador não for encontrado.", Type = typeof(MensagemDto))]
        public IHttpActionResult ObterLogger(string integrador)
        {
            var integrador1 = this.GerenciadorIntegradores.Integradores
                .FirstOrDefault(f => StringComparer.InvariantCultureIgnoreCase.Equals(f.Nome, integrador));

            if (integrador1 == null)
            {
                return this.NaoEncontrado($"Não foi possível encontrar o integrador '{integrador}'.");
            }

            var logger = new Models.Integracao.V1.Integradores.Logger.LoggerIntegracaoDto(integrador1.Logger);
            return this.Json(logger);
        }
    }
}
