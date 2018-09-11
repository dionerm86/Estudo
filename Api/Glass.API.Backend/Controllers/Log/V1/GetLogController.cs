// <copyright file="GetLogController.cs" company="Sync Softwares">
// Copyright (c) Sync Softwares. Todos os direitos reservados.
// </copyright>

using GDA;
using Glass.API.Backend.Helper.Respostas;
using Glass.API.Backend.Models.Genericas;
using Glass.API.Backend.Models.Log.Alteracao;
using Glass.API.Backend.Models.Log.Cancelamento;
using Glass.Data.DAL;
using Glass.Data.Helper;
using Glass.Data.Model;
using Swashbuckle.Swagger.Annotations;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Http;
using System.Web.Http.Description;

namespace Glass.API.Backend.Controllers.Log.V1
{
    /// <summary>
    /// Controller de logs do sistema.
    /// </summary>
    public partial class LogController : BaseController
    {
        /// <summary>
        /// Recupera os dados de logs de alteração para um determinado item e tabela.
        /// </summary>
        /// <param name="filtro">Os parâmetros de entrada para a consulta.</param>
        /// <returns>Uma lista JSON com os logs de alteração encontrados.</returns>
        [HttpGet]
        [Route("alteracao/{tabela}/logs/{idItem}")]
        [SwaggerResponse(200, "Logs sem paginação (apenas uma página de retorno) ou última página retornada.", Type = typeof(IEnumerable<Models.Log.Alteracao.LogDto>))]
        [SwaggerResponse(204, "Logs não encontrados para o filtro informado.")]
        [SwaggerResponse(206, "Logs paginados (qualquer página, exceto a última).", Type = typeof(IEnumerable<Models.Log.Alteracao.LogDto>))]
        [SwaggerResponse(400, "Filtro inválido informado (campo com valor ou formato inválido).", Type = typeof(MensagemDto))]
        [ResponseType(typeof(IEnumerable<Models.Log.Alteracao.LogDto>))]
        public IHttpActionResult ObterLogsAlteracao([FromUri] FiltroLogAlteracaoDto filtro)
        {
            using (var sessao = new GDATransaction())
            {
                var validacao = this.ValidarTabelaAlteracao(filtro.Tabela);

                if (validacao != null)
                {
                    return validacao;
                }

                filtro = filtro ?? new FiltroLogAlteracaoDto();
                bool exibirAdmin = UserInfo.GetUserInfo.IsAdministrador;

                var logs = LogAlteracaoDAO.Instance.GetList(
                    (int)filtro.Tabela.Value,
                    (uint)filtro.IdItem,
                    exibirAdmin,
                    filtro.Campo,
                    filtro.ObterTraducaoOrdenacao(),
                    filtro.ObterPrimeiroRegistroRetornar(),
                    filtro.NumeroRegistros);

                return this.ListaPaginada(
                    logs.Select(log => new Models.Log.Alteracao.LogDto(log)),
                    filtro,
                    () => LogAlteracaoDAO.Instance.GetCount(
                        (int)filtro.Tabela.Value,
                        (uint)filtro.IdItem,
                        exibirAdmin,
                        filtro.Campo));
            }
        }

        /// <summary>
        /// Verifica se um item possui logs de alteração.
        /// </summary>
        /// <param name="tabela">A tabela do item a ser verificado.</param>
        /// <param name="idItem">O identificador do item a ser verificado.</param>
        /// <param name="campo">O campo que foi alterado no log (opcional).</param>
        /// <returns>Um status HTTP que indica se existem ou não logs para o item/tabela informados.</returns>
        [HttpGet]
        [Route("alteracao/{tabela}/possuiLog/{idItem}")]
        [SwaggerResponse(200, "Existem logs para o item informado.")]
        [SwaggerResponse(400, "Filtro inválido informado (campo com valor ou formato inválido).", Type = typeof(MensagemDto))]
        [SwaggerResponse(404, "Não existem logs para o item informado.", Type = typeof(MensagemDto))]
        public IHttpActionResult PossuiLogAlteracao(LogAlteracao.TabelaAlteracao? tabela, int idItem, string campo = null)
        {
            using (var sessao = new GDATransaction())
            {
                var validacao = this.ValidarTabelaAlteracao(tabela);

                if (validacao != null)
                {
                    return validacao;
                }

                return LogAlteracaoDAO.Instance.TemRegistro(tabela.Value, (uint)idItem, campo)
                    ? (IHttpActionResult)this.Ok()
                    : (IHttpActionResult)this.NaoEncontrado("Não há registros para o item/tabela informados.");
            }
        }

        /// <summary>
        /// Recupera a lista de tabelas passíveis de log de alterações do sistema.
        /// </summary>
        /// <returns>Uma lista JSON com os dados básicos das tabelas.</returns>
        [HttpGet]
        [Route("alteracao/tabelas")]
        [SwaggerResponse(200, "Tabelas possíveis para log encontradas.", Type = typeof(IEnumerable<IdNomeDto>))]
        public IHttpActionResult ObterTabelasLogAlteracao()
        {
            using (var sessao = new GDATransaction())
            {
                var tabelas = Enum.GetValues(typeof(LogAlteracao.TabelaAlteracao))
                    .Cast<LogAlteracao.TabelaAlteracao>()
                    .Select(tabela => new IdNomeDto
                    {
                        Id = (int)tabela,
                        Nome = tabela.ToString(),
                    });

                return this.Lista(tabelas);
            }
        }

