// <copyright file="PutIntegradoresController.cs" company="Sync Softwares">
// Copyright (c) Sync Softwares. Todos os direitos reservados.
// </copyright>

using Glass.API.Backend.Helper.Respostas;
using Swashbuckle.Swagger.Annotations;
using System;
using System.Linq;
using System.Web.Http;

namespace Glass.API.Backend.Controllers.Integracao.V1.Integradores
{
    /// <summary>
    /// Controller de integradores.
    /// </summary>
    public partial class IntegradoresController
    {
        /// <summary>
        /// Executa o job associado com o integrador.
        /// </summary>
        /// <param name="integrador">Nome do integrador no qual o Job está associado.</param>
        /// <param name="job">Nome do job que será executado.</param>
        /// <returns>Ok quando o Job for executado com sucesso.</returns>
        [HttpPut]
        [Route("{integrador}/executarJob/{job}")]
        [SwaggerResponse(200, "Job executado.", Type = typeof(string))]
        [SwaggerResponse(404, "Quando o integrador ou o job não forem encontrados.", Type = typeof(MensagemDto))]
        [SwaggerResponse(500, "Erro na execução.", Type = typeof(MensagemDto))]
        public IHttpActionResult ExecutarJob(string integrador, string job)
        {
            var integrador1 = this.GerenciadorIntegradores.Integradores
                .FirstOrDefault(f => StringComparer.InvariantCultureIgnoreCase.Equals(f.Nome, integrador));

            if (integrador1 == null)
            {
                return this.NaoEncontrado($"Não foi possível encontrar o integrador '{integrador}'.");
            }

            var job1 = integrador1.Jobs.FirstOrDefault(f => StringComparer.InvariantCultureIgnoreCase.Equals(f.Nome, job));

            if (job1 == null)
            {
                return this.NaoEncontrado($"Não foi possível encontrar o job '{job}'.");
            }

            try
            {
                job1.Executar();
            }
            catch (Exception ex)
            {
                return this.ErroInternoServidor("Ocorreu um problema na execução do job.", ex);
            }

            return this.Ok();
        }
    }
}
