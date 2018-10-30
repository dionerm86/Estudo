// <copyright file="GetPlanosContaController.cs" company="Sync Softwares">
// Copyright (c) Sync Softwares. Todos os direitos reservados.
// </copyright>

using GDA;
using Glass.API.Backend.Helper;
using Glass.API.Backend.Helper.Respostas;
using Glass.API.Backend.Models.Genericas.V1;
using Glass.API.Backend.Models.PlanosConta.V1.Lista;
using Glass.Data.DAL;
using Swashbuckle.Swagger.Annotations;
using System.Collections.Generic;
using System.Linq;
using System.Web.Http;

namespace Glass.API.Backend.Controllers.PlanosConta.V1
{
    /// <summary>
    /// Controller de parcelas.
    /// </summary>
    public partial class PlanosContaController : BaseController
    {
        /// <summary>
        /// Recupera a lista de planos de conta.
        /// </summary>
        /// <param name="filtro">Os filtros para a busca dos planos de conta.</param>
        /// <returns>Uma lista JSON com os dados dos planos de conta.</returns>
        [HttpGet]
        [Route("")]
        [SwaggerResponse(200, "Planos de conta sem paginação (apenas uma página de retorno) ou última página retornada.", Type = typeof(IEnumerable<ListaDto>))]
        [SwaggerResponse(204, "Planos de conta não encontrados para o filtro informado.")]
        [SwaggerResponse(206, "Planos de conta paginados (qualquer página, exceto a última).", Type = typeof(IEnumerable<ListaDto>))]
        [SwaggerResponse(400, "Filtro inválido informado (campo com valor ou formato inválido).", Type = typeof(MensagemDto))]
        public IHttpActionResult ObterListaPlanosConta([FromUri] FiltroDto filtro)
        {
            using (var sessao = new GDATransaction())
            {
                filtro = filtro ?? new FiltroDto();

                var planosConta = Microsoft.Practices.ServiceLocation.ServiceLocator
                    .Current.GetInstance<Financeiro.Negocios.IPlanoContasFluxo>()
                    .PesquisarPlanosContas(
                        filtro.IdGrupoConta ?? 0,
                        filtro.Situacao);

                ((Colosoft.Collections.IVirtualList)planosConta).Configure(filtro.NumeroRegistros);
                ((Colosoft.Collections.ISortableCollection)planosConta).ApplySort(filtro.ObterTraducaoOrdenacao());

                return this.ListaPaginada(
                    planosConta
                        .Skip(filtro.ObterPrimeiroRegistroRetornar())
                        .Take(filtro.NumeroRegistros)
                        .Select(c => new ListaDto(c)),
                    filtro,
                    () => planosConta.Count);
            }
        }

        /// <summary>
        /// Recupera os planos de conta do sistema para os controles de filtro das telas.
        /// </summary>
        /// <param name="tipo">Tipo do plano de conta (Crédito ou débito)</param>
        /// <returns>Uma lista JSON com as parcelas encontradas.</returns>
        [HttpGet]
        [Route("filtro")]
        [SwaggerResponse(200, "Planos de conta encontrados.", Type = typeof(IEnumerable<IdNomeDto>))]
        [SwaggerResponse(204, "Planos de conta não encontrados.")]
        public IHttpActionResult ObterPlanosConta(Tipo tipo)
        {
            using (var sessao = new GDATransaction())
            {
                var planosConta = PlanoContasDAO.Instance.GetPlanoContas((int)tipo)
                    .Select(p => new IdNomeDto
                    {
                        Id = p.IdConta,
                        Nome = p.DescrPlanoGrupo,
                    });

                return this.Lista(planosConta);
            }
        }
    }
}
