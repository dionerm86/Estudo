// <copyright file="GetTiposPerdaController.cs" company="Sync Softwares">
// Copyright (c) Sync Softwares. Todos os direitos reservados.
// </copyright>

using GDA;
using Glass.API.Backend.Helper.Respostas;
using Swashbuckle.Swagger.Annotations;
using System.Collections.Generic;
using System.Linq;
using System.Web.Http;

namespace Glass.API.Backend.Controllers.Producao.V1.TiposPerda
{
    /// <summary>
    /// Controller de tipos de perda.
    /// </summary>
    public partial class TiposPerdaController : BaseController
    {
        /// <summary>
        /// Recupera a lista de tipos de perda.
        /// </summary>
        /// <param name="filtro">Os filtros para a busca dos tipos de perda.</param>
        /// <returns>Uma lista JSON com os dados dos tipos de perda.</returns>
        [HttpGet]
        [Route("")]
        [SwaggerResponse(200, "Tipos de perda sem paginação (apenas uma página de retorno) ou última página retornada.", Type = typeof(IEnumerable<Models.Producao.V1.TiposPerda.Lista.ListaDto>))]
        [SwaggerResponse(204, "Tipos de perda não encontrados para o filtro informado.")]
        [SwaggerResponse(206, "Tipos de perda paginados (qualquer página, exceto a última).", Type = typeof(IEnumerable<Models.Producao.V1.TiposPerda.Lista.ListaDto>))]
        [SwaggerResponse(400, "Filtro inválido informado (campo com valor ou formato inválido).", Type = typeof(MensagemDto))]
        public IHttpActionResult ObterListaTiposPerda([FromUri] Models.Producao.V1.TiposPerda.Lista.FiltroDto filtro)
        {
            using (var sessao = new GDATransaction())
            {
                filtro = filtro ?? new Models.Producao.V1.TiposPerda.Lista.FiltroDto();

                var tiposPerda = Microsoft.Practices.ServiceLocation.ServiceLocator
                    .Current.GetInstance<PCP.Negocios.IPerdaFluxo>()
                    .PesquisarTiposPerda();

                ((Colosoft.Collections.IVirtualList)tiposPerda).Configure(filtro.NumeroRegistros);
                ((Colosoft.Collections.ISortableCollection)tiposPerda).ApplySort(filtro.ObterTraducaoOrdenacao());

                return this.ListaPaginada(
                    tiposPerda
                        .Skip(filtro.ObterPrimeiroRegistroRetornar())
                        .Take(filtro.NumeroRegistros)
                        .Select(c => new Models.PCP.V1.TiposPerda.Lista.ListaDto(c)),
                    filtro,
                    () => tiposPerda.Count);
            }
        }
    }
}
