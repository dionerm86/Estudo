// <copyright file="GetCategoriasContaController.cs" company="Sync Softwares">
// Copyright (c) Sync Softwares. Todos os direitos reservados.
// </copyright>

using GDA;
using Glass.API.Backend.Helper;
using Glass.API.Backend.Helper.Respostas;
using Glass.API.Backend.Models.Genericas.V1;
using Glass.API.Backend.Models.PlanosConta.V1.CategoriasConta.Lista;
using Glass.Data.Model;
using Swashbuckle.Swagger.Annotations;
using System.Collections.Generic;
using System.Linq;
using System.Web.Http;

namespace Glass.API.Backend.Controllers.PlanosConta.V1.CategoriasConta
{
    /// <summary>
    /// Controller de categorias de conta.
    /// </summary>
    public partial class CategoriasContaController : BaseController
    {
        /// <summary>
        /// Recupera a lista de categorias de conta.
        /// </summary>
        /// <param name="filtro">Os filtros para a busca das categorias de conta.</param>
        /// <returns>Uma lista JSON com os dados dos itens.</returns>
        [HttpGet]
        [Route("")]
        [SwaggerResponse(200, "Categorias de conta sem paginação (apenas uma página de retorno) ou última página retornada.", Type = typeof(IEnumerable<ListaDto>))]
        [SwaggerResponse(204, "Categorias de conta não encontradas para o filtro informado.")]
        [SwaggerResponse(206, "Categorias de conta paginadas (qualquer página, exceto a última).", Type = typeof(IEnumerable<ListaDto>))]
        [SwaggerResponse(400, "Filtro inválido informado (campo com valor ou formato inválido).", Type = typeof(MensagemDto))]
        public IHttpActionResult ObterListaCategoriasConta([FromUri] FiltroDto filtro)
        {
            using (var sessao = new GDATransaction())
            {
                filtro = filtro ?? new FiltroDto();

                var categoriasConta = Microsoft.Practices.ServiceLocation.ServiceLocator
                    .Current.GetInstance<Financeiro.Negocios.IPlanoContasFluxo>()
                    .PesquisarCategoriasConta();

                ((Colosoft.Collections.IVirtualList)categoriasConta).Configure(filtro.NumeroRegistros);
                ((Colosoft.Collections.ISortableCollection)categoriasConta).ApplySort(filtro.ObterTraducaoOrdenacao());

                return this.ListaPaginada(
                    categoriasConta
                        .Skip(filtro.ObterPrimeiroRegistroRetornar())
                        .Take(filtro.NumeroRegistros)
                        .Select(c => new ListaDto(c)),
                    filtro,
                    () => categoriasConta.Count);
            }
        }

        /// <summary>
        /// Recupera os tipos de categoria de conta para os controles de filtro das telas.
        /// </summary>
        /// <returns>Uma lista JSON com os tipos de categoria de conta encontradas.</returns>
        [HttpGet]
        [Route("tipos")]
        [SwaggerResponse(200, "Tipos de categoria de conta encontrados.", Type = typeof(IEnumerable<IdNomeDto>))]
        [SwaggerResponse(204, "Tipos de categoria de conta não encontrados.")]
        public IHttpActionResult ObterTiposCategoriaParaFiltro()
        {
            using (var sessao = new GDATransaction())
            {
                var tipos = new ConversorEnum<TipoCategoriaConta>()
                    .ObterTraducao();

                return this.Lista(tipos);
            }
        }

        /// <summary>
        /// Recupera as categorias de conta para os controles de filtro das telas.
        /// </summary>
        /// <returns>Uma lista JSON com as categorias de conta encontradas.</returns>
        [HttpGet]
        [Route("filtro")]
        [SwaggerResponse(200, "Categorias de conta encontradas.", Type = typeof(IEnumerable<IdNomeDto>))]
        [SwaggerResponse(204, "Categorias de conta não encontradas.")]
        public IHttpActionResult ObterCategoriasContaParaFiltro()
        {
            using (var sessao = new GDATransaction())
            {
                var gruposConta = Microsoft.Practices.ServiceLocation.ServiceLocator
                    .Current.GetInstance<Financeiro.Negocios.IPlanoContasFluxo>()
                    .ObtemCategoriasConta(true)
                    .Select(g => new IdNomeDto
                    {
                        Id = g.Id,
                        Nome = g.Name,
                    });

                return this.Lista(gruposConta);
            }
        }
    }
}
