// <copyright file="GetComissionadosController.cs" company="Sync Softwares">
// Copyright (c) Sync Softwares. Todos os direitos reservados.
// </copyright>

using GDA;
using Glass.API.Backend.Helper.Respostas;
using Glass.API.Backend.Models.Comissionados.Lista;
using Glass.API.Backend.Models.Genericas;
using Glass.Data.DAL;
using Swashbuckle.Swagger.Annotations;
using System.Collections.Generic;
using System.Linq;
using System.Web.Http;

namespace Glass.API.Backend.Controllers.Comissionados.V1
{
    /// <summary>
    /// Controller de comissionados.
    /// </summary>
    public partial class ComissionadosController : BaseController
    {
        /// <summary>
        /// Recupera as configurações usadas pela tela de listagem de comissionados.
        /// </summary>
        /// <returns>Um objeto JSON com as configurações da tela.</returns>
        [HttpGet]
        [Route("configuracoes")]
        [SwaggerResponse(200, "Configurações recuperadas.", Type = typeof(Models.Comissionados.Configuracoes.ListaDto))]
        public IHttpActionResult ObterConfiguracoesListaComissionados()
        {
            using (var sessao = new GDATransaction())
            {
                var configuracoes = new Models.Comissionados.Configuracoes.ListaDto();
                return this.Item(configuracoes);
            }
        }

        /// <summary>
        /// Recupera a lista de comissionados.
        /// </summary>
        /// <param name="filtro">Os filtros para a busca dos itens.</param>
        /// <returns>Uma lista JSON com os dados dos itens.</returns>
        [HttpGet]
        [Route("")]
        [SwaggerResponse(200, "Comissionados encontrados sem paginação (apenas uma página de retorno) ou última página retornada.", Type = typeof(IEnumerable<ListaDto>))]
        [SwaggerResponse(204, "Comissionados não encontrados para o filtro informado.")]
        [SwaggerResponse(206, "Comissionados paginados (qualquer página, exceto a última).", Type = typeof(IEnumerable<ListaDto>))]
        [SwaggerResponse(400, "Filtro inválido informado (campo com valor ou formato inválido).", Type = typeof(MensagemDto))]
        public IHttpActionResult ObterComissionados([FromUri] FiltroDto filtro)
        {
            using (var sessao = new GDATransaction())
            {
                filtro = filtro ?? new FiltroDto();

                var fluxo = Microsoft.Practices.ServiceLocation.ServiceLocator
                    .Current.GetInstance<Global.Negocios.IComissionadoFluxo>();

                var comissionados = fluxo.PesquisarComissionados(
                    filtro.Nome,
                    filtro.Situacao);

                ((Colosoft.Collections.IVirtualList)comissionados).Configure(filtro.NumeroRegistros);
                ((Colosoft.Collections.ISortableCollection)comissionados).ApplySort(filtro.ObterTraducaoOrdenacao());

                return this.ListaPaginada(
                    comissionados
                        .Skip(filtro.ObterPrimeiroRegistroRetornar())
                        .Take(filtro.NumeroRegistros)
                        .Select(entidade => new ListaDto(entidade)),
                    filtro,
                    () => comissionados.Count);
            }
        }

        /// <summary>
        /// Recupera os comissionados para o controle de pesquisa.
        /// </summary>
        /// <param name="id">O identificador do comissionado para pesquisa.</param>
        /// <param name="nome">O nome do comissionado para pesquisa.</param>
        /// <returns>Uma lista JSON com os dados dos comissionados encontrados.</returns>
        [HttpGet]
        [Route("filtro")]
        [SwaggerResponse(200, "Comissionados encontrados.", Type = typeof(IEnumerable<IdNomeDto>))]
        [SwaggerResponse(204, "Comissionados não encontrados.")]
        [SwaggerResponse(400, "Filtros não informados.", Type = typeof(MensagemDto))]
        public IHttpActionResult ObterFiltro(int? id = null, string nome = null)
        {
            using (var sessao = new GDATransaction())
            {
                var comissionados = ComissionadoDAO.Instance.ObterAtivos(
                    sessao,
                    id.GetValueOrDefault(),
                    nome)
                    .Select(c => new IdNomeDto()
                    {
                        Id = c.IdComissionado,
                        Nome = c.Nome,
                    });

                return this.Lista(comissionados);
            }
        }
    }
}
