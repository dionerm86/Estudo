// <copyright file="PostIntegradoresController.cs" company="Sync Softwares">
// Copyright (c) Sync Softwares. Todos os direitos reservados.
// </copyright>

using Glass.API.Backend.Helper.Respostas;
using Swashbuckle.Swagger.Annotations;
using System.Linq;
using System.Threading.Tasks;
using System.Web.Http;

namespace Glass.API.Backend.Controllers.Integracao.V1.Integradores
{
    /// <summary>
    /// Controller de integradores.
    /// </summary>
    public partial class IntegradoresController
    {
        /// <summary>
        /// Executa a operação de integração.
        /// </summary>
        /// <param name="integrador">Nome do integrador onde a operação será executada.</param>
        /// <param name="dadosExecucao">Os dados da operação que será executada.</param>
        /// <returns>Json com o resultado da execução..</returns>
        [HttpPost]
        [Route("{integrador}/executarOperacao")]
        [SwaggerResponse(200, "Operação executada.", Type = typeof(string))]
        [SwaggerResponse(500, "Erro na execução.", Type = typeof(MensagemDto))]
        public async Task<IHttpActionResult> ExecutarOperacao(string integrador, [FromBody] Models.Integracao.V1.Integradores.ExecucaoOperacaoIntegracao.DadosExecucaoDto dadosExecucao)
        {
            var resultado = await this.GerenciadorIntegradores.ExecutarOperacao(
                integrador,
                dadosExecucao?.Operacao,
                dadosExecucao?.Parametros.OfType<object>().ToArray());

            return this.Json(resultado);
        }
    }
}
