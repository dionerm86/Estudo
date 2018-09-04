// <copyright file="GetProcessosController.cs" company="Sync Softwares">
// Copyright (c) Sync Softwares. Todos os direitos reservados.
// </copyright>

using GDA;
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
    }
}
