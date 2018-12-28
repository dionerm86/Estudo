// <copyright file="GetContasPagarController.cs" company="Sync Softwares">
// Copyright (c) Sync Softwares. Todos os direitos reservados.
// </copyright>

using GDA;
using Glass.API.Backend.Helper;
using Glass.API.Backend.Helper.Respostas;
using Glass.API.Backend.Models.ContasPagar.V1.Lista;
using Glass.API.Backend.Models.Genericas.V1;
using Glass.Configuracoes;
using Glass.Data.DAL;
using Swashbuckle.Swagger.Annotations;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Http;

namespace Glass.API.Backend.Controllers.ContasPagar.V1
{
    /// <summary>
    /// Controller de contas a pagar.
    /// </summary>
    public partial class ContasPagarController : BaseController
    {
        /// <summary>
        /// Recupera a lista de contas a pagar.
        /// </summary>
        /// <param name="filtro">Os filtros para a busca das contas a pagar.</param>
        /// <returns>Uma lista JSON com os dados das contas a pagar.</returns>
        [HttpGet]
        [Route("")]
        [SwaggerResponse(200, "Contas a pagar encontradas sem paginação (apenas uma página de retorno) ou última página retornada.", Type = typeof(IEnumerable<ListaDto>))]
        [SwaggerResponse(204, "Contas a pagar não encontradas.")]
        [SwaggerResponse(206, "Contas a pagar paginadas (qualquer página, exceto a última).", Type = typeof(IEnumerable<ListaDto>))]
        [SwaggerResponse(400, "Filtro inválido informado (campo com valor ou formato inválido).", Type = typeof(MensagemDto))]
        public IHttpActionResult ObterListaContasPagar([FromUri] FiltroDto filtro)
        {
            using (var sessao = new GDATransaction())
            {
                filtro = filtro ?? new FiltroDto();

                var carregamentos = ContasPagarDAO.Instance.GetPagtos(
                    filtro.Id ?? 0,
                    (uint)(filtro.IdCompra ?? 0),
                    filtro.NumeroNotaFiscal,
                    (uint)(filtro.IdLoja ?? 0),
                    (uint)(filtro.IdCustoFixo ?? 0),
                    (uint)(filtro.IdImpostoServico ?? 0),
                    (uint)(filtro.IdFornecedor ?? 0),
                    filtro.NomeFornecedor,
                    filtro.PeriodoVencimentoInicio?.ToShortDateString(),
                    filtro.PeriodoVencimentoFim?.ToShortDateString(),
                    filtro.PeriodoCadastroInicio?.ToShortDateString(),
                    filtro.PeriodoCadastroFim?.ToShortDateString(),
                    filtro.IdsFormaPagamento != null && filtro.IdsFormaPagamento.Any() ? filtro.IdsFormaPagamento.ToArray() : null,
                    (float)(filtro.ValorInicial ?? 0),
                    (float)(filtro.ValorFinal ?? 0),
                    filtro.BuscarCheques.GetValueOrDefault(false),
                    filtro.Tipo ?? 0,
                    filtro.BuscarPrevisaoCustoFixo.GetValueOrDefault(false),
                    filtro.ApenasContasDeComissao.GetValueOrDefault(false),
                    filtro.PlanoConta,
                    (uint)(filtro.IdPagamentoRestante ?? 0),
                    filtro.ApenasContasDeCustoFixo.GetValueOrDefault(false),
                    filtro.ApenasContasComValorAPagar.GetValueOrDefault(false),
                    filtro.PeriodoPagamentoInicio?.ToShortDateString(),
                    filtro.PeriodoPagamentoFim?.ToShortDateString(),
                    filtro.PeriodoNotaFiscalInicio?.ToShortDateString(),
                    filtro.PeriodoNotaFiscalFim?.ToShortDateString(),
                    (uint)(filtro.NumeroCte ?? 0),
                    (uint)(filtro.IdTransportadora ?? 0),
                    filtro.NomeTransportadora,
                    filtro.IdFuncionarioComissao ?? 0,
                    filtro.IdComissao ?? 0,
                    filtro.ObterTraducaoOrdenacao(),
                    filtro.ObterPrimeiroRegistroRetornar(),
                    filtro.NumeroRegistros);

                return this.ListaPaginada(
                    carregamentos.Select(c => new ListaDto(c)),
                    filtro,
                    () => ContasPagarDAO.Instance.GetPagtosCount(
                        filtro.Id ?? 0,
                        (uint)(filtro.IdCompra ?? 0),
                        filtro.NumeroNotaFiscal,
                        (uint)(filtro.IdLoja ?? 0),
                        (uint)(filtro.IdCustoFixo ?? 0),
                        (uint)(filtro.IdImpostoServico ?? 0),
                        (uint)(filtro.IdFornecedor ?? 0),
                        filtro.NomeFornecedor,
                        filtro.PeriodoVencimentoInicio?.ToShortDateString(),
                        filtro.PeriodoVencimentoFim?.ToShortDateString(),
                        filtro.PeriodoCadastroInicio?.ToShortDateString(),
                        filtro.PeriodoCadastroFim?.ToShortDateString(),
                        filtro.IdsFormaPagamento != null && filtro.IdsFormaPagamento.Any() ? filtro.IdsFormaPagamento.ToArray() : null,
                        (float)(filtro.ValorInicial ?? 0),
                        (float)(filtro.ValorFinal ?? 0),
                        filtro.BuscarCheques.GetValueOrDefault(false),
                        filtro.Tipo ?? 0,
                        filtro.BuscarPrevisaoCustoFixo.GetValueOrDefault(false),
                        filtro.ApenasContasDeComissao.GetValueOrDefault(false),
                        filtro.PlanoConta,
                        (uint)(filtro.IdPagamentoRestante ?? 0),
                        filtro.ApenasContasDeCustoFixo.GetValueOrDefault(false),
                        filtro.ApenasContasComValorAPagar.GetValueOrDefault(false),
                        filtro.PeriodoPagamentoInicio?.ToShortDateString(),
                        filtro.PeriodoPagamentoFim?.ToShortDateString(),
                        filtro.PeriodoNotaFiscalInicio?.ToShortDateString(),
                        filtro.PeriodoNotaFiscalFim?.ToShortDateString(),
                        (uint)(filtro.NumeroCte ?? 0),
                        (uint)(filtro.IdTransportadora ?? 0),
                        filtro.NomeTransportadora,
                        filtro.IdFuncionarioComissao ?? 0,
                        filtro.IdComissao ?? 0));
            }
        }

        /// <summary>
        /// Recupera a lista de tipos contábeis.
        /// </summary>
        /// <returns>Uma lista JSON com os dados dos tipos contábeis.</returns>
        [HttpGet]
        [Route("tiposContabeis")]
        [SwaggerResponse(200, "Tipos contábeis encontrados.", Type = typeof(IEnumerable<IdNomeDto>))]
        [SwaggerResponse(204, "Tipos contábeis não encontradas.")]
        public IHttpActionResult ObterTiposContabeis()
        {
            try
            {
                var tiposContabeis = new List<IdNomeDto>();

                tiposContabeis.Add(new IdNomeDto
                {
                    Id = 1,
                    Nome = FinanceiroConfig.ContasPagarReceber.DescricaoContaContabil,
                });

                tiposContabeis.Add(new IdNomeDto
                {
                    Id = 2,
                    Nome = FinanceiroConfig.ContasPagarReceber.DescricaoContaNaoContabil,
                });

                return this.Lista(tiposContabeis);
            }
            catch (Exception ex)
            {
                return this.ErroValidacao($"Erro ao obter lista de tipos contábeis.", ex);
            }
        }
    }
}