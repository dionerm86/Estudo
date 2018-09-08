// <copyright file="GetContasReceberController.cs" company="Sync Softwares">
// Copyright (c) Sync Softwares. Todos os direitos reservados.
// </copyright>

using GDA;
using Glass.API.Backend.Helper.Respostas;
using Glass.API.Backend.Models.Genericas;
using Glass.Configuracoes;
using Glass.Data.DAL;
using Swashbuckle.Swagger.Annotations;
using System.Collections.Generic;
using System.Linq;
using System.Web.Http;

namespace Glass.API.Backend.Controllers.ContasReceber.V1
{
    /// <summary>
    /// Controller de contas a receber/recebidas.
    /// </summary>
    public partial class ContasReceberController : BaseController
    {
        /// <summary>
        /// Recupera as configurações usadas pela tela de listagem de contas recebidas.
        /// </summary>
        /// <returns>Um objeto JSON com as configurações da tela.</returns>
        [HttpGet]
        [Route("configuracoes/recebidas")]
        [SwaggerResponse(200, "Configurações recuperadas.", Type = typeof(Models.ContasReceber.Configuracoes.ListaRecebidasDto))]
        public IHttpActionResult ObterConfiguracoesListaContasRecebidas()
        {
            using (var sessao = new GDATransaction())
            {
                var configuracoes = new Models.ContasReceber.Configuracoes.ListaRecebidasDto();
                return this.Item(configuracoes);
            }
        }

        /// <summary>
        /// Recupera a lista de contas recebidas.
        /// </summary>
        /// <param name="filtro">Os filtros para a busca das contas recebidas.</param>
        /// <returns>Uma lista JSON com os dados das contas recebidas.</returns>
        [HttpGet]
        [Route("recebidas")]
        [SwaggerResponse(200, "Contas recebidas sem paginação (apenas uma página de retorno) ou última página retornada.", Type = typeof(IEnumerable<Models.ContasReceber.ListaRecebidas.ListaDto>))]
        [SwaggerResponse(204, "Contas recebidas não encontradas para o filtro informado.")]
        [SwaggerResponse(206, "Contas recebidas paginados (qualquer página, exceto a última).", Type = typeof(IEnumerable<Models.ContasReceber.ListaRecebidas.ListaDto>))]
        [SwaggerResponse(400, "Filtro inválido informado (campo com valor ou formato inválido).", Type = typeof(MensagemDto))]
        public IHttpActionResult ObterListaContasRecebidas([FromUri] Models.ContasReceber.ListaRecebidas.FiltroDto filtro)
        {
            using (var sessao = new GDATransaction())
            {
                filtro = filtro ?? new Models.ContasReceber.ListaRecebidas.FiltroDto();

                var contasRecebidas = ContasReceberDAO.Instance.GetForListRpt(
                    (uint)(filtro.IdPedido ?? 0),
                    (uint)(filtro.IdLiberarPedido ?? 0),
                    (uint)(filtro.IdAcerto ?? 0),
                    (uint)(filtro.IdAcertoParcial ?? 0),
                    (uint)(filtro.IdTrocaDevolucao ?? 0),
                    (uint)(filtro.NumeroNfe ?? 0),
                    (uint)(filtro.IdLoja ?? 0),
                    (uint)(filtro.IdVendedor ?? 0),
                    (uint)(filtro.RecebidaPor ?? 0),
                    (uint)(filtro.IdCliente ?? 0),
                    (uint)(filtro.TipoEntrega ?? 0),
                    filtro.NomeCliente,
                    filtro.PeriodoVencimentoInicio?.ToShortDateString(),
                    filtro.PeriodoVencimentoFim?.ToShortDateString(),
                    filtro.PeriodoRecebimentoInicio?.ToShortDateString(),
                    filtro.PeriodoRecebimentoFim?.ToShortDateString(),
                    filtro.PeriodoCadastroInicio?.ToShortDateString(),
                    filtro.PeriodoCadastroFim?.ToShortDateString(),
                    filtro.FormasPagamento != null && filtro.FormasPagamento.Any() ? string.Join(",", filtro.FormasPagamento) : null,
                    0,
                    (float)filtro.ValorRecebidoInicio,
                    (float)filtro.ValorRecebidoFim,
                    filtro.BuscarContasRenegociadas,
                    !filtro.BuscarContasAReceber,
                    (uint)(filtro.IdComissionado ?? 0),
                    (uint)(filtro.IdRota ?? 0),
                    filtro.Observacao,
                    0,
                    filtro.TiposContabeis != null && filtro.TiposContabeis.Any() ? string.Join(",", filtro.TiposContabeis) : null,
                    (uint)(filtro.NumeroArquivoRemessa ?? 0),
                    filtro.BuscarContasDeObra,
                    filtro.BuscaArquivoRemessa.GetValueOrDefault(1),
                    filtro.IdVendedorAssociadoCliente ?? 0,
                    filtro.IdVendedorObra ?? 0,
                    filtro.IdComissao ?? 0,
                    filtro.IdSinal ?? 0,
                    filtro.IdCte ?? 0,
                    filtro.BuscarContasProtestadas,
                    filtro.BuscarContasVinculadas,
                    filtro.BuscaNotaFiscal != null && filtro.BuscaNotaFiscal.Any() ? string.Join(",", filtro.BuscaNotaFiscal) : null,
                    filtro.NumeroAutorizacaoCartao,
                    filtro.ObterTraducaoOrdenacao(),
                    filtro.ObterPrimeiroRegistroRetornar(),
                    filtro.NumeroRegistros);

                return this.ListaPaginada(
                    contasRecebidas.Select(c => new Models.ContasReceber.ListaRecebidas.ListaDto(c)),
                    filtro,
                    () => ContasReceberDAO.Instance.GetRptCount(
                        (uint)(filtro.IdPedido ?? 0),
                        (uint)(filtro.IdLiberarPedido ?? 0),
                        (uint)(filtro.IdAcerto ?? 0),
                        (uint)(filtro.IdAcertoParcial ?? 0),
                        (uint)(filtro.IdTrocaDevolucao ?? 0),
                        (uint)(filtro.NumeroNfe ?? 0),
                        (uint)(filtro.IdLoja ?? 0),
                        (uint)(filtro.IdVendedor ?? 0),
                        (uint)(filtro.RecebidaPor ?? 0),
                        (uint)(filtro.IdCliente ?? 0),
                        (uint)(filtro.TipoEntrega ?? 0),
                        filtro.NomeCliente,
                        filtro.PeriodoVencimentoInicio?.ToShortDateString(),
                        filtro.PeriodoVencimentoFim?.ToShortDateString(),
                        filtro.PeriodoRecebimentoInicio?.ToShortDateString(),
                        filtro.PeriodoRecebimentoFim?.ToShortDateString(),
                        filtro.PeriodoCadastroInicio?.ToShortDateString(),
                        filtro.PeriodoCadastroFim?.ToShortDateString(),
                        filtro.FormasPagamento != null && filtro.FormasPagamento.Any() ? string.Join(",", filtro.FormasPagamento) : null,
                        0,
                        (float)filtro.ValorRecebidoInicio,
                        (float)filtro.ValorRecebidoFim,
                        filtro.BuscarContasRenegociadas,
                        !filtro.BuscarContasAReceber,
                        (uint)(filtro.IdComissionado ?? 0),
                        (uint)(filtro.IdRota ?? 0),
                        filtro.Observacao,
                        0,
                        filtro.TiposContabeis != null && filtro.TiposContabeis.Any() ? string.Join(",", filtro.TiposContabeis) : null,
                        (uint)(filtro.NumeroArquivoRemessa ?? 0),
                        filtro.BuscarContasDeObra,
                        filtro.BuscaArquivoRemessa.GetValueOrDefault(1),
                        filtro.IdVendedorAssociadoCliente ?? 0,
                        filtro.IdVendedorObra ?? 0,
                        filtro.IdComissao ?? 0,
                        filtro.IdSinal ?? 0,
                        filtro.IdCte ?? 0,
                        filtro.BuscarContasProtestadas,
                        filtro.BuscarContasVinculadas,
                        filtro.BuscaNotaFiscal != null && filtro.BuscaNotaFiscal.Any() ? string.Join(",", filtro.BuscaNotaFiscal) : null,
                        filtro.NumeroAutorizacaoCartao));
            }
        }

