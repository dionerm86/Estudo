// <copyright file="GetMovimentacoesEstoqueFiscalController.cs" company="Sync Softwares">
// Copyright (c) Sync Softwares. Todos os direitos reservados.
// </copyright>

using GDA;
using Glass.API.Backend.Helper.Respostas;
using Glass.Data.DAL;
using Glass.Data.Helper;
using Glass.Data.Model;
using Swashbuckle.Swagger.Annotations;
using System.Collections.Generic;
using System.Linq;
using System.Web.Http;

namespace Glass.API.Backend.Controllers.Estoques.V1.Movimentacoes.Fiscais
{
    /// <summary>
    /// Controller de movimentação de estoque fiscal.
    /// </summary>
    public partial class MovimentacoesEstoqueFiscalController : BaseController
    {
        /// <summary>
        /// Recupera a lista de movimentações do estoque fiscal.
        /// </summary>
        /// <param name="filtro">Os filtros para a busca dos estoques.</param>
        /// <returns>Uma lista JSON com os dados dos estoques.</returns>
        [HttpGet]
        [Route("")]
        [SwaggerResponse(200, "Movimentações de estoque encontradas sem paginação (apenas uma página de retorno) ou última página retornada.", Type = typeof(IEnumerable<Models.Estoques.V1.Movimentacoes.Fiscais.ListaDto>))]
        [SwaggerResponse(204, "Movimentações de estoque não encontradas.")]
        [SwaggerResponse(206, "Movimentações de estoque paginadas (qualquer página, exceto a última).", Type = typeof(IEnumerable<Models.Estoques.V1.Movimentacoes.Fiscais.ListaDto>))]
        public IHttpActionResult ObterListaMovimentacaoEstoqueFiscal([FromUri] Models.Estoques.V1.Movimentacoes.Fiscais.FiltroDto filtro)
        {
            using (var sessao = new GDATransaction())
            {
                filtro = filtro ?? new Models.Estoques.V1.Movimentacoes.Fiscais.FiltroDto();

                var situacao = filtro.SituacaoProduto.GetValueOrDefault(Situacao.Ativo);
                var idLoja = filtro.IdLoja.GetValueOrDefault((int)UserInfo.GetUserInfo.IdLoja);

                var movimentacoesEstoqueFiscal = MovEstoqueFiscalDAO.Instance.GetList(
                    (uint)(filtro.IdLoja ?? 0),
                    filtro.CodigoProduto,
                    filtro.DescricaoProduto,
                    filtro.Ncm,
                    filtro.NumeroNotaFiscal,
                    filtro.PeriodoMovimentacaoInicio?.ToShortDateString(),
                    filtro.PeriodoMovimentacaoFim?.ToShortDateString(),
                    filtro.TipoMovimentacao ?? 0,
                    (int)situacao,
                    (uint)(filtro.IdCfop ?? 0),
                    (uint)(filtro.IdGrupoProduto ?? 0),
                    (uint)(filtro.IdSubgrupoProduto ?? 0),
                    (uint)(filtro.IdCorVidro ?? 0),
                    (uint)(filtro.IdCorFerragem ?? 0),
                    (uint)(filtro.IdCorAluminio ?? 0),
                    filtro.ApenasLancamentosManuais == true);

                return this.Lista(
                    movimentacoesEstoqueFiscal.Select(o => new Models.Estoques.V1.Movimentacoes.Fiscais.ListaDto(o)));
            }
        }

        /// <summary>
        /// Recupera as configurações usadas pela tela de listagem de movimentações do estoque fiscal.
        /// </summary>
        /// <returns>Um objeto JSON com as configurações da tela.</returns>
        [HttpGet]
        [Route("configuracoes")]
        [SwaggerResponse(200, "Configurações recuperadas.", Type = typeof(Models.Estoques.V1.Movimentacoes.Fiscais.Configuracoes.ListaDto))]
        public IHttpActionResult ObterConfiguracoes()
        {
            using (var sessao = new GDATransaction())
            {
                var configuracoes = new Models.Estoques.V1.Movimentacoes.Fiscais.Configuracoes.ListaDto();
                return this.Item(configuracoes);
            }
        }
    }
}
