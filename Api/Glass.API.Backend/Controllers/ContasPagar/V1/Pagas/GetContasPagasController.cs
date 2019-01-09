// <copyright file="GetContasPagasController.cs" company="Sync Softwares">
// Copyright (c) Sync Softwares. Todos os direitos reservados.
// </copyright>

using GDA;
using Glass.API.Backend.Helper;
using Glass.API.Backend.Helper.Respostas;
using Glass.API.Backend.Models.ContasPagar.V1.Pagas.Lista;
using Glass.API.Backend.Models.Genericas.V1;
using Glass.Data.DAL;
using Swashbuckle.Swagger.Annotations;
using System.Collections.Generic;
using System.Linq;
using System.Web.Http;

namespace Glass.API.Backend.Controllers.ContasPagar.V1.Pagas
{
    /// <summary>
    /// Controller de contas pagas.
    /// </summary>
    public partial class ContasPagasController : BaseController
    {
        /// <summary>
        /// Recupera a lista de contas pagas.
        /// </summary>
        /// <param name="filtro">Os filtros para a busca das contas pagas.</param>
        /// <returns>Uma lista JSON com os dados das contas pagas.</returns>
        [HttpGet]
        [Route("")]
        [SwaggerResponse(200, "Contas pagas encontradas sem paginação (apenas uma página de retorno) ou última página retornada.", Type = typeof(IEnumerable<ListaDto>))]
        [SwaggerResponse(204, "Contas pagas não encontradas.")]
        [SwaggerResponse(206, "Contas pagas paginadas (qualquer página, exceto a última).", Type = typeof(IEnumerable<ListaDto>))]
        public IHttpActionResult ObterListaContasPagas([FromUri] FiltroDto filtro)
        {
            using (var sessao = new GDATransaction())
            {
                filtro = filtro ?? new FiltroDto();

                var contasPagas = ContasPagarDAO.Instance.GetPagas(
                    filtro.Id ?? 0,
                    (uint)(filtro.IdCompra ?? 0),
                    filtro.NumeroNotaFiscal,
                    (uint)(filtro.IdLoja ?? 0),
                    (uint)(filtro.IdCustoFixo ?? 0),
                    (uint)(filtro.IdImpostoServico ?? 0),
                    (uint)(filtro.IdFornecedor ?? 0),
                    filtro.NomeFornecedor,
                    filtro.IdsFormaPagamento != null && filtro.IdsFormaPagamento.Any() ? string.Join(",", filtro.IdsFormaPagamento) : null,
                    filtro.PeriodoCadastroInicio?.ToShortDateString(),
                    filtro.PeriodoCadastroFim?.ToShortDateString(),
                    filtro.PeriodoPagamentoInicio?.ToShortDateString(),
                    filtro.PeriodoPagamentoFim?.ToShortDateString(),
                    filtro.PeriodoVencimentoInicio?.ToShortDateString(),
                    filtro.PeriodoVencimentoFim?.ToShortDateString(),
                    (float)(filtro.ValorVencimentoInicial ?? 0),
                    (float)(filtro.ValorVencimentoFinal ?? 0),
                    filtro.Tipo ?? 0,
                    filtro.ApenasContasDeComissao.GetValueOrDefault(false),
                    filtro.BuscarRenegociadas.GetValueOrDefault(false),
                    filtro.BuscarContasComJurosMulta.GetValueOrDefault(false),
                    filtro.PlanoConta,
                    filtro.ApenasContasDeCustoFixo.GetValueOrDefault(false),
                    filtro.BuscarContasPagar.GetValueOrDefault(false),
                    filtro.IdComissao ?? 0,
                    filtro.NumeroCte ?? 0,
                    filtro.Observacao,
                    filtro.ObterTraducaoOrdenacao(),
                    filtro.ObterPrimeiroRegistroRetornar(),
                    filtro.NumeroRegistros);

                return this.ListaPaginada(
                    contasPagas.Select(c => new ListaDto(c)),
                    filtro,
                    () => ContasPagarDAO.Instance.GetPagasCount(
                        filtro.Id ?? 0,
                        (uint)(filtro.IdCompra ?? 0),
                        filtro.NumeroNotaFiscal,
                        (uint)(filtro.IdLoja ?? 0),
                        (uint)(filtro.IdCustoFixo ?? 0),
                        (uint)(filtro.IdImpostoServico ?? 0),
                        (uint)(filtro.IdFornecedor ?? 0),
                        filtro.NomeFornecedor,
                        filtro.IdsFormaPagamento != null && filtro.IdsFormaPagamento.Any() ? string.Join(",", filtro.IdsFormaPagamento) : null,
                        filtro.PeriodoCadastroInicio?.ToShortDateString(),
                        filtro.PeriodoCadastroFim?.ToShortDateString(),
                        filtro.PeriodoPagamentoInicio?.ToShortDateString(),
                        filtro.PeriodoPagamentoFim?.ToShortDateString(),
                        filtro.PeriodoVencimentoInicio?.ToShortDateString(),
                        filtro.PeriodoVencimentoFim?.ToShortDateString(),
                        (float)(filtro.ValorVencimentoInicial ?? 0),
                        (float)(filtro.ValorVencimentoFinal ?? 0),
                        filtro.Tipo ?? 0,
                        filtro.ApenasContasDeComissao.GetValueOrDefault(false),
                        filtro.BuscarRenegociadas.GetValueOrDefault(false),
                        filtro.BuscarContasComJurosMulta.GetValueOrDefault(false),
                        filtro.PlanoConta,
                        filtro.ApenasContasDeCustoFixo.GetValueOrDefault(false),
                        filtro.BuscarContasPagar.GetValueOrDefault(false),
                        filtro.IdComissao ?? 0,
                        filtro.NumeroCte ?? 0,
                        filtro.Observacao));
            }
        }

        /// <summary>
        /// Recupera a lista de configurações para a tela de listagem de contas pagas.
        /// </summary>
        /// <returns>Uma lista JSON com os dados das configurações.</returns>
        [HttpGet]
        [Route("configuracoes")]
        [SwaggerResponse(200, "Configurações recuperadas.", Type = typeof(IEnumerable<IdNomeDto>))]
        public IHttpActionResult ObterConfiguracoes()
        {
            using (var sessao = new GDATransaction())
            {
                var configuracoes = new Models.ContasPagar.V1.Pagas.Configuracoes.ListaDto();
                return this.Item(configuracoes);
            }
        }
    }
}