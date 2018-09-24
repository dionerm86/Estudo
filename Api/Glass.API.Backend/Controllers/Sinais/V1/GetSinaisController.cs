// <copyright file="GetSinaisController.cs" company="Sync Softwares">
// Copyright (c) Sync Softwares. Todos os direitos reservados.
// </copyright>

using GDA;
using Glass.API.Backend.Helper.Respostas;
using Glass.Data.DAL;
using Swashbuckle.Swagger.Annotations;
using System.Collections.Generic;
using System.Linq;
using System.Web.Http;

namespace Glass.API.Backend.Controllers.Sinais.V1
{
    /// <summary>
    /// Controller de sinais.
    /// </summary>
    public partial class SinaisController : BaseController
    {
        /// <summary>
        /// Recupera a lista de sinais.
        /// </summary>
        /// <param name="filtro">Os filtros para a busca dos itens.</param>
        /// <returns>Uma lista JSON com os dados dos itens.</returns>
        [HttpGet]
        [Route("")]
        [SwaggerResponse(200, "Sinais encontrados sem paginação (apenas uma página de retorno) ou última página retornada.", Type = typeof(IEnumerable<Models.Sinais.Lista.ListaDto>))]
        [SwaggerResponse(204, "Sinais não encontrados para o filtro informado.")]
        [SwaggerResponse(206, "Sinais paginados (qualquer página, exceto a última).", Type = typeof(IEnumerable<Models.Sinais.Lista.ListaDto>))]
        [SwaggerResponse(400, "Filtro inválido informado (campo com valor ou formato inválido).", Type = typeof(MensagemDto))]
        public IHttpActionResult ObterSinais([FromUri] Models.Sinais.Lista.FiltroDto filtro)
        {
            using (var sessao = new GDATransaction())
            {
                filtro = filtro ?? new Models.Sinais.Lista.FiltroDto();

                var sinais = SinalDAO.Instance.GetList(
                    (uint)(filtro.Id ?? 0),
                    (uint)(filtro.IdPedido ?? 0),
                    (uint)(filtro.IdCliente ?? 0),
                    filtro.PeriodoCadastroInicio?.ToShortDateString(),
                    filtro.PeriodoCadastroFim?.ToShortDateString(),
                    0,
                    filtro.PagamentoAntecipado,
                    0,
                    filtro.ObterTraducaoOrdenacao(),
                    filtro.ObterPrimeiroRegistroRetornar(),
                    filtro.NumeroRegistros);

                return this.ListaPaginada(
                    sinais.Select(o => new Models.Sinais.Lista.ListaDto(o)),
                    filtro,
                    () => SinalDAO.Instance.GetCount(
                        (uint)(filtro.Id ?? 0),
                        (uint)(filtro.IdPedido ?? 0),
                        (uint)(filtro.IdCliente ?? 0),
                        filtro.PeriodoCadastroInicio?.ToShortDateString(),
                        filtro.PeriodoCadastroFim?.ToShortDateString(),
                        0,
                        filtro.PagamentoAntecipado,
                        0));
            }
        }
    }
}
