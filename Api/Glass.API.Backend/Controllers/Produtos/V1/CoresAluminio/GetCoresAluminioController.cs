// <copyright file="GetCoresAluminioController.cs" company="Sync Softwares">
// Copyright (c) Sync Softwares. Todos os direitos reservados.
// </copyright>

using GDA;
using Glass.API.Backend.Models.Genericas;
using Glass.Data.DAL;
using Swashbuckle.Swagger.Annotations;
using System.Collections.Generic;
using System.Linq;
using System.Web.Http;

namespace Glass.API.Backend.Controllers.Produtos.V1.CoresAluminio
{
    /// <summary>
    /// Controller de cores de alumínio.
    /// </summary>
    public partial class CoresAluminioController : BaseController
    {
        /// <summary>
        /// Recupera a lista de cores de alumínio para a tela de listagem.
        /// </summary>
        /// <param name="filtro">O filtro para a busca de cores de alumínio.</param>
        /// <returns>Uma lista JSON com os dados das cores de alumínio.</returns>
        [HttpGet]
        [Route("")]
        [SwaggerResponse(200, "Cores de alumínio encontradas sem paginação (apenas uma página de retorno) ou última página retornada.", Type = typeof(IEnumerable<ListaDto>))]
        [SwaggerResponse(204, "Cores de alumínio não encontradas para o filtro informado.")]
        [SwaggerResponse(206, "Cores de alumínio paginados (qualquer página, exceto a última).", Type = typeof(IEnumerable<ListaDto>))]
        public IHttpActionResult ObterLista([FromUri] FiltroDto filtro)
        {
            using (var sessao = new GDATransaction())
            {
                var fluxo = Microsoft.Practices.ServiceLocation.ServiceLocator
                    .Current.GetInstance<Global.Negocios.ICoresFluxo>();

                var coresAluminio = fluxo.PesquisarCoresAluminio();

                ((Colosoft.Collections.IVirtualList)coresAluminio).Configure(filtro.NumeroRegistros);
                ((Colosoft.Collections.ISortableCollection)coresAluminio).ApplySort(filtro.ObterTraducaoOrdenacao());

                return this.ListaPaginada(
                    coresAluminio
                        .Skip(filtro.ObterPrimeiroRegistroRetornar())
                        .Take(filtro.NumeroRegistros)
                        .Select(entidade => new ListaDto(entidade)),
                    filtro,
                    () => coresAluminio.Count);
            }
        }

        /// <summary>
        /// Recupera as cores de alumínio para os controles de filtro das telas.
        /// </summary>
        /// <returns>Uma lista JSON com as cores de alumínio encontradas.</returns>
        [HttpGet]
        [Route("filtro")]
        [SwaggerResponse(200, "Cores de alumínio encontradas.", Type = typeof(IEnumerable<IdNomeDto>))]
        [SwaggerResponse(204, "Cores de alumínio não encontradas.")]
        public IHttpActionResult ObterCoresAluminioParaFiltro()
        {
            using (var sessao = new GDATransaction())
            {
                var coresAluminio = CorAluminioDAO.Instance.GetAll()
                    .Select(c => new IdNomeDto
                    {
                        Id = c.IdCorAluminio,
                        Nome = c.Descricao,
                    });

                return this.Lista(coresAluminio);
            }
        }
    }
}
