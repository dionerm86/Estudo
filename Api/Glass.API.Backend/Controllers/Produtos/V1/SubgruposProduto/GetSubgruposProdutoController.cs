// <copyright file="GetSubgruposProdutoController.cs" company="Sync Softwares">
// Copyright (c) Sync Softwares. Todos os direitos reservados.
// </copyright>

using GDA;
using Glass.API.Backend.Helper;
using Glass.API.Backend.Helper.Respostas;
using Glass.API.Backend.Models.Genericas.V1;
using Glass.Data.DAL;
using Glass.Data.Model;
using Swashbuckle.Swagger.Annotations;
using System.Collections.Generic;
using System.Linq;
using System.Web.Http;

namespace Glass.API.Backend.Controllers.Produtos.V1.SubgruposProduto
{
    /// <summary>
    /// Controller de subgrupos de produto.
    /// </summary>
    public partial class SubgruposProdutoController : BaseController
    {
        /// <summary>
        /// Recupera as configurações usadas pela tela de listagem de subgrupos de produto.
        /// </summary>
        /// <returns>Um objeto JSON com as configurações da tela.</returns>
        [HttpGet]
        [Route("configuracoes")]
        [SwaggerResponse(200, "Configurações recuperadas.", Type = typeof(Models.Produtos.V1.SubgruposProduto.Configuracoes.ListaDto))]
        public IHttpActionResult ObterConfiguracoesListaSubgruposProduto()
        {
            using (var sessao = new GDATransaction())
            {
                var configuracoes = new Models.Produtos.V1.SubgruposProduto.Configuracoes.ListaDto();
                return this.Item(configuracoes);
            }
        }

        /// <summary>
        /// Recupera a lista de subgrupos de produto.
        /// </summary>
        /// <param name="filtro">Os filtros para a busca dos subgrupos de produto.</param>
        /// <returns>Uma lista JSON com os dados dos subgrupos de produto.</returns>
        [HttpGet]
        [Route("")]
        [SwaggerResponse(200, "Subgrupos de produto sem paginação (apenas uma página de retorno) ou última página retornada.", Type = typeof(IEnumerable<Models.Produtos.V1.SubgruposProduto.Lista.ListaDto>))]
        [SwaggerResponse(204, "Subgrupos de produto não encontrados para o filtro informado.")]
        [SwaggerResponse(206, "Subgrupos de produto paginados (qualquer página, exceto a última).", Type = typeof(IEnumerable<Models.Produtos.V1.SubgruposProduto.Lista.ListaDto>))]
        [SwaggerResponse(400, "Filtro inválido informado (campo com valor ou formato inválido).", Type = typeof(MensagemDto))]
        public IHttpActionResult ObterListaSubgruposProduto([FromUri] Models.Produtos.V1.SubgruposProduto.Lista.FiltroDto filtro)
        {
            using (var sessao = new GDATransaction())
            {
                filtro = filtro ?? new Models.Produtos.V1.SubgruposProduto.Lista.FiltroDto();

                var subgruposProduto = Microsoft.Practices.ServiceLocation.ServiceLocator
                    .Current.GetInstance<Global.Negocios.IGrupoProdutoFluxo>()
                    .PesquisarSubgruposProduto(filtro.IdGrupoProduto);

                ((Colosoft.Collections.IVirtualList)subgruposProduto).Configure(filtro.NumeroRegistros);
                ((Colosoft.Collections.ISortableCollection)subgruposProduto).ApplySort(filtro.ObterTraducaoOrdenacao());

                return this.ListaPaginada(
                    subgruposProduto
                        .Skip(filtro.ObterPrimeiroRegistroRetornar())
                        .Take(filtro.NumeroRegistros)
                        .Select(s => new Models.Produtos.V1.SubgruposProduto.Lista.ListaDto(s)),
                    filtro,
                    () => subgruposProduto.Count);
            }
        }

        /// <summary>
        /// Recupera os subgrupos de produto para os controles de filtro das telas.
        /// </summary>
        /// <param name="idGrupoProduto">O identificador do grupo de produto.</param>
        /// <returns>Uma lista JSON com os subgrupos de produto encontrados.</returns>
        [HttpGet]
        [Route("filtro")]
        [SwaggerResponse(200, "Subgrupos de produto encontrados.", Type = typeof(IEnumerable<IdNomeDto>))]
        [SwaggerResponse(204, "Subgrupos de produto não encontrados.")]
        public IHttpActionResult ObterSubgruposProdutoParaFiltro(int? idGrupoProduto = null)
        {
            using (var sessao = new GDATransaction())
            {
                var situacoes = SubgrupoProdDAO.Instance.GetForFilter(idGrupoProduto.GetValueOrDefault())
                    .Where(g => g.IdSubgrupoProd > 0)
                    .Select(g => new IdNomeDto
                    {
                        Id = g.IdSubgrupoProd,
                        Nome = g.Descricao,
                    });

                return this.Lista(situacoes);
            }
        }

        /// <summary>
        /// Recupera os subgrupos de produto para os controles de filtro das telas.
        /// </summary>
        /// <param name="idsGruposProduto">O identificador do grupo de produto.</param>
        /// <returns>Uma lista JSON com os subgrupos de produto encontrados.</returns>
        [HttpGet]
        [Route("filtro/varios")]
        [SwaggerResponse(200, "Subgrupos de produto encontrados.", Type = typeof(IEnumerable<IdNomeDto>))]
        [SwaggerResponse(204, "Subgrupos de produto não encontrados.")]
        public IHttpActionResult ObterSubgruposProdutoParaFiltro([FromUri] int[] idsGruposProduto)
        {
            using (var sessao = new GDATransaction())
            {
                if (idsGruposProduto == null || !idsGruposProduto.Any())
                {
                    return this.SemConteudo();
                }

                var ids = string.Join(",", idsGruposProduto);
                var situacoes = SubgrupoProdDAO.Instance.GetForFilter(ids)
                    .Where(g => g.IdSubgrupoProd > 0)
                    .Select(g => new IdNomeDto
                    {
                        Id = g.IdSubgrupoProd,
                        Nome = g.Descricao,
                    });

                return this.Lista(situacoes);
            }
        }

        /// <summary>
        /// Recupera a lista de tipos de subgrupo de produto.
        /// </summary>
        /// <returns>Uma lista JSON com os dados básicos dos tipos de subgrupo de produto.</returns>
        [HttpGet]
        [Route("tipos")]
        [SwaggerResponse(200, "Tipos de subgrupo de produto encontrados.", Type = typeof(IEnumerable<IdNomeDto>))]
        [SwaggerResponse(204, "Tipos de subgrupo de produto não encontrados.")]
        public IHttpActionResult ObterTipos()
        {
            using (var sessao = new GDATransaction())
            {
                var tipos = new ConversorEnum<TipoSubgrupoProd>()
                    .ObterTraducao();

                return this.Lista(tipos);
            }
        }
    }
}
