// <copyright file="GetSeguradorasController.cs" company="Sync Softwares">
// Copyright (c) Sync Softwares. Todos os direitos reservados.
// </copyright>

using GDA;
using Glass.API.Backend.Helper;
using Glass.API.Backend.Helper.Respostas;
using Swashbuckle.Swagger.Annotations;
using System.Collections.Generic;
using System.Linq;
using System.Web.Http;

namespace Glass.API.Backend.Controllers.Seguradoras.V1
{
    /// <summary>
    /// Controller de seguradora.
    /// </summary>
    public partial class SeguradorasController : BaseController
    {
        /// <summary>
        /// Recupera a lista de seguradora.
        /// </summary>
        /// <param name="filtro">Os filtros para a busca das seguradoras.</param>
        /// <returns>Uma lista JSON com os dados das seguradoras.</returns>
        [HttpGet]
        [Route("")]
        [SwaggerResponse(200, "Seguradoras sem paginação (apenas uma página de retorno) ou última página retornada.", Type = typeof(IEnumerable<Models.Seguradoras.V1.Lista.ListaDto>))]
        [SwaggerResponse(204, "Seguradoras não encontradas para o filtro informado.")]
        [SwaggerResponse(206, "Seguradoras paginadas (qualquer página, exceto a última).", Type = typeof(IEnumerable<Models.Seguradoras.V1.Lista.ListaDto>))]
        [SwaggerResponse(400, "Filtro inválido informado (campo com valor ou formato inválido).", Type = typeof(MensagemDto))]
        public IHttpActionResult ObterListaSeguradoras([FromUri] Models.Seguradoras.V1.Lista.FiltroDto filtro)
        {
            using (var sessao = new GDATransaction())
            {
                filtro = filtro ?? new Models.Seguradoras.V1.Lista.FiltroDto();

                var seguradoras = Microsoft.Practices.ServiceLocation.ServiceLocator
                    .Current.GetInstance<Fiscal.Negocios.ICTeFluxo>()
                    .PesquisarSeguradoras();

                ((Colosoft.Collections.IVirtualList)seguradoras).Configure(filtro.NumeroRegistros);
                ((Colosoft.Collections.ISortableCollection)seguradoras).ApplySort(filtro.ObterTraducaoOrdenacao());

                return this.ListaPaginada(
                    seguradoras
                        .Skip(filtro.ObterPrimeiroRegistroRetornar())
                        .Take(filtro.NumeroRegistros)
                        .Select(c => new Models.Seguradoras.V1.Lista.ListaDto(c)),
                    filtro,
                    () => seguradoras.Count);
            }
        }
    }
}
