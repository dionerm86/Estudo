// <copyright file="GetCaixaGeralController.cs" company="Sync Softwares">
// Copyright (c) Sync Softwares. Todos os direitos reservados.
// </copyright>

using Colosoft;
using GDA;
using Glass.API.Backend.Helper.Respostas;
using Glass.API.Backend.Models.Caixas.Geral.Lista;
using Glass.API.Backend.Models.Genericas;
using Glass.Data.DAL;
using Glass.Data.Helper;
using Swashbuckle.Swagger.Annotations;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Http;

namespace Glass.API.Backend.Controllers.Caixas.Geral.V1
{
    /// <summary>
    /// Controller de caixa geral.
    /// </summary>
    public partial class CaixaGeralController : BaseController
    {
        /// <summary>
        /// Recupera as configurações usadas pela tela de listagem de movimentações do caixa geral.
        /// </summary>
        /// <returns>Um objeto JSON com as configurações da tela.</returns>
        [HttpGet]
        [Route("configuracoes")]
        [SwaggerResponse(200, "Configurações recuperadas.", Type = typeof(Models.Caixas.Geral.Configuracoes.ListaDto))]
        public IHttpActionResult ObterConfiguracoesListaCaixaDiario()
        {
            using (var sessao = new GDATransaction())
            {
                var configuracoes = new Models.Caixas.Geral.Configuracoes.ListaDto();
                return this.Item(configuracoes);
            }
        }

        /// <summary>
        /// Recupera a lista de movimentações do caixa geral.
        /// </summary>
        /// <param name="filtro">Os filtros para a busca das movimentações.</param>
        /// <returns>Uma lista JSON com os dados das movimentações.</returns>
        [HttpGet]
        [Route("")]
        [SwaggerResponse(200, "Movimentações encontradas sem paginação (apenas uma página de retorno) ou última página retornada.", Type = typeof(IEnumerable<ListaDto>))]
        [SwaggerResponse(204, "Movimentações não encontradas para o filtro informado.")]
        [SwaggerResponse(206, "Movimentações paginadas (qualquer página, exceto a última).", Type = typeof(IEnumerable<ListaDto>))]
        [SwaggerResponse(400, "Filtro inválido informado (campo com valor ou formato inválido).", Type = typeof(MensagemDto))]
        public IHttpActionResult ObterMovimentacoesCaixaGeral([FromUri] FiltroDto filtro)
        {
            using (var sessao = new GDATransaction())
            {
                filtro = filtro ?? new FiltroDto();

                var movimentacoes = CaixaGeralDAO.Instance.GetMovimentacoes(
                    sessao,
                    (uint)(filtro.Id ?? 0),
                    (uint)(filtro.IdFuncionario ?? 0),
                    filtro.PeriodoCadastroInicio?.ToShortDateString(),
                    filtro.PeriodoCadastroFim?.ToShortDateString(),
                    filtro.Valor.ToString(),
                    filtro.Valor.ToString(),
                    filtro.ApenasDinheiro,
                    filtro.ApenasCheque,
                    (int)(filtro.Tipo ?? 0),
                    0,
                    filtro.ApenasEntradaExcetoEstorno,
                    (uint)(filtro.IdLoja ?? 0),
                    UserInfo.GetUserInfo);

                return this.ListaPaginada(
                    movimentacoes.Select(o => new ListaDto(o)),
                    filtro,
                    () => movimentacoes.Length);
            }
        }

