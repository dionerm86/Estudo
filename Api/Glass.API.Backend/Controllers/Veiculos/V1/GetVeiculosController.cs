// <copyright file="GetVeiculosController.cs" company="Sync Softwares">
// Copyright (c) Sync Softwares. Todos os direitos reservados.
// </copyright>

using GDA;
using Glass.API.Backend.Helper.Respostas;
using Glass.API.Backend.Models.Veiculos.Lista;
using Glass.Data.DAL;
using Swashbuckle.Swagger.Annotations;
using System.Collections.Generic;
using System.Linq;
using System.Web.Http;

namespace Glass.API.Backend.Controllers.Veiculos.V1
{
    /// <summary>
    /// Controller de veículos.
    /// </summary>
    public partial class VeiculosController : BaseController
    {
        /// <summary>
        /// Recupera as configurações usadas pela tela de listagem de veículos.
        /// </summary>
        /// <returns>Um objeto JSON com as configurações da tela.</returns>
        [HttpGet]
        [Route("configuracoes")]
        [SwaggerResponse(200, "Configurações recuperadas.", Type = typeof(Models.Veiculos.Configuracoes.ListaDto))]
        public IHttpActionResult ObterConfiguracoesListaVeiculos()
        {
            using (var sessao = new GDATransaction())
            {
                var configuracoes = new Models.Veiculos.Configuracoes.ListaDto();
                return this.Item(configuracoes);
            }
        }

        /// <summary>
        /// Recupera a lista de veículos.
        /// </summary>
        /// <param name="filtro">Os filtros para a busca dos itens.</param>
        /// <returns>Uma lista JSON com os dados dos itens.</returns>
        [HttpGet]
        [Route("")]
        [SwaggerResponse(200, "Veículos encontrados sem paginação (apenas uma página de retorno) ou última página retornada.", Type = typeof(IEnumerable<ListaDto>))]
        [SwaggerResponse(204, "Veículos não encontrados para o filtro informado.")]
        [SwaggerResponse(206, "Veículos paginados (qualquer página, exceto a última).", Type = typeof(IEnumerable<ListaDto>))]
        [SwaggerResponse(400, "Filtro inválido informado (campo com valor ou formato inválido).", Type = typeof(MensagemDto))]
        public IHttpActionResult ObterVeiculos([FromUri] FiltroDto filtro)
        {
            using (var sessao = new GDATransaction())
            {
                filtro = filtro ?? new FiltroDto();

                var fluxo = Microsoft.Practices.ServiceLocation.ServiceLocator
                    .Current.GetInstance<Global.Negocios.IVeiculoFluxo>();

                var veiculos = fluxo.PesquisarVeiculos();

                ((Colosoft.Collections.IVirtualList)veiculos).Configure(filtro.NumeroRegistros);
                ((Colosoft.Collections.ISortableCollection)veiculos).ApplySort(filtro.ObterTraducaoOrdenacao());

                return this.ListaPaginada(
                    veiculos
                        .Skip(filtro.ObterPrimeiroRegistroRetornar())
                        .Take(filtro.NumeroRegistros)
                        .Select(entidade => new ListaDto(entidade)),
                    filtro,
                    () => veiculos.Count);
            }
        }
    }
}
