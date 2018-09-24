// <copyright file="GetCoresFerragemController.cs" company="Sync Softwares">
// Copyright (c) Sync Softwares. Todos os direitos reservados.
// </copyright>

using GDA;
using Glass.API.Backend.Models.Genericas;
using Glass.API.Backend.Models.Produtos.CoresFerragem.Lista;
using Glass.Data.DAL;
using Swashbuckle.Swagger.Annotations;
using System.Collections.Generic;
using System.Linq;
using System.Web.Http;

namespace Glass.API.Backend.Controllers.Produtos.V1.CoresFerragem
{
    /// <summary>
    /// Controller de cores de ferragem.
    /// </summary>
    public partial class CoresFerragemController : BaseController
    {
        /// <summary>
        /// Recupera a lista de cores de ferragem para a tela de listagem.
        /// </summary>
        /// <param name="filtro">O filtro para a busca de cores de ferragem.</param>
        /// <returns>Uma lista JSON com os dados das cores de ferragem.</returns>
        [HttpGet]
        [Route("")]
        [SwaggerResponse(200, "Cores de ferragem encontradas sem paginação (apenas uma página de retorno) ou última página retornada.", Type = typeof(IEnumerable<ListaDto>))]
        [SwaggerResponse(204, "Cores de ferragem não encontradas para o filtro informado.")]
        [SwaggerResponse(206, "Cores de ferragem paginados (qualquer página, exceto a última).", Type = typeof(IEnumerable<ListaDto>))]
        public IHttpActionResult ObterLista([FromUri] FiltroDto filtro)
        {
            using (var sessao = new GDATransaction())
            {
                var fluxo = Microsoft.Practices.ServiceLocation.ServiceLocator
                    .Current.GetInstance<Global.Negocios.ICoresFluxo>();

                var coresFerragem = fluxo.PesquisarCoresFerragem();

                ((Colosoft.Collections.IVirtualList)coresFerragem).Configure(filtro.NumeroRegistros);
                ((Colosoft.Collections.ISortableCollection)coresFerragem).ApplySort(filtro.ObterTraducaoOrdenacao());

                return this.ListaPaginada(
                    coresFerragem
                        .Skip(filtro.ObterPrimeiroRegistroRetornar())
                        .Take(filtro.NumeroRegistros)
                        .Select(entidade => new ListaDto(entidade)),
                    filtro,
                    () => coresFerragem.Count);
            }
        }

        /// <summary>
        /// Recupera as cores de ferragem para os controles de filtro das telas.
        /// </summary>
        /// <returns>Uma lista JSON com as cores de ferragem encontradas.</returns>
        [HttpGet]
        [Route("filtro")]
        [SwaggerResponse(200, "Cores de ferragem encontradas.", Type = typeof(IEnumerable<IdNomeDto>))]
        [SwaggerResponse(204, "Cores de ferragem não encontradas.")]
        public IHttpActionResult ObterCoresFerragemParaFiltro()
        {
            using (var sessao = new GDATransaction())
            {
                var coresFerragem = CorFerragemDAO.Instance.GetAll()
                    .Select(c => new IdNomeDto
                    {
                        Id = c.IdCorFerragem,
                        Nome = c.Descricao,
                    });

                return this.Lista(coresFerragem);
            }
        }
    }
}