        /// <summary>
        /// Recupera os totais por forma de pagamento.
        /// </summary>
        /// <param name="filtro">Os filtros para a busca dos totalizadores.</param>
        /// <returns>Um item com os totalizadores.</returns>
        [HttpGet]
        [Route("totaisPorFormaPagamento")]
        [SwaggerResponse(200, "Totais calculados.", Type = typeof(TotalizadoresDto))]
        [SwaggerResponse(204, "Totais não calculados.")]
        public IHttpActionResult ObterTotaisPorFormaPagamento([FromUri] FiltroDto filtro)
        {
            using (var sessao = new GDATransaction())
            {
                filtro = filtro ?? new FiltroDto();

                var totais = CaixaGeralDAO.Instance.ObterTotalizadores(
                    sessao,
                    (uint)(filtro.IdFuncionario ?? 0),
                    filtro.PeriodoCadastroInicio?.ToShortDateString(),
                    filtro.PeriodoCadastroFim?.ToShortDateString(),
                    filtro.ApenasDinheiro,
                    filtro.ApenasEntradaExcetoEstorno,
                    (uint)(filtro.IdLoja ?? 0),
                    UserInfo.GetUserInfo);

                var totalizador = new TotalizadoresDto()
                {
                    TotaisAcumulados = new TotaisAcumuladosDto()
                    {
                        Cheque = totais.TotalCheque,
                        Dinheiro = totais.TotalDinheiro,
                    },

                    TotaisCheques = new TotaisChequesDto()
                    {
                        Devolvido = totais.TotalChequeDevolvido,
                        Reapresentado = totais.TotalChequeReapresentado,
                        Terceiro = totais.TotalChequeTerc,
                    },

                    Entrada = new TotaisFormaPagamentoDto()
                    {
                        Cartao = totais.TotalEntradaCartao,
                        Cheque = totais.TotalEntradaCheque,
                        Construcard = totais.TotalEntradaConstrucard,
                        Dinheiro = totais.TotalEntradaDinheiro,
                        Permuta = totais.TotalEntradaPermuta,
                    },

                    Saida = new TotaisFormaPagamentoDto()
                    {
                        Cartao = totais.TotalSaidaCartao,
                        Cheque = totais.TotalSaidaCheque,
                        Construcard = totais.TotalSaidaConstrucard,
                        Dinheiro = totais.TotalSaidaDinheiro,
                        Permuta = totais.TotalSaidaPermuta,
                    },

                    Saldo = new TotaisFormaPagamentoDto()
                    {
                        Cartao = totais.SaldoCartao,
                        Cheque = totais.SaldoCheque,
                        Construcard = totais.SaldoConstrucard,
                        Dinheiro = totais.SaldoDinheiro,
                        Permuta = totais.SaldoPermuta,
                    },

                    TotaisCredito = new TotaisCreditoDto()
                    {
                        Gerado = totais.TotalCreditoGerado,
                        Recebido = totais.TotalCreditoRecebido,
                    },

                    TotaisParcelas = new TotaisParcelasDto()
                    {
                        Gerada = totais.ContasReceberGeradas,
                        RecebidaContabil = totais.TotalContasRecebidasContabeis,
                        RecebidaNaoContabil = totais.TotalContasRecebidasNaoContabeis,
                    },
                };

                return this.Item(totalizador);
            }
        }

        /// <summary>
        /// Recupera os tipos de movimentação para a tela de caixa geral.
        /// </summary>
        /// <returns>Uma lista JSON com os tipos de movimentação.</returns>
        [HttpGet]
        [Route("tiposMovimentacao")]
        [SwaggerResponse(200, "Tipos de movimentação encontrados.", Type = typeof(IEnumerable<IdNomeDto>))]
        [SwaggerResponse(204, "Tipos de movimentação não encontrados.")]
        public IHttpActionResult ObterTiposMovimentacao()
        {
            using (var sessao = new GDATransaction())
            {
                var tiposMovimentacao = new List<IdNomeDto>();

                foreach (Data.Model.CaixaGeral.TipoEnum tipo in Enum.GetValues(typeof(Data.Model.CaixaGeral.TipoEnum)))
                {
                    tiposMovimentacao.Add(new IdNomeDto()
                    {
                        Id = (int)tipo,
                        Nome = tipo.Translate().ToString(),
                    });
                }

                return this.Lista(tiposMovimentacao);
            }
        }
    }
}
