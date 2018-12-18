// <copyright file="GetRotasController.cs" company="Sync Softwares">
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

namespace Glass.API.Backend.Controllers.Rotas.V1
{
    /// <summary>
    /// Controller de rotas.
    /// </summary>
    public partial class RotasController : BaseController
    {
        /// <summary>
        /// Recupera as configurações usadas pela tela de listagem de rotas.
        /// </summary>
        /// <returns>Um objeto JSON com as configurações da tela.</returns>
        [HttpGet]
        [Route("configuracoes")]
        [SwaggerResponse(200, "Configurações recuperadas.", Type = typeof(Models.Rotas.V1.Configuracoes.ListaDto))]
        public IHttpActionResult ObterConfiguracoesListaRotas()
        {
            using (var sessao = new GDATransaction())
            {
                var configuracoes = new Models.Rotas.V1.Configuracoes.ListaDto();
                return this.Item(configuracoes);
            }
        }

        /// <summary>
        /// Recupera a lista de rotas.
        /// </summary>
        /// <param name="filtro">Os filtros para a busca das rotas.</param>
        /// <returns>Uma lista JSON com os dados das rotas.</returns>
        [HttpGet]
        [Route("")]
        [SwaggerResponse(200, "Rotas sem paginação (apenas uma página de retorno) ou última página retornada.", Type = typeof(IEnumerable<Models.Rotas.V1.Lista.ListaDto>))]
        [SwaggerResponse(204, "Rotas não encontradas para o filtro informado.")]
        [SwaggerResponse(206, "Rotas paginadas (qualquer página, exceto a última).", Type = typeof(IEnumerable<Models.Rotas.V1.Lista.ListaDto>))]
        [SwaggerResponse(400, "Filtro inválido informado (campo com valor ou formato inválido).", Type = typeof(MensagemDto))]
        public IHttpActionResult ObterListaRotas([FromUri] Models.Rotas.V1.Lista.FiltroDto filtro)
        {
            using (var sessao = new GDATransaction())
            {
                filtro = filtro ?? new Models.Rotas.V1.Lista.FiltroDto();

                var rotas = Microsoft.Practices.ServiceLocation.ServiceLocator
                    .Current.GetInstance<Global.Negocios.IRotaFluxo>()
                    .PesquisarRotas();

                ((Colosoft.Collections.IVirtualList)rotas).Configure(filtro.NumeroRegistros);
                ((Colosoft.Collections.ISortableCollection)rotas).ApplySort(filtro.ObterTraducaoOrdenacao());

                return this.ListaPaginada(
                    rotas
                        .Skip(filtro.ObterPrimeiroRegistroRetornar())
                        .Take(filtro.NumeroRegistros)
                        .Select(s => new Models.Rotas.V1.Lista.ListaDto(s)),
                    filtro,
                    () => rotas.Count);
            }
        }

        /// <summary>
        /// Recupera as rotas para os controles de filtro das telas.
        /// </summary>
        /// <param name="id">O identificador da rota.</param>
        /// <param name="codigo">O código da rota.</param>
        /// <returns>Uma lista JSON com as rotas encontradas.</returns>
        [HttpGet]
        [Route("filtro")]
        [SwaggerResponse(200, "Rotas encontradas.", Type = typeof(IEnumerable<IdNomeDto>))]
        [SwaggerResponse(204, "Rotas não encontradas.")]
        public IHttpActionResult ObterFiltro(int? id = null, string codigo = null)
        {
            using (var sessao = new GDATransaction())
            {
                var rotas = RotaDAO.Instance.ObtemAtivasPorIdCodigo(id, codigo)
                    .Select(p => new IdNomeDto
                    {
                        Id = p.IdRota,
                        Nome = p.CodInterno,
                    });

                return this.Lista(rotas);
            }
        }

        /// <summary>
        /// Recupera as rotas externas.
        /// </summary>
        /// <returns>Uma lista JSON com as rotas externas encontradas.</returns>
        [HttpGet]
        [Route("externa")]
        [SwaggerResponse(200, "Rotas externas encontradas.", Type = typeof(IEnumerable<IdNomeDto>))]
        [SwaggerResponse(204, "Rotas externas não encontradas.")]
        public IHttpActionResult ObterRotasExternas()
        {
            using (var sessao = new GDATransaction())
            {
                var rotasExternas = Glass.Data.Helper.DataSources.Instance.GetRotasExternas()
                    .Select(p => new IdNomeDto
                    {
                        Id = (int)p.Id,
                        Nome = p.Descr,
                    });

                return this.Lista(rotasExternas);
            }
        }
    }
}
