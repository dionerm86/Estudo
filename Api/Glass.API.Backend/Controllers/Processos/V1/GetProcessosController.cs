// <copyright file="GetProcessosController.cs" company="Sync Softwares">
// Copyright (c) Sync Softwares. Todos os direitos reservados.
// </copyright>

using GDA;
using Glass.API.Backend.Helper;
using Glass.API.Backend.Models.Genericas;
using Glass.API.Backend.Models.Processos.Filtro;
using Glass.API.Backend.Models.Processos.Lista;
using Glass.Data.DAL;
using Swashbuckle.Swagger.Annotations;
using System.Collections.Generic;
using System.Linq;
using System.Web.Http;

namespace Glass.API.Backend.Controllers.Processos.V1
{
    /// <summary>
    /// Controller de processos (etiqueta).
    /// </summary>
    public partial class ProcessosController : BaseController
    {
        /// <summary>
        /// Recupera a lista de processos (etiqueta) para a tela de listagem.
        /// </summary>
        /// <param name="filtro">O filtro para a busca de processos.</param>
        /// <returns>Uma lista JSON com os dados dos processos de etiqueta.</returns>
        [HttpGet]
        [Route("")]
        [SwaggerResponse(200, "Processos encontrados sem paginação (apenas uma página de retorno) ou última página retornada.", Type = typeof(IEnumerable<ListaDto>))]
        [SwaggerResponse(204, "Processos não encontrados para o filtro informado.")]
        [SwaggerResponse(206, "Processos paginados (qualquer página, exceto a última).", Type = typeof(IEnumerable<ListaDto>))]
        public IHttpActionResult ObterLista([FromUri] FiltroDto filtro)
        {
            using (var sessao = new GDATransaction())
            {
                var fluxo = Microsoft.Practices.ServiceLocation.ServiceLocator
                    .Current.GetInstance<Global.Negocios.IEtiquetaFluxo>();

                var processos = fluxo.PesquisarEtiquetaProcessos();

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
        [SwaggerResponse(200, "Configurações encontradas.", Type = typeof(Models.Processos.Configuracoes.ListaDto))]
        public IHttpActionResult ObterConfiguracoes()
        {
            using (var sessao = new GDATransaction())
            {
                var configuracoes = new Models.Processos.Configuracoes.ListaDto();
                return this.Item(configuracoes);
            }
        }

        /// <summary>
        /// Recupera a lista de processos (etiqueta) para controle de filtro.
        /// </summary>
        /// <returns>Uma lista JSON com os dados dos processos de etiqueta.</returns>
        [HttpGet]
        [Route("filtro")]
        [SwaggerResponse(200, "Processos encontrados.", Type = typeof(IEnumerable<ProcessoDto>))]
        [SwaggerResponse(204, "Processos não encontrados.")]
        public IHttpActionResult ObterParaFiltro()
        {
            using (var sessao = new GDATransaction())
            {
                var processos = EtiquetaProcessoDAO.Instance.GetForFilter()
                    .Select(p => new ProcessoDto
                    {
                        Id = p.IdProcesso,
                        Codigo = p.CodInterno,
                        Aplicacao = IdCodigoDto.TentarConverter(p.IdAplicacao, p.CodAplicacao),
                    });

                return this.Lista(processos);
            }
        }

        /// <summary>
        /// Recupera a lista de tipos de processos (etiqueta) para controle de filtro.
        /// </summary>
        /// <returns>Uma lista JSON com os dados dos tipos de processos de etiqueta.</returns>
        [HttpGet]
        [Route("tipos")]
        [SwaggerResponse(200, "Tipos de processo encontrados.", Type = typeof(IEnumerable<IdNomeDto>))]
        [SwaggerResponse(204, "Tipos de processo não encontrados.")]
        public IHttpActionResult ObterTiposProcessos()
        {
            using (var sessao = new GDATransaction())
            {
                var tipos = new ConversorEnum<Data.Model.EtiquetaTipoProcesso>()
                    .ObterTraducao();

                return this.Lista(tipos);
            }
        }

        /// <summary>
        /// Recupera a lista de situações para controle de filtro na tela de processos de etiqueta.
        /// </summary>
        /// <returns>Uma lista JSON com os dados dos situações de processo de etiqueta.</returns>
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
