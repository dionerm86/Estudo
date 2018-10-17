// <copyright file="GetObservacoesFinanceiroController.cs" company="Sync Softwares">
// Copyright (c) Sync Softwares. Todos os direitos reservados.
// </copyright>

using GDA;
using Glass.API.Backend.Helper.Respostas;
using Glass.API.Backend.Models.Pedidos.V1.ObservacoesFinanceiro.Lista;
using Glass.Data.DAL;
using Swashbuckle.Swagger.Annotations;
using System.Collections.Generic;
using System.Linq;
using System.Web.Http;

namespace Glass.API.Backend.Controllers.Pedidos.V1.ObservacoesFinanceiro
{
    /// <summary>
    /// Controller de observações financeiras de pedido.
    /// </summary>
    public partial class ObservacoesFinanceiroController : ApiController
    {
        /// <summary>
        /// Recupera a lista de observações do financeiro para um pedido.
        /// </summary>
        /// <param name="idPedido">O identificador do pedido.</param>
        /// <param name="filtro">Os filtros de entrada para o método.</param>
        /// <returns>Uma lista JSON com as observações do financeiro encontradas.</returns>
        [HttpGet]
        [Route("")]
        [SwaggerResponse(200, "Observações do financeiro sem paginação (apenas uma página de retorno) ou última página retornada.", Type = typeof(IEnumerable<ListaObservacaoFinanceiroDto>))]
        [SwaggerResponse(204, "Observações do financeiro não encontrados para o filtro informado.")]
        [SwaggerResponse(206, "Observações do financeiro paginadas (qualquer página, exceto a última).", Type = typeof(IEnumerable<ListaObservacaoFinanceiroDto>))]
        [SwaggerResponse(400, "Erro de valor ou formato do campo id.", Type = typeof(MensagemDto))]
        [SwaggerResponse(404, "Pedido informado não encontrado.", Type = typeof(MensagemDto))]
        public IHttpActionResult ObterObservacoesFinanceiro(int idPedido, [FromUri] FiltroDto filtro)
        {
            using (var sessao = new GDATransaction())
            {
                var validacao = this.ValidarIdPedido(idPedido);

                if (validacao != null)
                {
                    return validacao;
                }

                filtro = filtro ?? new FiltroDto();

                var observacoes = ObservacaoFinalizacaoFinanceiroDAO.Instance.ObtemObservacoesFinalizacao(
                    (uint)idPedido,
                    0,
                    null,
                    0,
                    null,
                    null,
                    null,
                    filtro.ObterTraducaoOrdenacao(),
                    filtro.ObterPrimeiroRegistroRetornar(),
                    filtro.NumeroRegistros);

                return this.ListaPaginada(
                    observacoes.Select(o => new ListaObservacaoFinanceiroDto(o)),
                    filtro,
                    () => ObservacaoFinalizacaoFinanceiroDAO.Instance.ObtemNumeroObservacoesFinalizacao(
                        (uint)idPedido,
                        0,
                        null,
                        0,
                        null,
                        null,
                        null));
            }
        }
    }
}
