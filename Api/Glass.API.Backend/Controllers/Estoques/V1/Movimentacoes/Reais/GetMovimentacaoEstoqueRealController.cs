// <copyright file="GetMovimentacaoEstoqueRealController.cs" company="Sync Softwares">
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

namespace Glass.API.Backend.Controllers.Estoques.V1.Movimentacoes.Reais
{
    /// <summary>
    /// Controller de movimentação de estoque real.
    /// </summary>
    public partial class MovimentacaoEstoqueRealController : BaseController
    {
        /// <summary>
        /// Recupera a lista de movimentações do estoque real.
        /// </summary>
        /// <param name="filtro">Os filtros para a busca dos estoques.</param>
        /// <returns>Uma lista JSON com os dados dos estoques.</returns>
        [HttpGet]
        [Route("")]
        [SwaggerResponse(200, "Movimentações de estoque encontradas sem paginação (apenas uma página de retorno) ou última página retornada.", Type = typeof(IEnumerable<Models.Estoques.V1.Movimentacoes.Reais.ListaDto>))]
        [SwaggerResponse(204, "Movimentações de estoque não encontradas.")]
        [SwaggerResponse(206, "Movimentações de estoque paginadas (qualquer página, exceto a última).", Type = typeof(IEnumerable<Models.Estoques.V1.Movimentacoes.Reais.ListaDto>))]
        public IHttpActionResult ObterListaMovimentacaoEstoqueReal([FromUri] Models.Estoques.V1.Movimentacoes.Reais.FiltroDto filtro)
        {
            using (var sessao = new GDATransaction())
            {
                filtro = filtro ?? new Models.Estoques.V1.Movimentacoes.Reais.FiltroDto();

                var situacao = filtro.SituacaoProduto.GetValueOrDefault(Situacao.Ativo);
                var idLoja = filtro.IdLoja.GetValueOrDefault((int)UserInfo.GetUserInfo.IdLoja);

                var movimentacoesEstoqueReal = MovEstoqueDAO.Instance.GetList(
                    (uint)idLoja,
                    filtro.CodigoProduto,
                    filtro.DescricaoProduto,
                    filtro.CodigoOtimizacao,
                    filtro.PeriodoMovimentacaoInicio?.ToShortDateString(),
                    filtro.PeriodoMovimentacaoFim?.ToShortDateString(),
                    filtro.TipoMovimentacao ?? 0,
                    (int)(filtro.SituacaoProduto ?? 0),
                    filtro.IdsGrupoProduto != null && filtro.IdsGrupoProduto.Any() ? string.Join(",", filtro.IdsGrupoProduto) : null,
                    filtro.IdsSubgrupoProduto != null && filtro.IdsSubgrupoProduto.Any() ? string.Join(",", filtro.IdsSubgrupoProduto) : null,
                    (uint)(filtro.IdCorVidro ?? 0),
                    (uint)(filtro.IdCorFerragem ?? 0),
                    (uint)(filtro.IdCorAluminio ?? 0),
                    filtro.NaoBuscarProdutosComEstoqueZerado ?? false,
                    filtro.ApenasLancamentosManuais ?? false);

                return this.Lista(
                    movimentacoesEstoqueReal.Select(o => new Models.Estoques.V1.Movimentacoes.Reais.ListaDto(o)));
            }
        }

        /// <summary>
        /// Recupera o código da tabela MovEstoque.
        /// </summary>
        /// <returns>Número inteiro com o código da tabela de cancelamento MovEstoque.</returns>
        [HttpGet]
        [Route("codigoTabela")]
        [SwaggerResponse(200, "Código da tabela de cancelamento encontrado", Type = typeof(int))]
        [SwaggerResponse(204, "Código da tabela de cancelamento não encontrado")]
        public IHttpActionResult ObterCodigoTabelaCancelamento()
        {
            using (var sessao = new GDATransaction())
            {
                return this.Item((int)LogCancelamento.TabelaCancelamento.MovEstoque);
            }
        }
    }
}
