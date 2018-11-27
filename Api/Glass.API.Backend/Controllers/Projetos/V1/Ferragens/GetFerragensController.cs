// <copyright file="GetFerragensController.cs" company="Sync Softwares">
// Copyright (c) Sync Softwares. Todos os direitos reservados.
// </copyright>

using GDA;
using Glass.API.Backend.Helper;
using Glass.API.Backend.Helper.Respostas;
using Glass.API.Backend.Models.Projetos.V1.Ferragens.Lista;
using Glass.API.Backend.Models.Genericas.V1;
using Glass.Data.Model;
using Swashbuckle.Swagger.Annotations;
using System.Collections.Generic;
using System.Linq;
using System.Web.Http;

namespace Glass.API.Backend.Controllers.Projetos.V1.Ferragens
{
    /// <summary>
    /// Controller de ferragens.
    /// </summary>
    public partial class FerragensController : BaseController
    {
        /// <summary>
        /// Recupera as configurações usadas pela tela de ferragens.
        /// </summary>
        /// <returns>Um objeto JSON com as configurações da tela.</returns>
        [HttpGet]
        [Route("configuracoes")]
        [SwaggerResponse(200, "Configurações recuperadas.", Type = typeof(Models.Producao.V1.Setores.Configuracoes.ListaDto))]
        public IHttpActionResult ObterConfiguracoesListaFerragens()
        {
            using (var sessao = new GDATransaction())
            {
                var configuracoes = new Models.Projetos.V1.Ferragens.Configuracoes.ListaDto();
                return this.Item(configuracoes);
            }
        }

        /// <summary>
        /// Recupera a lista de ferragens.
        /// </summary>
        /// <param name="filtro">Os filtros para a busca das ferragens.</param>
        /// <returns>Uma lista JSON com os dados das ferragens.</returns>
        [HttpGet]
        [Route("")]
        [SwaggerResponse(200, "Ferragens sem paginação (apenas uma página de retorno) ou última página retornada.", Type = typeof(IEnumerable<ListaDto>))]
        [SwaggerResponse(204, "Ferragens não encontradas para o filtro informado.")]
        [SwaggerResponse(206, "Ferragens paginadas (qualquer página, exceto a última).", Type = typeof(IEnumerable<ListaDto>))]
        [SwaggerResponse(400, "Filtro inválido informado (campo com valor ou formato inválido).", Type = typeof(MensagemDto))]
        public IHttpActionResult ObterListaFerragens([FromUri] FiltroDto filtro)
        {
            using (var sessao = new GDATransaction())
            {
                filtro = filtro ?? new FiltroDto();

                var ferragens = Microsoft.Practices.ServiceLocation.ServiceLocator
                    .Current.GetInstance<Projeto.Negocios.IFerragemFluxo>()
                    .PesquisarFerragem(
                    filtro.Nome,
                    filtro.IdFabricante ?? 0,
                    filtro.Codigo);

                ((Colosoft.Collections.IVirtualList)ferragens).Configure(filtro.NumeroRegistros);
                ((Colosoft.Collections.ISortableCollection)ferragens).ApplySort(filtro.ObterTraducaoOrdenacao());

                return this.ListaPaginada(
                    ferragens
                        .Skip(filtro.ObterPrimeiroRegistroRetornar())
                        .Take(filtro.NumeroRegistros)
                        .Select(c => new ListaDto(c)),
                    filtro,
                    () => ferragens.Count);
            }
        }
    }
}
