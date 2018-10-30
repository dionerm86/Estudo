// <copyright file="GetClassificacoesRoteiroController.cs" company="Sync Softwares">
// Copyright (c) Sync Softwares. Todos os direitos reservados.
// </copyright>

using GDA;
using Glass.API.Backend.Helper.Respostas;
using Swashbuckle.Swagger.Annotations;
using System.Collections.Generic;
using System.Linq;
using System.Web.Http;

namespace Glass.API.Backend.Controllers.Producao.V1.Roteiros.ClassificacoesRoteiro
{
    /// <summary>
    /// Controller de classificações de roteiro.
    /// </summary>
    public partial class ClassificacoesRoteiroController : BaseController
    {
        /// <summary>
        /// Recupera a lista de Classificações de roteiro.
        /// </summary>
        /// <param name="filtro">Os filtros para a busca das classificações de roteiro.</param>
        /// <returns>Uma lista JSON com os dados das classificações de roteiro.</returns>
        [HttpGet]
        [Route("")]
        [SwaggerResponse(200, "Classificações de roteiro sem paginação (apenas uma página de retorno) ou última página retornada.", Type = typeof(IEnumerable<Models.Producao.V1.Roteiros.ClassificacoesRoteiro.Lista.ListaDto>))]
        [SwaggerResponse(204, "Classificações de roteiro não encontrados para o filtro informado.")]
        [SwaggerResponse(206, "Classificações de roteiro paginados (qualquer página, exceto a última).", Type = typeof(IEnumerable<Models.Producao.V1.Roteiros.ClassificacoesRoteiro.Lista.ListaDto>))]
        [SwaggerResponse(400, "Filtro inválido informado (campo com valor ou formato inválido).", Type = typeof(MensagemDto))]
        public IHttpActionResult ObterListaClassificacoesRoteiro([FromUri] Models.Producao.V1.Roteiros.ClassificacoesRoteiro.Lista.FiltroDto filtro)
        {
            using (var sessao = new GDATransaction())
            {
                filtro = filtro ?? new Models.Producao.V1.Roteiros.ClassificacoesRoteiro.Lista.FiltroDto();

                var tiposPerda = Microsoft.Practices.ServiceLocation.ServiceLocator
                    .Current.GetInstance<PCP.Negocios.IClassificacaoRoteiroProducaoFluxo>()
                    .PesquisarClassificacao();

                ((Colosoft.Collections.IVirtualList)tiposPerda).Configure(filtro.NumeroRegistros);
                ((Colosoft.Collections.ISortableCollection)tiposPerda).ApplySort(filtro.ObterTraducaoOrdenacao());

                return this.ListaPaginada(
                    tiposPerda
                        .Skip(filtro.ObterPrimeiroRegistroRetornar())
                        .Take(filtro.NumeroRegistros)
                        .Select(c => new Models.Producao.V1.Roteiros.ClassificacoesRoteiro.Lista.ListaDto(c)),
                    filtro,
                    () => tiposPerda.Count);
            }
        }
    }
}
