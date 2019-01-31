// <copyright file="GetDevolucoesPagamentoController.cs" company="Sync Softwares">
// Copyright (c) Sync Softwares. Todos os direitos reservados.
// </copyright>

using GDA;
using Glass.API.Backend.Models.DevolucoesPagamento.V1.Lista;
using Glass.Data.DAL;
using Swashbuckle.Swagger.Annotations;
using System.Collections.Generic;
using System.Linq;
using System.Web.Http;

namespace Glass.API.Backend.Controllers.DevolucoesPagamento
{
    /// <summary>
    /// Controller de devoluções de pagamentos.
    /// </summary>
    public partial class DevolucoesPagamentoController : BaseController
    {
        /// <summary>
        /// Recupera a lista de devoluções de pagamento.
        /// </summary>
        /// <param name="filtro">Os filtros para a busca das devoluções de pagamento.</param>
        /// <returns>Uma lista JSON com os dados das devoluções de pagamento.</returns>
        [HttpGet]
        [Route("")]
        [SwaggerResponse(200, "Devoluções de pagamentos encontradas sem paginação (apenas uma página de retorno) ou última página retornada", Type = typeof(IEnumerable<ListaDto>))]
        [SwaggerResponse(204, "Devoluções de pagamentos não encontradas.")]
        [SwaggerResponse(206, "Devoluções de pagamentos paginadas (qualquer página, exceto a última).", Type = typeof(IEnumerable<ListaDto>))]
        public IHttpActionResult ObterListaDevolucoesPagamento([FromUri] FiltroDto filtro)
        {
            using (var sessao = new GDATransaction())
            {
                filtro = filtro ?? new FiltroDto();

                var devolucoesPagamento = DevolucaoPagtoDAO.Instance.GetList(
                    (uint)(filtro.IdCliente ?? 0),
                    filtro.NomeCliente,
                    filtro.PeriodoCadastroInicio?.ToShortDateString(),
                    filtro.PeriodoCadastroFim?.ToShortDateString(),
                    filtro.ObterTraducaoOrdenacao(),
                    filtro.ObterPrimeiroRegistroRetornar(),
                    filtro.NumeroRegistros);

                return this.ListaPaginada(
                    devolucoesPagamento.Select(dp => new ListaDto(dp)),
                    filtro,
                    () => DevolucaoPagtoDAO.Instance.GetCount(
                        (uint)(filtro.IdCliente ?? 0),
                        filtro.NomeCliente,
                        filtro.PeriodoCadastroInicio?.ToShortDateString(),
                        filtro.PeriodoCadastroFim?.ToShortDateString()));
            }
        }
    }
}