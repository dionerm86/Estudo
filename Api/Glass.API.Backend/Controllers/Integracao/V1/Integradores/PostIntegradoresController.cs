﻿// <copyright file="PostIntegradoresController.cs" company="Sync Softwares">
// Copyright (c) Sync Softwares. Todos os direitos reservados.
// </copyright>

using Glass.API.Backend.Helper.Respostas;
using Swashbuckle.Swagger.Annotations;
using System;
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

        /// <summary>
        /// Executa o job associado com o integrador.
        /// </summary>
        /// <param name="integrador">Nome do integrador no qual o Job está associado.</param>
        /// <param name="job">Nome do job que será executado.</param>
        /// <returns>Ok quando o Job for executado com sucesso.</returns>
        [HttpPost]
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


            if (job1.Situacao != Glass.Integracao.SituacaoJobIntegracao.Executando)
            {
                try
                {
                    job1.Executar();
                }
                catch (Exception ex)
                {
                    return this.ErroInternoServidor("Ocorreu um problema na execução do job.", ex);
                }
            }
            else
            {
                return this.ErroInternoServidor("O job já está em execução.");
            }

            return this.Ok();
        }

        /// <summary>
        /// Obtém os itens de histórico do integrador.
        /// </summary>
        /// <param name="integrador">Nome do integrador.</param>
        /// <param name="itemEsquema">Item do esquema de histórico que será usado como filtro.</param>
        /// <param name="filtro">Filtro que será usado na consulta.</param>
        /// <returns>JSON com os dados dos itens do histórico.</returns>
        [HttpPost]
        [Route("{integrador}/historico/{itemEsquema}/itens")]
        [SwaggerResponse(200, "Itens do histórico..", Type = typeof(string))]
        [SwaggerResponse(404, "Quando o integrador ou o item do esquena não forem encontrados.", Type = typeof(MensagemDto))]
        public IHttpActionResult ObterItensHistorico(
            string integrador,
            string itemEsquema,
            [FromBody]Models.Integracao.V1.Integradores.Lista.ItensHistoricoFiltroDto filtro)
        {
            var integrador1 = this.GerenciadorIntegradores.Integradores
                .FirstOrDefault(f => StringComparer.InvariantCultureIgnoreCase.Equals(f.Nome, integrador));

            if (integrador1 == null)
            {
                return this.NaoEncontrado($"Não foi possível encontrar o integrador '{integrador}'.");
            }

            var itemEsquema1 = integrador1.EsquemaHistorico.Itens.FirstOrDefault(f => StringComparer.InvariantCultureIgnoreCase.Equals(f.Nome, itemEsquema));

            if (itemEsquema1 == null)
            {
                return this.NaoEncontrado($"Não foi possível encontrar o item '{itemEsquema}' do esquema de histórico do integrador '{integrador}'.");
            }

            var itens = this.ProvedorHistorico.ObterItens(itemEsquema1, filtro?.Tipo, filtro?.Identificadores);

            return this.Json(itens.Take(10).Select(item => new Models.Integracao.V1.Integradores.Lista.ItemHistoricoDto(item)));
        }
    }
}
