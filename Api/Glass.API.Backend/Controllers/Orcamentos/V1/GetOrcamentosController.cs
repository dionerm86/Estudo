// <copyright file="GetOrcamentosController.cs" company="Sync Softwares">
// Copyright (c) Sync Softwares. Todos os direitos reservados.
// </copyright>

using GDA;
using Glass.API.Backend.Helper.Respostas;
using Glass.API.Backend.Models.Genericas;
using Glass.Data.DAL;
using Glass.Data.Helper;
using Swashbuckle.Swagger.Annotations;
using System.Collections.Generic;
using System.Linq;
using System.Web.Http;

namespace Glass.API.Backend.Controllers.Orcamentos.V1
{
    /// <summary>
    /// Controller de orçamentos.
    /// </summary>
    public partial class OrcamentosController : BaseController
    {
        /// <summary>
        /// Recupera as configurações usadas pela tela de listagem de orçamentos.
        /// </summary>
        /// <returns>Um objeto JSON com as configurações da tela.</returns>
        [HttpGet]
        [Route("configuracoes")]
        [SwaggerResponse(200, "Configurações recuperadas.", Type = typeof(Models.Orcamentos.Configuracoes.ListaDto))]
        public IHttpActionResult ObterConfiguracoesListaOrcamentos()
        {
            using (var sessao = new GDATransaction())
            {
                var configuracoes = new Models.Orcamentos.Configuracoes.ListaDto();
                return this.Item(configuracoes);
            }
        }

        /// <summary>
        /// Recupera a lista de orçamentos.
        /// </summary>
        /// <param name="filtro">Os filtros para a busca dos orçamentos.</param>
        /// <returns>Uma lista JSON com os dados dos orçamentos.</returns>
        [HttpGet]
        [Route("")]
        [SwaggerResponse(200, "Orçamentos sem paginação (apenas uma página de retorno) ou última página retornada.", Type = typeof(IEnumerable<Models.Orcamentos.Lista.ListaDto>))]
        [SwaggerResponse(204, "Orçamentos não encontrados para o filtro informado.")]
        [SwaggerResponse(206, "Orçamentos paginados (qualquer página, exceto a última).", Type = typeof(IEnumerable<Models.Orcamentos.Lista.ListaDto>))]
        [SwaggerResponse(400, "Filtro inválido informado (campo com valor ou formato inválido).", Type = typeof(MensagemDto))]
        public IHttpActionResult ObterListaOrcamentos([FromUri] Models.Orcamentos.Lista.FiltroDto filtro)
        {
            using (var sessao = new GDATransaction())
            {
                filtro = filtro ?? new Models.Orcamentos.Lista.FiltroDto();

                var orcamentos = OrcamentoDAO.Instance.GetList(
                    (uint)(filtro.Id ?? 0),
                    (uint)(filtro.IdLoja ?? 0),
                    (uint)(filtro.IdCliente ?? 0),
                    filtro.NomeCliente,
                    (uint)(filtro.IdVendedor ?? 0),
                    filtro.Telefone,
                    (uint)(filtro.IdCidade ?? 0),
                    filtro.Endereco,
                    filtro.Complemento,
                    filtro.Bairro,
                    filtro.Situacao?.Select(f => (int)f),
                    filtro.PeriodoCadastroInicio,
                    filtro.PeriodoCadastroFim,
                    filtro.ObterTraducaoOrdenacao(),
                    filtro.ObterPrimeiroRegistroRetornar(),
                    filtro.NumeroRegistros);

                return this.ListaPaginada(
                    orcamentos.Select(o => new Models.Orcamentos.Lista.ListaDto(o)),
                    filtro,
                    () => OrcamentoDAO.Instance.GetCount(
                        null,
                        (uint)(filtro.Id ?? 0),
                        (uint)(filtro.IdLoja ?? 0),
                        (uint)(filtro.IdCliente ?? 0),
                        filtro.NomeCliente,
                        (uint)(filtro.IdVendedor ?? 0),
                        filtro.Telefone,
                        (uint)(filtro.IdCidade ?? 0),
                        filtro.Endereco,
                        filtro.Complemento,
                        filtro.Bairro,
                        filtro.Situacao?.Select(f => (int)f),
                        filtro.PeriodoCadastroInicio,
                        filtro.PeriodoCadastroFim));
            }
        }

        /// <summary>
        /// Recupera a lista de situações de orçamento.
        /// </summary>
        /// <returns>Uma lista JSON com os dados básicos de situações do orçamento.</returns>
        [HttpGet]
        [Route("situacoes")]
        [SwaggerResponse(200, "Situações encontradas.", Type = typeof(IEnumerable<IdNomeDto>))]
        [SwaggerResponse(204, "Situações não encontradas.")]
        public IHttpActionResult ObterSituacoes()
        {
            using (var sessao = new GDATransaction())
            {
                var situacoes = DataSources.Instance.GetSituacaoOrcamento()
                    .Select(s => new IdNomeDto
                    {
                        Id = (int)(s.Id ?? 0),
                        Nome = s.Descr,
                    });

                return this.Lista(situacoes);
            }
        }
    }
}
