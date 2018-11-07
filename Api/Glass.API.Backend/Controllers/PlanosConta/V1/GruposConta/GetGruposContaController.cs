// <copyright file="GetGruposContaController.cs" company="Sync Softwares">
// Copyright (c) Sync Softwares. Todos os direitos reservados.
// </copyright>

using GDA;
using Glass.API.Backend.Helper.Respostas;
using Glass.API.Backend.Models.Genericas.V1;
using Glass.API.Backend.Models.PlanosConta.V1.GruposConta.Lista;
using Swashbuckle.Swagger.Annotations;
using System.Collections.Generic;
using System.Linq;
using System.Web.Http;

namespace Glass.API.Backend.Controllers.PlanosConta.V1.GruposConta
{
    /// <summary>
    /// Controller de grupos de conta.
    /// </summary>
    public partial class GruposContaController : BaseController
    {
        /// <summary>
        /// Recupera a lista de grupos de conta.
        /// </summary>
        /// <param name="filtro">Os filtros para a busca dos grupos de conta.</param>
        /// <returns>Uma lista JSON com os dados dos grupos de conta.</returns>
        [HttpGet]
        [Route("")]
        [SwaggerResponse(200, "Grupos de conta sem paginação (apenas uma página de retorno) ou última página retornada.", Type = typeof(IEnumerable<ListaDto>))]
        [SwaggerResponse(204, "Grupos de conta não encontrados para o filtro informado.")]
        [SwaggerResponse(206, "Grupos de conta paginados (qualquer página, exceto a última).", Type = typeof(IEnumerable<ListaDto>))]
        [SwaggerResponse(400, "Filtro inválido informado (campo com valor ou formato inválido).", Type = typeof(MensagemDto))]
        public IHttpActionResult ObterListaGruposConta([FromUri] FiltroDto filtro)
        {
            using (var sessao = new GDATransaction())
            {
                filtro = filtro ?? new FiltroDto();

                var gruposConta = Microsoft.Practices.ServiceLocation.ServiceLocator
                    .Current.GetInstance<Financeiro.Negocios.IPlanoContasFluxo>()
                    .PesquisarGruposConta();

                ((Colosoft.Collections.IVirtualList)gruposConta).Configure(filtro.NumeroRegistros);
                ((Colosoft.Collections.ISortableCollection)gruposConta).ApplySort(filtro.ObterTraducaoOrdenacao());

                return this.ListaPaginada(
                    gruposConta
                        .Skip(filtro.ObterPrimeiroRegistroRetornar())
                        .Take(filtro.NumeroRegistros)
                        .Select(c => new ListaDto(c)),
                    filtro,
                    () => gruposConta.Count);
            }
        }

        /// <summary>
        /// Recupera os grupos de conta para os controles de filtro das telas.
        /// </summary>
        /// <returns>Uma lista JSON com os grupos de conta encontrados.</returns>
        [HttpGet]
        [Route("filtro")]
        [SwaggerResponse(200, "Grupos de conta encontrados.", Type = typeof(IEnumerable<IdNomeDto>))]
        [SwaggerResponse(204, "Grupos de conta não encontrados.")]
        public IHttpActionResult ObterGruposContaParaFiltro()
        {
            using (var sessao = new GDATransaction())
            {
                var gruposConta = Microsoft.Practices.ServiceLocation.ServiceLocator
                    .Current.GetInstance<Financeiro.Negocios.IPlanoContasFluxo>()
                    .ObtemGruposContaCadastro()
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
