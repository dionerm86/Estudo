// <copyright file="GetTransportadoresController.cs" company="Sync Softwares">
// Copyright (c) Sync Softwares. Todos os direitos reservados.
// </copyright>

using GDA;
using Glass.API.Backend.Helper.Respostas;
using Glass.API.Backend.Models.Genericas;
using Glass.API.Backend.Models.Transportadores.Lista;
using Glass.Data.DAL;
using Swashbuckle.Swagger.Annotations;
using System.Collections.Generic;
using System.Linq;
using System.Web.Http;

namespace Glass.API.Backend.Controllers.Transportadores.V1
{
    /// <summary>
    /// Controller de transportadores.
    /// </summary>
    public partial class TransportadoresController : BaseController
    {
        /// <summary>
        /// Recupera as configurações usadas pela tela de listagem de transportadores.
        /// </summary>
        /// <returns>Um objeto JSON com as configurações da tela.</returns>
        [HttpGet]
        [Route("configuracoes")]
        [SwaggerResponse(200, "Configurações recuperadas.", Type = typeof(Models.Transportadores.Configuracoes.ListaDto))]
        public IHttpActionResult ObterConfiguracoesListaTransportadores()
        {
            using (var sessao = new GDATransaction())
            {
                var configuracoes = new Models.Transportadores.Configuracoes.ListaDto();
                return this.Item(configuracoes);
            }
        }

        /// <summary>
        /// Recupera a lista de transportadores.
        /// </summary>
        /// <param name="filtro">Os filtros para a busca dos itens.</param>
        /// <returns>Uma lista JSON com os dados dos itens.</returns>
        [HttpGet]
        [Route("")]
        [SwaggerResponse(200, "Transportadores encontrados sem paginação (apenas uma página de retorno) ou última página retornada.", Type = typeof(IEnumerable<ListaDto>))]
        [SwaggerResponse(204, "Transportadores não encontrados para o filtro informado.")]
        [SwaggerResponse(206, "Transportadores paginados (qualquer página, exceto a última).", Type = typeof(IEnumerable<ListaDto>))]
        [SwaggerResponse(400, "Filtro inválido informado (campo com valor ou formato inválido).", Type = typeof(MensagemDto))]
        public IHttpActionResult ObterAcertos([FromUri] FiltroDto filtro)
        {
            using (var sessao = new GDATransaction())
            {
                filtro = filtro ?? new FiltroDto();

                var fluxo = Microsoft.Practices.ServiceLocation.ServiceLocator
                    .Current.GetInstance<Global.Negocios.ITransportadorFluxo>();

                var transportadores = fluxo.PesquisarTransportadores(
                    filtro.Id ?? 0,
                    filtro.Nome,
                    null);

                ((Colosoft.Collections.IVirtualList)transportadores).Configure(filtro.NumeroRegistros);
                ((Colosoft.Collections.ISortableCollection)transportadores).ApplySort(filtro.ObterTraducaoOrdenacao());

                return this.ListaPaginada(
                    transportadores
                        .Skip(filtro.ObterPrimeiroRegistroRetornar())
                        .Take(filtro.NumeroRegistros)
                        .Select(entidade => new ListaDto(entidade)),
                    filtro,
                    () => transportadores.Count);
            }
        }

        /// <summary>
        /// Obtém uma lista com todos os transportadores
        /// </summary>
        /// <returns>Uma lista JSON com os dados básicos de todos transportadores</returns>
        [HttpGet]
        [Route("filtro")]
        [SwaggerResponse(200, "Transportadores encontrados.", Type = typeof(IEnumerable<IdNomeDto>))]
        [SwaggerResponse(204, "Transportadores não encontrados.")]
        public IHttpActionResult ObterTransportadores()
        {
            using (var sessao = new GDATransaction())
            {
                var transportadores = TransportadorDAO.Instance.GetOrdered()
                    .Select(f => new IdNomeDto
                    {
                        Id = f.IdTransportador,
                        Nome = f.Nome,
                    });

                return this.Lista(transportadores);
            }
        }
    }
}
