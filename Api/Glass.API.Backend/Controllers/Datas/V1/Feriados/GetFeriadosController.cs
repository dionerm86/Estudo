// <copyright file="GetCoresVidroController.cs" company="Sync Softwares">
// Copyright (c) Sync Softwares. Todos os direitos reservados.
// </copyright>

using GDA;
using Glass.API.Backend.Models.Genericas;
using Glass.API.Backend.Models.Datas.Feriados.Lista;
using Glass.Data.DAL;
using Swashbuckle.Swagger.Annotations;
using System.Collections.Generic;
using System.Linq;
using System.Web.Http;

namespace Glass.API.Backend.Controllers.Datas.V1.Feriados
{
    /// <summary>
    /// Controller de feriados.
    /// </summary>
    public partial class FeriadosController : BaseController
    {
        /// <summary>
        /// Recupera a lista de feriados para a tela de listagem.
        /// </summary>
        /// <param name="filtro">O filtro para a busca de feriados.</param>
        /// <returns>Uma lista JSON com os dados dos feriados.</returns>
        [HttpGet]
        [Route("")]
        [SwaggerResponse(200, "Feriados encontrados sem paginação (apenas uma página de retorno) ou última página retornada.", Type = typeof(IEnumerable<ListaDto>))]
        [SwaggerResponse(204, "Feriados não encontrados para o filtro informado.")]
        [SwaggerResponse(206, "Feriados paginados (qualquer página, exceto a última).", Type = typeof(IEnumerable<ListaDto>))]
        public IHttpActionResult ObterLista([FromUri] FiltroDto filtro)
        {
            using (var sessao = new GDATransaction())
            {
                var fluxo = Microsoft.Practices.ServiceLocation.ServiceLocator
                    .Current.GetInstance<Global.Negocios.IDataFluxo>();

                var feriados = fluxo.PesquisarFeriados();

                ((Colosoft.Collections.IVirtualList)feriados).Configure(filtro.NumeroRegistros);
                ((Colosoft.Collections.ISortableCollection)feriados).ApplySort(filtro.ObterTraducaoOrdenacao());

                return this.ListaPaginada(
                    feriados
                        .Skip(filtro.ObterPrimeiroRegistroRetornar())
                        .Take(filtro.NumeroRegistros)
                        .Select(entidade => new ListaDto(entidade)),
                    filtro,
                    () => feriados.Count);
            }
        }
    }
}
