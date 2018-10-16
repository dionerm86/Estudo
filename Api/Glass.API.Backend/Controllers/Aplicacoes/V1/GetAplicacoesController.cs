// <copyright file="GetAplicacoesController.cs" company="Sync Softwares">
// Copyright (c) Sync Softwares. Todos os direitos reservados.
// </copyright>

using GDA;
using Glass.API.Backend.Helper;
using Glass.API.Backend.Models.Aplicacoes.Lista;
using Glass.API.Backend.Models.Genericas;
using Glass.Data.DAL;
using Swashbuckle.Swagger.Annotations;
using System.Collections.Generic;
using System.Linq;
using System.Web.Http;

namespace Glass.API.Backend.Controllers.Aplicacoes.V1
{
    /// <summary>
    /// Controller de aplicações (etiqueta).
    /// </summary>
    public partial class AplicacoesController : BaseController
    {
        /// <summary>
        /// Recupera a lista de processos (etiqueta) para a tela de listagem.
        /// </summary>
        /// <param name="filtro">O filtro para a busca de processos.</param>
        /// <returns>Uma lista JSON com os dados dos processos de etiqueta.</returns>
        [HttpGet]
        [Route("")]
        [SwaggerResponse(200, "Aplicações encontradas sem paginação (apenas uma página de retorno) ou última página retornada.", Type = typeof(IEnumerable<ListaDto>))]
        [SwaggerResponse(204, "Aplicações não encontradas para o filtro informado.")]
        [SwaggerResponse(206, "Aplicações paginadas (qualquer página, exceto a última).", Type = typeof(IEnumerable<ListaDto>))]
        public IHttpActionResult ObterLista([FromUri] FiltroDto filtro)
        {
            using (var sessao = new GDATransaction())
            {
                var fluxo = Microsoft.Practices.ServiceLocation.ServiceLocator
                    .Current.GetInstance<Global.Negocios.IEtiquetaFluxo>();

                var processos = fluxo.PesquisarEtiquetaAplicacoes();

                ((Colosoft.Collections.IVirtualList)processos).Configure(filtro.NumeroRegistros);
                ((Colosoft.Collections.ISortableCollection)processos).ApplySort(filtro.ObterTraducaoOrdenacao());

                return this.ListaPaginada(
                    processos
                        .Skip(filtro.ObterPrimeiroRegistroRetornar())
                        .Take(filtro.NumeroRegistros)
                        .Select(entidade => new ListaDto(entidade)),
                    filtro,
                    () => processos.Count);
            }
        }

        /// <summary>
        /// Recupera as configurações para a tela de processos de etiquetas.
        /// </summary>
        /// <returns>Um objeto JSON com as configurações da tela.</returns>
        [HttpGet]
        [Route("configuracoes")]
        [SwaggerResponse(200, "Configurações encontradas.", Type = typeof(Models.Aplicacoes.Configuracoes.ListaDto))]
        public IHttpActionResult ObterConfiguracoes()
        {
            using (var sessao = new GDATransaction())
            {
                var configuracoes = new Models.Aplicacoes.Configuracoes.ListaDto();
                return this.Item(configuracoes);
            }
        }

        /// <summary>
        /// Recupera a lista de aplicações (etiqueta) para controle de filtro.
        /// </summary>
        /// <returns>Uma lista JSON com os dados das aplicações de etiqueta.</returns>
        [HttpGet]
        [Route("filtro")]
        [SwaggerResponse(200, "Aplicações encontradas.", Type = typeof(IEnumerable<IdCodigoDto>))]
        [SwaggerResponse(204, "Aplicações não encontradas.")]
        public IHttpActionResult ObterParaFiltro()
        {
            using (var sessao = new GDATransaction())
            {
                var processos = EtiquetaAplicacaoDAO.Instance.GetForFilter()
                    .Select(p => new IdCodigoDto
                    {
                        Id = p.IdAplicacao,
                        Codigo = p.CodInterno,
                    });

                return this.Lista(processos);
            }
        }

        /// <summary>
        /// Recupera a lista de situações para controle de filtro na tela de aplicações de etiqueta.
        /// </summary>
        /// <returns>Uma lista JSON com os dados dos situações de aplicação de etiqueta.</returns>
        [HttpGet]
        [Route("situacoes")]
        [SwaggerResponse(200, "Situações encontradas.", Type = typeof(IEnumerable<IdNomeDto>))]
        [SwaggerResponse(204, "Situações não encontradas.")]
        public IHttpActionResult ObterSituacoes()
        {
            using (var sessao = new GDATransaction())
            {
                var tipos = new ConversorEnum<Situacao>()
                    .ObterTraducao();

                return this.Lista(tipos);
            }
        }
    }
}
