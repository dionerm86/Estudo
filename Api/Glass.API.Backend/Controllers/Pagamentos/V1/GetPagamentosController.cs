// <copyright file="GetPagamentosController.cs" company="Sync Softwares">
// Copyright (c) Sync Softwares. Todos os direitos reservados.
// </copyright>

using GDA;
using Glass.API.Backend.Helper.Respostas;
using Glass.Data.DAL;
using Swashbuckle.Swagger.Annotations;
using System.Collections.Generic;
using System.Linq;
using System.Web.Http;

namespace Glass.API.Backend.Controllers.Pagamentos.V1
{
    /// <summary>
    /// Controller de pagamentos.
    /// </summary>
    public partial class PagamentosController : BaseController
    {
        /// <summary>
        /// Recupera a lista de pagamentos.
        /// </summary>
        /// <param name="filtro">Os filtros para a busca dos pagamentos.</param>
        /// <returns>Uma lista JSON com os dados dos pagamentos.</returns>
        [HttpGet]
        [Route("")]
        [SwaggerResponse(200, "Pagamentos encontrados sem paginação (apenas uma página de retorno) ou última página retornada.", Type = typeof(IEnumerable<Models.Pagamentos.V1.Lista.ListaDto>))]
        [SwaggerResponse(204, "Pagamentos não encontrados.")]
        [SwaggerResponse(206, "Pagamentos paginados (qualquer página, exceto a última).", Type = typeof(IEnumerable<Models.Pagamentos.V1.Lista.ListaDto>))]
        [SwaggerResponse(400, "Filtro inválido informado (campo com valor ou formato inválido).", Type = typeof(MensagemDto))]
        public IHttpActionResult ObterListaPagamentos([FromUri] Models.Pagamentos.V1.Lista.FiltroDto filtro)
        {
            using (var sessao = new GDATransaction())
            {
                filtro = filtro ?? new Models.Pagamentos.V1.Lista.FiltroDto();

                var pagamentos = PagtoDAO.Instance.GetList(
                    (uint)(filtro.Id ?? 0),
                    (uint)(filtro.IdCompra ?? 0),
                    (uint)(filtro.IdFornecedor ?? 0),
                    filtro.NomeFornecedor,
                    filtro.PeriodoCadastroInicio?.ToShortDateString(),
                    filtro.PeriodoCadastroFim?.ToShortDateString(),
                    (float)(filtro.ValorInicial ?? 0),
                    (float)(filtro.ValorFinal ?? 0),
                    filtro.Situacao ?? 0,
                    (uint)(filtro.NumeroNotaFiscal ?? 0),
                    (uint)(filtro.IdCustoFixo ?? 0),
                    (uint)(filtro.IdImpostoServico ?? 0),
                    filtro.Observacao,
                    filtro.ObterTraducaoOrdenacao(),
                    filtro.ObterPrimeiroRegistroRetornar(),
                    filtro.NumeroRegistros);

                return this.ListaPaginada(
                    pagamentos.Select(o => new Models.Pagamentos.V1.Lista.ListaDto(o)),
                    filtro,
                    () => PagtoDAO.Instance.GetCount(
                        (uint)(filtro.Id ?? 0),
                        (uint)(filtro.IdCompra ?? 0),
                        (uint)(filtro.IdFornecedor ?? 0),
                        filtro.NomeFornecedor,
                        filtro.PeriodoCadastroInicio?.ToShortDateString(),
                        filtro.PeriodoCadastroFim?.ToShortDateString(),
                        (float)(filtro.ValorInicial ?? 0),
                        (float)(filtro.ValorFinal ?? 0),
                        filtro.Situacao ?? 0,
                        (uint)(filtro.NumeroNotaFiscal ?? 0),
                        (uint)(filtro.IdCustoFixo ?? 0),
                        (uint)(filtro.IdImpostoServico ?? 0),
                        filtro.Observacao));
            }
        }
    }
}