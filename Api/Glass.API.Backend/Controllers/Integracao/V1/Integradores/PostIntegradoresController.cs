// <copyright file="PostIntegradoresController.cs" company="Sync Softwares">
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

            Glass.Integracao.Historico.TipoItemHistorico? tipo = null;
            Glass.Integracao.Historico.TipoItemHistorico tipo2;

            if (!string.IsNullOrEmpty(filtro?.Tipo) &&
                Enum.TryParse<Glass.Integracao.Historico.TipoItemHistorico>(filtro.Tipo, out tipo2))
            {
                tipo = tipo2;
            }

            var itens = this.ProvedorHistorico.ObterItens(itemEsquema1, tipo, filtro?.Identificadores);

            return this.Json(itens.Take(10).Select(item => new Models.Integracao.V1.Integradores.Lista.ItemHistoricoDto(item)));
        }
    }
}
