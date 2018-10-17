// <copyright file="GetCoresVidroController.cs" company="Sync Softwares">
// Copyright (c) Sync Softwares. Todos os direitos reservados.
// </copyright>

using GDA;
using Glass.API.Backend.Models.Genericas.V1;
using Glass.API.Backend.Models.Produtos.V1.CoresVidro.Lista;
using Glass.Data.DAL;
using Swashbuckle.Swagger.Annotations;
using System.Collections.Generic;
using System.Linq;
using System.Web.Http;

namespace Glass.API.Backend.Controllers.Produtos.V1.CoresVidro
{
    /// <summary>
    /// Controller de cores de vidro.
    /// </summary>
    public partial class CoresVidroController : BaseController
    {
        /// <summary>
        /// Recupera a lista de cores de vidro para a tela de listagem.
        /// </summary>
        /// <param name="filtro">O filtro para a busca de cores de vidro.</param>
        /// <returns>Uma lista JSON com os dados das cores de vidro.</returns>
        [HttpGet]
        [Route("")]
        [SwaggerResponse(200, "Cores de vidro encontradas sem paginação (apenas uma página de retorno) ou última página retornada.", Type = typeof(IEnumerable<ListaDto>))]
        [SwaggerResponse(204, "Cores de vidro não encontradas para o filtro informado.")]
        [SwaggerResponse(206, "Cores de vidro paginados (qualquer página, exceto a última).", Type = typeof(IEnumerable<ListaDto>))]
        public IHttpActionResult ObterLista([FromUri] FiltroDto filtro)
        {
            using (var sessao = new GDATransaction())
            {
                var fluxo = Microsoft.Practices.ServiceLocation.ServiceLocator
                    .Current.GetInstance<Global.Negocios.ICoresFluxo>();

                var coresVidro = fluxo.PesquisarCoresVidro();

                ((Colosoft.Collections.IVirtualList)coresVidro).Configure(filtro.NumeroRegistros);
                ((Colosoft.Collections.ISortableCollection)coresVidro).ApplySort(filtro.ObterTraducaoOrdenacao());

                return this.ListaPaginada(
                    coresVidro
                        .Skip(filtro.ObterPrimeiroRegistroRetornar())
                        .Take(filtro.NumeroRegistros)
                        .Select(entidade => new ListaDto(entidade)),
                    filtro,
                    () => coresVidro.Count);
            }
        }

        /// <summary>
        /// Recupera as cores de vidro para os controles de filtro das telas.
        /// </summary>
        /// <returns>Uma lista JSON com as cores de vidro encontradas.</returns>
        [HttpGet]
        [Route("filtro")]
        [SwaggerResponse(200, "Cores de vidro encontradas.", Type = typeof(IEnumerable<IdNomeDto>))]
        [SwaggerResponse(204, "Cores de vidro não encontradas.")]
        public IHttpActionResult ObterCoresVidroParaFiltro()
        {
            using (var sessao = new GDATransaction())
            {
                var coresVidro = CorVidroDAO.Instance.GetForFiltro()
                    .Select(c => new IdNomeDto
                    {
                        Id = c.IdCorVidro,
                        Nome = c.Descricao,
                    });

                return this.Lista(coresVidro);
            }
        }
    }
}
