// <copyright file="GetCaixaDiarioController.cs" company="Sync Softwares">
// Copyright (c) Sync Softwares. Todos os direitos reservados.
// </copyright>

using GDA;
using Glass.API.Backend.Helper.Respostas;
using Glass.API.Backend.Models.Caixas.Diario.Lista;
using Glass.Data.DAL;
using Swashbuckle.Swagger.Annotations;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Http;

namespace Glass.API.Backend.Controllers.Caixas.Diario.V1
{
    /// <summary>
    /// Controller de caixa diário.
    /// </summary>
    public partial class CaixaDiarioController : BaseController
    {
        /// <summary>
        /// Recupera as configurações usadas pela tela de listagem de movimentações do caixa diário.
        /// </summary>
        /// <returns>Um objeto JSON com as configurações da tela.</returns>
        [HttpGet]
        [Route("configuracoes")]
        [SwaggerResponse(200, "Configurações recuperadas.", Type = typeof(Models.Caixas.Diario.Configuracoes.ListaDto))]
        public IHttpActionResult ObterConfiguracoesListaCaixaDiario()
        {
            using (var sessao = new GDATransaction())
            {
                var configuracoes = new Models.Caixas.Diario.Configuracoes.ListaDto();
                return this.Item(configuracoes);
            }
        }

        /// <summary>
        /// Recupera a lista de movimentações do caixa diário.
        /// </summary>
        /// <param name="filtro">Os filtros para a busca das movimentações.</param>
        /// <returns>Uma lista JSON com os dados das movimentações.</returns>
        [HttpGet]
        [Route("")]
        [SwaggerResponse(200, "Movimentações encontradas sem paginação (apenas uma página de retorno) ou última página retornada.", Type = typeof(IEnumerable<ListaDto>))]
        [SwaggerResponse(204, "Movimentações não encontradas para o filtro informado.")]
        [SwaggerResponse(206, "Movimentações paginadas (qualquer página, exceto a última).", Type = typeof(IEnumerable<ListaDto>))]
        [SwaggerResponse(400, "Filtro inválido informado (campo com valor ou formato inválido).", Type = typeof(MensagemDto))]
        public IHttpActionResult ObterMovimentacoesCaixaDiario([FromUri] FiltroDto filtro)
        {
            using (var sessao = new GDATransaction())
            {
                filtro = filtro ?? new FiltroDto();

                var movimentacoes = CaixaDiarioDAO.Instance.GetMovimentacoes(
                    sessao,
                    (uint)filtro.IdLoja,
                    (uint)(filtro.IdFuncionario ?? 0),
                    filtro.Data);

                return this.ListaPaginada(
                    movimentacoes.Select(o => new ListaDto(o)),
                    filtro,
                    () => movimentacoes.Length);
            }
        }

        /// <summary>
        /// Recupera dados para fechamento do caixa.
        /// </summary>
        /// <param name="idLoja">O identificado da loja que terão os dados de fechamento recuperados.</param>
        /// <returns>Uma lista JSON com os dados para fechamento do caixa.</returns>
        [HttpGet]
        [Route("{idLoja}/obterDadosFechamento")]
        [SwaggerResponse(200, "Dados de fechamento encontrados.", Type = typeof(FechamentoDto))]
        [SwaggerResponse(204, "Dados de fechamento encontrados.")]
        public IHttpActionResult ObterDadosFechamento(int idLoja)
        {
            using (var sessao = new GDATransaction())
            {
                var dataCaixaAberto = CaixaDiarioDAO.Instance.GetDataCaixaAberto(sessao, (uint)idLoja);

                var fechamento = new FechamentoDto();
                fechamento.DiaAtual = new DiaAtualDto()
                {
                    CaixaFechado = CaixaDiarioDAO.Instance.CaixaFechado(sessao, (uint)idLoja),
                    Saldo = CaixaDiarioDAO.Instance.GetSaldoByLoja(sessao, (uint)idLoja),
                    SaldoDinheiro = CaixaDiarioDAO.Instance.GetSaldoByFormaPagto(sessao, Data.Model.Pagto.FormaPagto.Dinheiro, 0, (uint)idLoja, 0, DateTime.Now, 1),
                    ExistemMovimentacoes = CaixaDiarioDAO.Instance.ExisteMovimentacao(sessao, (uint)idLoja, DateTime.Now),
                };

                fechamento.DiaAnterior = new DiaAnteriorDto()
                {
                    CaixaFechado = CaixaDiarioDAO.Instance.CaixaFechadoDiaAnterior(sessao, (uint)idLoja),
                    Saldo = CaixaDiarioDAO.Instance.GetSaldoDiaAnterior(sessao, (uint)idLoja),
                    SaldoDinheiro = CaixaDiarioDAO.Instance.GetSaldoByFormaPagto(sessao, Data.Model.Pagto.FormaPagto.Dinheiro, 0, (uint)idLoja, 0, dataCaixaAberto, 1),
                    DataCaixaAberto = dataCaixaAberto,
                };

                if (fechamento.DiaAtual.Saldo == 0 && !fechamento.DiaAtual.ExistemMovimentacoes)
                {
                    fechamento.DiaAtual.Saldo = fechamento.DiaAnterior.Saldo;
                    fechamento.DiaAtual.SaldoDinheiro += fechamento.DiaAnterior.Saldo;
                }

                return this.Item(fechamento);
            }
        }
    }
}