        /// <summary>
        /// Recupera os dados de logs de cancelamento para um determinado item e tabela.
        /// </summary>
        /// <param name="filtro">Os parâmetros de entrada para a consulta.</param>
        /// <returns>Uma lista JSON com os logs de cancelamento encontrados.</returns>
        [HttpGet]
        [Route("cancelamento/{tabela}/logs/{idItem}")]
        [SwaggerResponse(200, "Logs sem paginação (apenas uma página de retorno) ou última página retornada.", Type = typeof(IEnumerable<Models.Log.Cancelamento.LogDto>))]
        [SwaggerResponse(204, "Logs não encontrados para o filtro informado.")]
        [SwaggerResponse(206, "Logs paginados (qualquer página, exceto a última).", Type = typeof(IEnumerable<Models.Log.Cancelamento.LogDto>))]
        [SwaggerResponse(400, "Filtro inválido informado (campo com valor ou formato inválido).", Type = typeof(MensagemDto))]
        [ResponseType(typeof(IEnumerable<Models.Log.Cancelamento.LogDto>))]
        public IHttpActionResult ObterLogsCancelamento([FromUri] FiltroLogCancelamentoDto filtro)
        {
            using (var sessao = new GDATransaction())
            {
                var validacao = this.ValidarTabelaCancelamento(filtro.Tabela);

                if (validacao != null)
                {
                    return validacao;
                }

                filtro = filtro ?? new FiltroLogCancelamentoDto();
                bool exibirAdmin = UserInfo.GetUserInfo.IsAdministrador;

                var logs = LogCancelamentoDAO.Instance.GetList(
                    (int)filtro.Tabela.Value,
                    (uint)filtro.IdItem,
                    exibirAdmin,
                    filtro.Campo,
                    (uint)(filtro.IdFuncionarioCancelamento ?? 0),
                    filtro.PeriodoCancelamentoInicio?.ToShortDateString(),
                    filtro.PeriodoCancelamentoFim?.ToShortDateString(),
                    filtro.Valor,
                    filtro.ObterTraducaoOrdenacao(),
                    filtro.ObterPrimeiroRegistroRetornar(),
                    filtro.NumeroRegistros);

                return this.ListaPaginada(
                    logs.Select(log => new Models.Log.Cancelamento.LogDto(log)),
                    filtro,
                    () => LogCancelamentoDAO.Instance.GetCount(
                        (int)filtro.Tabela.Value,
                        (uint)filtro.IdItem,
                        exibirAdmin,
                        filtro.Campo,
                        (uint)(filtro.IdFuncionarioCancelamento ?? 0),
                        filtro.PeriodoCancelamentoInicio?.ToShortDateString(),
                        filtro.PeriodoCancelamentoFim?.ToShortDateString(),
                        filtro.Valor));
            }
        }

        /// <summary>
        /// Verifica se um item possui logs de cancelamento.
        /// </summary>
        /// <param name="tabela">A tabela do item a ser verificado.</param>
        /// <param name="idItem">O identificador do item a ser verificado.</param>
        /// <param name="campo">O campo que foi alterado no log (opcional).</param>
        /// <returns>Um status HTTP que indica se existem ou não logs para o item/tabela informados.</returns>
        [HttpGet]
        [Route("cancelamento/{tabela}/possuiLog/{idItem}")]
        [SwaggerResponse(200, "Existem logs para o item informado.")]
        [SwaggerResponse(400, "Filtro inválido informado (campo com valor ou formato inválido).", Type = typeof(MensagemDto))]
        [SwaggerResponse(404, "Não existem logs para o item informado.", Type = typeof(MensagemDto))]
        public IHttpActionResult PossuiLogCancelamento(LogCancelamento.TabelaCancelamento? tabela, int idItem, string campo = null)
        {
            using (var sessao = new GDATransaction())
            {
                var validacao = this.ValidarTabelaCancelamento(tabela);

                if (validacao != null)
                {
                    return validacao;
                }

                return LogCancelamentoDAO.Instance.TemRegistro(tabela.Value, (uint)idItem)
                    ? (IHttpActionResult)this.Ok()
                    : (IHttpActionResult)this.NaoEncontrado("Não há registros para o item/tabela informados.");
            }
        }

        /// <summary>
        /// Recupera a lista de tabelas passíveis de log de cancelamento do sistema.
        /// </summary>
        /// <returns>Uma lista JSON com os dados básicos das tabelas.</returns>
        [HttpGet]
        [Route("cancelamento/tabelas")]
        [SwaggerResponse(200, "Tabelas possíveis para log encontradas.", Type = typeof(IEnumerable<IdNomeDto>))]
        public IHttpActionResult ObterTabelasLogCancelamento()
        {
            using (var sessao = new GDATransaction())
            {
                var tabelas = Enum.GetValues(typeof(LogCancelamento.TabelaCancelamento))
                    .Cast<LogCancelamento.TabelaCancelamento>()
                    .Select(tabela => new IdNomeDto
                    {
                        Id = (int)tabela,
                        Nome = tabela.ToString(),
                    });

                return this.Lista(tabelas);
            }
        }
    }
}
