// <copyright file="GetTrocasDevolucoesController.cs" company="Sync Softwares">
// Copyright (c) Sync Softwares. Todos os direitos reservados.
// </copyright>

using GDA;
using Glass.API.Backend.Helper.Respostas;
using Glass.API.Backend.Models.Genericas.V1;
using Glass.Data.DAL;
using Swashbuckle.Swagger.Annotations;
using System.Collections.Generic;
using System.Linq;
using System.Web.Http;

namespace Glass.API.Backend.Controllers.Estoques.V1.TrocasDevolucoes
{
    /// <summary>
    /// Controller de trocas/devoluções.
    /// </summary>
    public partial class TrocasDevolucoesController : BaseController
    {
        /// <summary>
        /// Recupera a lista de dados para a listagem de trocas/devoluções.
        /// </summary>
        /// <param name="filtro">Os filtros para a busca das trocas/devoluções.</param>
        /// <returns>Uma lista JSON com os dados das trocas/devoluções.</returns>
        [HttpGet]
        [Route("")]
        [SwaggerResponse(200, "Trocas/devoluções encontradas sem paginação (apenas uma página de retorno) ou última página retornada", Type = typeof(IEnumerable<Models.Estoques.V1.TrocasDevolucoes.Lista.ListaDto>))]
        [SwaggerResponse(204, "Trocas/devoluções não encontradas.")]
        [SwaggerResponse(206, "Trocas/devoluções paginadas (qualquer página, exceto a última).", Type = typeof(IEnumerable<Models.Estoques.V1.TrocasDevolucoes.Lista.ListaDto>))]
        [SwaggerResponse(400, "Filtro inválido informado (campo com valor ou formato inválido).", Type = typeof(MensagemDto))]
        public IHttpActionResult ObterListaTrocasDevolucoes([FromUri] Models.Estoques.V1.TrocasDevolucoes.Lista.FiltroDto filtro)
        {
            using (var sessao = new GDATransaction())
            {
                filtro = filtro ?? new Models.Estoques.V1.TrocasDevolucoes.Lista.FiltroDto();

                var idsFuncionario = filtro.IdsFuncionario != null ? string.Join(",", filtro.IdsFuncionario) : string.Empty;
                var idsFuncionarioAssociadoCliente = filtro.IdsFuncionario != null ? string.Join(",", filtro.IdsFuncionarioAssociadoCliente) : string.Empty;
                var tiposPedidos = filtro.TipoPedido != null ? string.Join(",", filtro.TipoPedido) : string.Empty;

                var trocasDevolucoes = TrocaDevolucaoDAO.Instance.GetList(
                    (uint)filtro.Id.GetValueOrDefault(),
                    (uint)filtro.IdPedido.GetValueOrDefault(),
                    filtro.Tipo.GetValueOrDefault(),
                    filtro.Situacao != null ? (int)filtro.Situacao.Value : 0,
                    (uint)filtro.IdCliente.GetValueOrDefault(),
                    filtro.NomeCliente ?? string.Empty,
                    idsFuncionario,
                    idsFuncionarioAssociadoCliente,
                    filtro.PeriodoTrocaInicio != null ? filtro.PeriodoTrocaInicio.ToString() : string.Empty,
                    filtro.PeriodoTrocaFim != null ? filtro.PeriodoTrocaInicio.ToString() : string.Empty,
                    (uint)filtro.IdProduto.GetValueOrDefault(),
                    filtro.AlturaMinima.GetValueOrDefault(),
                    filtro.AlturaMaxima.GetValueOrDefault(),
                    filtro.LarguraMinima.GetValueOrDefault(),
                    filtro.LarguraMaxima.GetValueOrDefault(),
                    (uint)filtro.IdOrigemTrocaDevolucao.GetValueOrDefault(),
                    (uint)filtro.IdTipoPerda.GetValueOrDefault(),
                    filtro.IdSetor.GetValueOrDefault(),
                    tiposPedidos,
                    filtro.IdGrupoProduto,
                    filtro.IdSubgrupoProduto.GetValueOrDefault(),
                    filtro.ObterTraducaoOrdenacao(),
                    filtro.ObterPrimeiroRegistroRetornar(),
                    filtro.NumeroRegistros);

                return this.ListaPaginada(
                    trocasDevolucoes.Select(troca => new Models.Estoques.V1.TrocasDevolucoes.Lista.ListaDto(troca)),
                    filtro,
                    () => TrocaDevolucaoDAO.Instance.GetCount(
                        (uint)filtro.Id.GetValueOrDefault(),
                        (uint)filtro.IdPedido.GetValueOrDefault(),
                        filtro.Tipo.GetValueOrDefault(),
                        filtro.Situacao != null ? (int)filtro.Situacao.Value : 0,
                        (uint)filtro.IdCliente.GetValueOrDefault(),
                        filtro.NomeCliente ?? string.Empty,
                        idsFuncionario,
                        idsFuncionarioAssociadoCliente,
                        filtro.PeriodoTrocaInicio != null ? filtro.PeriodoTrocaInicio.ToString() : string.Empty,
                        filtro.PeriodoTrocaFim != null ? filtro.PeriodoTrocaInicio.ToString() : string.Empty,
                        (uint)filtro.IdProduto.GetValueOrDefault(),
                        filtro.AlturaMinima.GetValueOrDefault(),
                        filtro.AlturaMaxima.GetValueOrDefault(),
                        filtro.LarguraMinima.GetValueOrDefault(),
                        filtro.LarguraMaxima.GetValueOrDefault(),
                        (uint)filtro.IdOrigemTrocaDevolucao.GetValueOrDefault(),
                        (uint)filtro.IdTipoPerda.GetValueOrDefault(),
                        filtro.IdSetor.GetValueOrDefault(),
                        tiposPedidos,
                        filtro.IdGrupoProduto,
                        filtro.IdSubgrupoProduto.GetValueOrDefault()));
            }
        }

        /// <summary>
        /// Obtém as configurações para a tela de listagem de trocas/devoluções.
        /// </summary>
        /// <returns>Um objeto JSON com as configurações da tela.</returns>
        [HttpGet]
        [Route("configuracoes")]
        [SwaggerResponse(200, "Configurações encontradas.", Type = typeof(Models.Estoques.V1.TrocasDevolucoes.Configuracoes.ListaDto))]
        public IHttpActionResult ObterConfiguracoesListagem()
        {
            var configuracoes = new Models.Estoques.V1.TrocasDevolucoes.Configuracoes.ListaDto();
            return this.Item(configuracoes);
        }

        /// <summary>
        /// Recupera as origens usadas pela tela de listagem de trocas/devoluções.
        /// </summary>
        /// <returns>Um objeto JSON com as origens da tela.</returns>
        [HttpGet]
        [Route("origens/filtro")]
        [SwaggerResponse(200, "Origens encontrados.", Type = typeof(IEnumerable<IdNomeDto>))]
        [SwaggerResponse(204, "Origens não encontrados.")]
        public IHttpActionResult ObterOrigemListaTrocaDevolucao()
        {
            using (var sessao = new GDATransaction())
            {
                var origens = OrigemTrocaDescontoDAO.Instance.GetList()
                    .Select(f => new IdNomeDto
                    {
                        Id = f.IdOrigemTrocaDesconto,
                        Nome = f.Descricao,
                    });

                return this.Item(origens);
            }
        }
    }
}
