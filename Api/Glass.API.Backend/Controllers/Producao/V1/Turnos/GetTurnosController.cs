// <copyright file="GetTurnosController.cs" company="Sync Softwares">
// Copyright (c) Sync Softwares. Todos os direitos reservados.
// </copyright>

using GDA;
using Glass.API.Backend.Helper;
using Glass.API.Backend.Helper.Respostas;
using Glass.API.Backend.Models.Genericas.V1;
using Glass.Data.DAL;
using Glass.Data.Model;
using Swashbuckle.Swagger.Annotations;
using System.Collections.Generic;
using System.Linq;
using System.Web.Http;

namespace Glass.API.Backend.Controllers.Producao.V1.Turnos
{
    /// <summary>
    /// Controller de turnos.
    /// </summary>
    public partial class TurnosController : BaseController
    {
        /// <summary>
        /// Recupera a lista de turnos.
        /// </summary>
        /// <param name="filtro">Os filtros para a busca dos turnos.</param>
        /// <returns>Uma lista JSON com os dados dos turnos.</returns>
        [HttpGet]
        [Route("")]
        [SwaggerResponse(200, "Turnos sem paginação (apenas uma página de retorno) ou última página retornada.", Type = typeof(IEnumerable<Models.Producao.V1.Turnos.Lista.ListaDto>))]
        [SwaggerResponse(204, "Turnos não encontrados para o filtro informado.")]
        [SwaggerResponse(206, "Turnos paginados (qualquer página, exceto a última).", Type = typeof(IEnumerable<Models.Producao.V1.Turnos.Lista.ListaDto>))]
        [SwaggerResponse(400, "Filtro inválido informado (campo com valor ou formato inválido).", Type = typeof(MensagemDto))]
        public IHttpActionResult ObterListaTurnos([FromUri] Models.Producao.V1.Turnos.Lista.FiltroDto filtro)
        {
            using (var sessao = new GDATransaction())
            {
                filtro = filtro ?? new Models.Producao.V1.Turnos.Lista.FiltroDto();

                var turnos = Microsoft.Practices.ServiceLocation.ServiceLocator
                    .Current.GetInstance<Global.Negocios.ITurnoFluxo>()
                    .PesquisarTurnos();

                ((Colosoft.Collections.IVirtualList)turnos).Configure(filtro.NumeroRegistros);
                ((Colosoft.Collections.ISortableCollection)turnos).ApplySort(filtro.ObterTraducaoOrdenacao());

                return this.ListaPaginada(
                    turnos
                        .Skip(filtro.ObterPrimeiroRegistroRetornar())
                        .Take(filtro.NumeroRegistros)
                        .Select(c => new Models.Producao.V1.Turnos.Lista.ListaDto(c)),
                    filtro,
                    () => turnos.Count);
            }
        }

        /// <summary>
        /// Recupera a lista de sequencias de turno.
        /// </summary>
        /// <returns>Uma lista JSON com os dados básicos das sequencias de turno.</returns>
        [HttpGet]
        [Route("sequencias")]
        [SwaggerResponse(200, "Sequencias de turno encontradas.", Type = typeof(IEnumerable<IdNomeDto>))]
        [SwaggerResponse(204, "Sequencias de turno não encontradas.")]
        public IHttpActionResult ObterSequencias()
        {
            using (var sessao = new GDATransaction())
            {
                var tipos = new ConversorEnum<TurnoSequencia>()
                    .ObterTraducao();

                return this.Lista(tipos);
            }
        }
    }
}
