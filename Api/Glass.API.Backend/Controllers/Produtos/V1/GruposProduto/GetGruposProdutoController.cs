// <copyright file="GetGruposProdutoController.cs" company="Sync Softwares">
// Copyright (c) Sync Softwares. Todos os direitos reservados.
// </copyright>

using GDA;
using Glass.API.Backend.Helper;
using Glass.API.Backend.Helper.Respostas;
using Glass.API.Backend.Models.Genericas;
using Glass.Data.DAL;
using Glass.Data.Model;
using Swashbuckle.Swagger.Annotations;
using System.Collections.Generic;
using System.Linq;
using System.Web.Http;

namespace Glass.API.Backend.Controllers.Produtos.V1.GruposProduto
{
    /// <summary>
    /// Controller de grupos de produto.
    /// </summary>
    public partial class GruposProdutoController : BaseController
    {
        /// <summary>
        /// Recupera as configurações usadas pela tela de listagem de grupos de produto.
        /// </summary>
        /// <returns>Um objeto JSON com as configurações da tela.</returns>
        [HttpGet]
        [Route("configuracoes")]
        [SwaggerResponse(200, "Configurações recuperadas.", Type = typeof(Models.Produtos.GruposProduto.Configuracoes.ListaDto))]
        public IHttpActionResult ObterConfiguracoesListaGruposProduto()
        {
            using (var sessao = new GDATransaction())
            {
                var configuracoes = new Models.Produtos.GruposProduto.Configuracoes.ListaDto();
                return this.Item(configuracoes);
            }
        }

        /// <summary>
        /// Recupera a lista de grupos de produto.
        /// </summary>
        /// <param name="filtro">Os filtros para a busca dos grupos de produto.</param>
        /// <returns>Uma lista JSON com os dados dos grupos de produto.</returns>
        [HttpGet]
        [Route("")]
        [SwaggerResponse(200, "Grupos de produto sem paginação (apenas uma página de retorno) ou última página retornada.", Type = typeof(IEnumerable<Models.Produtos.GruposProduto.Lista.ListaDto>))]
        [SwaggerResponse(204, "Grupos de produto não encontrados para o filtro informado.")]
        [SwaggerResponse(206, "Grupos de produto paginados (qualquer página, exceto a última).", Type = typeof(IEnumerable<Models.Produtos.GruposProduto.Lista.ListaDto>))]
        [SwaggerResponse(400, "Filtro inválido informado (campo com valor ou formato inválido).", Type = typeof(MensagemDto))]
        public IHttpActionResult ObterListaGruposProduto([FromUri] Models.Produtos.GruposProduto.Lista.FiltroDto filtro)
        {
            using (var sessao = new GDATransaction())
            {
                filtro = filtro ?? new Models.Produtos.GruposProduto.Lista.FiltroDto();

                var gruposProduto = Microsoft.Practices.ServiceLocation.ServiceLocator
                    .Current.GetInstance<Global.Negocios.IGrupoProdutoFluxo>()
                    .PesquisarGruposProduto();

                ((Colosoft.Collections.IVirtualList)gruposProduto).Configure(filtro.NumeroRegistros);
                ((Colosoft.Collections.ISortableCollection)gruposProduto).ApplySort(filtro.ObterTraducaoOrdenacao());

                return this.ListaPaginada(
                    gruposProduto
                        .Skip(filtro.ObterPrimeiroRegistroRetornar())
                        .Take(filtro.NumeroRegistros)
                        .Select(c => new Models.Produtos.GruposProduto.Lista.ListaDto(c)),
                    filtro,
                    () => gruposProduto.Count);
            }
        }

        /// <summary>
        /// Recupera os grupos de produto para os controles de filtro das telas.
        /// </summary>
        /// <returns>Uma lista JSON com os grupos de produto encontrados.</returns>
        [HttpGet]
        [Route("filtro")]
        [SwaggerResponse(200, "Grupos de produto encontrados.", Type = typeof(IEnumerable<IdNomeDto>))]
        [SwaggerResponse(204, "Grupos de produto não encontrados.")]
        public IHttpActionResult ObterGruposProdutoParaFiltro()
        {
            using (var sessao = new GDATransaction())
            {
                var situacoes = GrupoProdDAO.Instance.GetForFilter()
                    .Where(g => g.IdGrupoProd > 0)
                    .Select(g => new IdNomeDto
                    {
                        Id = g.IdGrupoProd,
                        Nome = g.Descricao,
                    });

                return this.Lista(situacoes);
            }
        }

        /// <summary>
        /// Recupera a lista de tipos de grupo de produto.
        /// </summary>
        /// <returns>Uma lista JSON com os dados básicos dos tipos de grupo de produto.</returns>
        [HttpGet]
        [Route("tipos")]
        [SwaggerResponse(200, "Tipos de grupo de produto encontrados.", Type = typeof(IEnumerable<IdNomeDto>))]
        [SwaggerResponse(204, "Tipos de grupo de produto não encontrados.")]
        public IHttpActionResult ObterTipos()
        {
            using (var sessao = new GDATransaction())
            {
                var tipos = new ConversorEnum<TipoGrupoProd>()
                    .ObterTraducao();

                return this.Lista(tipos);
            }
        }

        /// <summary>
        /// Recupera a lista de tipos de cálculo de grupo de produto.
        /// </summary>
        /// <param name="notaFiscal">Define se serão buscados tipos de cálculo de nota fiscal, caso false, busca de pedido.</param>
        /// <returns>Uma lista JSON com os dados básicos dos tipos de cálculo de pedido de grupo de produto.</returns>
        [HttpGet]
        [Route("tiposCalculo")]
        [SwaggerResponse(200, "Tipos de cálculo encontrados.", Type = typeof(IEnumerable<IdNomeDto>))]
        [SwaggerResponse(204, "Tipos de cálculo não encontrados.")]
        public IHttpActionResult ObterTiposCalculo(bool notaFiscal)
        {
            using (var sessao = new GDATransaction())
            {
                var tipos = Microsoft.Practices.ServiceLocation.ServiceLocator
                    .Current.GetInstance<Global.Negocios.IGrupoProdutoFluxo>()
                    .ObtemTiposCalculo(true, notaFiscal);

                return this.Lista(tipos);
            }
        }
    }
}