        /// <summary>
        /// Recupera os tipos contábeis de conta a receber/recebida.
        /// </summary>
        /// <returns>Uma lista JSON com os tipos contábeis.</returns>
        [HttpGet]
        [Route("tiposContabeis")]
        [SwaggerResponse(200, "Tipos contábeis encontrados.", Type = typeof(IEnumerable<IdNomeDto>))]
        [SwaggerResponse(204, "Tipos contábeis não encontrados.")]
        public IHttpActionResult ObterTiposCotabeis()
        {
            using (var sessao = new GDATransaction())
            {
                var tiposContabeis = ContasReceberDAO.Instance.ObtemTiposContas()
                    .Select(p => new IdNomeDto
                    {
                        Id = (int?)p.Id,
                        Nome = p.Descr,
                    });

                return this.Lista(tiposContabeis);
            }
        }

        /// <summary>
        /// Recupera os tipos de busca de NF-e para tela de conta recebida.
        /// </summary>
        /// <returns>Uma lista JSON com os tipos de busca de NF-e.</returns>
        [HttpGet]
        [Route("tiposBuscaNfe")]
        [SwaggerResponse(200, "Tipos de busca de NF-e encontrados.", Type = typeof(IEnumerable<IdNomeDto>))]
        [SwaggerResponse(204, "Tipos de busca de NF-e não encontrados.")]
        public IHttpActionResult ObterTiposBucaNfe()
        {
            using (var sessao = new GDATransaction())
            {
                var tiposBuscaNfe = new List<IdNomeDto>()
                {
                    new IdNomeDto()
                    {
                        Id = 1,
                        Nome = "Contas COM NF-e geradas",
                    },
                    new IdNomeDto()
                    {
                        Id = 2,
                        Nome = "Contas SEM NF-e geradas",
                    },
                    new IdNomeDto()
                    {
                        Id = 3,
                        Nome = "Demais contas",
                    },
                };

                return this.Lista(tiposBuscaNfe);
            }
        }
    }
}
