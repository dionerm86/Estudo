// <copyright file="GetEstoquesController.cs" company="Sync Softwares">
// Copyright (c) Sync Softwares. Todos os direitos reservados.
// </copyright>

using GDA;
using Glass.API.Backend.Helper.Respostas;
using Glass.Data.DAL;
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
        [SwaggerResponse(200, "Configurações recuperadas.", Type = typeof(Models.Estoques.Configuracoes.ListaDto))]
        public IHttpActionResult ObterConfiguracoesListaEstoque()
        {
            using (var sessao = new GDATransaction())
            {
                var configuracoes = new Models.Estoques.Configuracoes.ListaDto();
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
        [SwaggerResponse(200, "Estoques de produto sem paginação (apenas uma página de retorno) ou última página retornada.", Type = typeof(IEnumerable<Models.Estoques.Lista.ListaDto>))]
        [SwaggerResponse(204, "Estoques de produto não encontradas para o filtro informado.")]
        [SwaggerResponse(206, "Estoques de produto paginados (qualquer página, exceto a última).", Type = typeof(IEnumerable<Models.Estoques.Lista.ListaDto>))]
        [SwaggerResponse(400, "Filtro inválido informado (campo com valor ou formato inválido).", Type = typeof(MensagemDto))]
        public IHttpActionResult ObterListaEstoques([FromUri] Models.Estoques.Lista.FiltroDto filtro)
        {
            using (var sessao = new GDATransaction())
            {
                filtro = filtro ?? new Models.Estoques.Lista.FiltroDto();

                var estoques = ProdutoLojaDAO.Instance.GetForEstoque(
                    (uint)(filtro.IdLoja ?? 0),
                    filtro.CodigoInternoProduto,
                    filtro.DescricaoProduto,
                    (uint)(filtro.IdGrupoProduto ?? 0),
                    (uint)(filtro.IdSubgrupoProduto ?? 0),
                    filtro.ApenasComEstoque,
                    filtro.ApenasPosseTerceiros,
                    filtro.ApenasProdutosProjeto,
                    (uint)(filtro.IdCorVidro ?? 0),
                    (uint)(filtro.IdCorFerragem ?? 0),
                    (uint)(filtro.IdCorAluminio ?? 0),
                    (int)filtro.Situacao,
                    filtro.EstoqueFiscal ? 1 : 0,
                    filtro.AguardandoSaidaEstoque,
                    filtro.OrdenacaoFiltro,
                    filtro.ObterTraducaoOrdenacao(),
                    filtro.ObterPrimeiroRegistroRetornar(),
                    filtro.NumeroRegistros);

                return this.ListaPaginada(
                    estoques.Select(o => new Models.Estoques.Lista.ListaDto(o)),
                    filtro,
                    () => ProdutoLojaDAO.Instance.GetForEstoqueCount(
                        (uint)(filtro.IdLoja ?? 0),
                        filtro.CodigoInternoProduto,
                        filtro.DescricaoProduto,
                        (uint)(filtro.IdGrupoProduto ?? 0),
                        (uint)(filtro.IdSubgrupoProduto ?? 0),
                        filtro.ApenasComEstoque,
                        filtro.ApenasPosseTerceiros,
                        filtro.ApenasProdutosProjeto,
                        (uint)(filtro.IdCorVidro ?? 0),
                        (uint)(filtro.IdCorFerragem ?? 0),
                        (uint)(filtro.IdCorAluminio ?? 0),
                        (int)filtro.Situacao,
                        filtro.EstoqueFiscal ? 1 : 0,
                        filtro.AguardandoSaidaEstoque,
                        filtro.OrdenacaoFiltro));
            }
        }
    }
}
