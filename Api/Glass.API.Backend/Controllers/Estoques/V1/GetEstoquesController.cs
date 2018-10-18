// <copyright file="GetEstoquesController.cs" company="Sync Softwares">
// Copyright (c) Sync Softwares. Todos os direitos reservados.
// </copyright>

using GDA;
using Glass.API.Backend.Helper.Respostas;
using Glass.Data.DAL;
using Glass.Data.Helper;
using Swashbuckle.Swagger.Annotations;
using System.Collections.Generic;
using System.Linq;
using System.Web.Http;

namespace Glass.API.Backend.Controllers.Estoques.V1
{
    /// <summary>
    /// Controller de estoques.
    /// </summary>
    public partial class EstoquesController : BaseController
    {
        /// <summary>
        /// Recupera as configurações usadas pela tela de listagem de estoque.
        /// </summary>
        /// <returns>Um objeto JSON com as configurações da tela.</returns>
        [HttpGet]
        [Route("configuracoes")]
        [SwaggerResponse(200, "Configurações recuperadas.", Type = typeof(Models.Estoques.V1.Configuracoes.ListaDto))]
        public IHttpActionResult ObterConfiguracoesListaEstoque()
        {
            using (var sessao = new GDATransaction())
            {
                var configuracoes = new Models.Estoques.V1.Configuracoes.ListaDto();
                return this.Item(configuracoes);
            }
        }

        /// <summary>
        /// Recupera a lista de estoques.
        /// </summary>
        /// <param name="filtro">Os filtros para a busca dos estoques.</param>
        /// <returns>Uma lista JSON com os dados dos estoques.</returns>
        [HttpGet]
        [Route("")]
        [SwaggerResponse(200, "Estoques de produto sem paginação (apenas uma página de retorno) ou última página retornada.", Type = typeof(IEnumerable<Models.Estoques.V1.Lista.ListaDto>))]
        [SwaggerResponse(204, "Estoques de produto não encontrados para o filtro informado.")]
        [SwaggerResponse(206, "Estoques de produto paginados (qualquer página, exceto a última).", Type = typeof(IEnumerable<Models.Estoques.V1.Lista.ListaDto>))]
        [SwaggerResponse(400, "Filtro inválido informado (campo com valor ou formato inválido).", Type = typeof(MensagemDto))]
        public IHttpActionResult ObterListaEstoques([FromUri] Models.Estoques.V1.Lista.FiltroDto filtro)
        {
            using (var sessao = new GDATransaction())
            {
                filtro = filtro ?? new Models.Estoques.V1.Lista.FiltroDto();

                var situacao = filtro.Situacao.GetValueOrDefault(Situacao.Ativo);
                var idLoja = filtro.IdLoja.GetValueOrDefault((int)UserInfo.GetUserInfo.IdLoja);

                var estoques = ProdutoLojaDAO.Instance.GetForEstoque(
                    (uint)idLoja,
                    filtro.CodigoInternoProduto,
                    filtro.DescricaoProduto,
                    (uint)(filtro.IdGrupoProduto ?? 0),
                    filtro.IdsSubgrupoProduto != null && filtro.IdsSubgrupoProduto.Any() ? string.Join(",", filtro.IdsSubgrupoProduto) : null,
                    filtro.ApenasComEstoque,
                    filtro.ApenasPosseTerceiros,
                    filtro.ApenasProdutosProjeto,
                    (uint)(filtro.IdCorVidro ?? 0),
                    (uint)(filtro.IdCorFerragem ?? 0),
                    (uint)(filtro.IdCorAluminio ?? 0),
                    (int)situacao,
                    filtro.EstoqueFiscal ? 1 : 0,
                    filtro.AguardandoSaidaEstoque,
                    0,
                    filtro.ObterTraducaoOrdenacao(),
                    filtro.ObterPrimeiroRegistroRetornar(),
                    filtro.NumeroRegistros);

                return this.ListaPaginada(
                    estoques.Select(o => new Models.Estoques.V1.Lista.ListaDto(o)),
                    filtro,
                    () => ProdutoLojaDAO.Instance.GetForEstoqueCount(
                        (uint)idLoja,
                        filtro.CodigoInternoProduto,
                        filtro.DescricaoProduto,
                        (uint)(filtro.IdGrupoProduto ?? 0),
                        filtro.IdsSubgrupoProduto != null && filtro.IdsSubgrupoProduto.Any() ? string.Join(",", filtro.IdsSubgrupoProduto) : null,
                        filtro.ApenasComEstoque,
                        filtro.ApenasPosseTerceiros,
                        filtro.ApenasProdutosProjeto,
                        (uint)(filtro.IdCorVidro ?? 0),
                        (uint)(filtro.IdCorFerragem ?? 0),
                        (uint)(filtro.IdCorAluminio ?? 0),
                        (int)situacao,
                        filtro.EstoqueFiscal ? 1 : 0,
                        filtro.AguardandoSaidaEstoque,
                        filtro.OrdenacaoFiltro));
            }
        }
    }
}
