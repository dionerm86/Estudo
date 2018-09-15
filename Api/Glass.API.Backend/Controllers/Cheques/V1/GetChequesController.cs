// <copyright file="GetChequesController.cs" company="Sync Softwares">
// Copyright (c) Sync Softwares. Todos os direitos reservados.
// </copyright>

using GDA;
using Glass.API.Backend.Helper.Respostas;
using Glass.API.Backend.Models.Genericas;
using Glass.Data.DAL;
using Glass.Data.Helper;
using Swashbuckle.Swagger.Annotations;
using System.Collections.Generic;
using System.Linq;
using System.Web.Http;

namespace Glass.API.Backend.Controllers.Cheques.V1
{
    /// <summary>
    /// Controller de cheques.
    /// </summary>
    public partial class ChequesController : BaseController
    {
        /// <summary>
        /// Recupera as configurações usadas pela tela de listagem de cheque.
        /// </summary>
        /// <returns>Um objeto JSON com as configurações da tela.</returns>
        [HttpGet]
        [Route("configuracoes")]
        [SwaggerResponse(200, "Configurações recuperadas.", Type = typeof(Models.Cheques.Configuracoes.ListaDto))]
        public IHttpActionResult ObterConfiguracoesListaEstoque()
        {
            using (var sessao = new GDATransaction())
            {
                var configuracoes = new Models.Cheques.Configuracoes.ListaDto();
                return this.Item(configuracoes);
            }
        }

        /// <summary>
        /// Recupera a lista de cheques.
        /// </summary>
        /// <param name="filtro">Os filtros para a busca dos cheques.</param>
        /// <returns>Uma lista JSON com os dados dos cheques.</returns>
        [HttpGet]
        [Route("")]
        [SwaggerResponse(200, "Cheques sem paginação (apenas uma página de retorno) ou última página retornada.", Type = typeof(IEnumerable<Models.Cheques.Lista.ListaDto>))]
        [SwaggerResponse(204, "Cheques não encontrados para o filtro informado.")]
        [SwaggerResponse(206, "Cheques paginados (qualquer página, exceto a última).", Type = typeof(IEnumerable<Models.Cheques.Lista.ListaDto>))]
        [SwaggerResponse(400, "Filtro inválido informado (campo com valor ou formato inválido).", Type = typeof(MensagemDto))]
        public IHttpActionResult ObterListaCheques([FromUri] Models.Cheques.Lista.FiltroDto filtro)
        {
            using (var sessao = new GDATransaction())
            {
                filtro = filtro ?? new Models.Cheques.Lista.FiltroDto();

                var tipo = filtro.Tipo ?? 2;

                var cheques = ChequesDAO.Instance.GetByFilter(
                    (uint)(filtro.IdLoja ?? 0),
                    (uint)(filtro.IdPedido ?? 0),
                    (uint)(filtro.IdLiberacao ?? 0),
                    (uint)(filtro.IdAcerto ?? 0),
                    (uint)(filtro.NumeroNfe ?? 0),
                    tipo,
                    filtro.NumeroCheque ?? 0,
                    filtro.Situacao != null && filtro.Situacao.Any() ? string.Join(",", filtro.Situacao) : null,
                    filtro.Reapresentado,
                    filtro.Advogado ?? 0,
                    filtro.Titular,
                    filtro.Agencia,
                    filtro.Conta,
                    filtro.PeriodoVencimentoInicio != null ? filtro.PeriodoCadastroInicio.Value.ToShortDateString() : null,
                    filtro.PeriodoVencimentoFim != null ? filtro.PeriodoCadastroFim.Value.ToShortDateString() : null,
                    filtro.PeriodoCadastroInicio != null ? filtro.PeriodoCadastroInicio.Value.ToShortDateString() : null,
                    filtro.PeriodoCadastroFim != null ? filtro.PeriodoCadastroFim.Value.ToShortDateString() : null,
                    filtro.CpfCnpj,
                    (uint)(filtro.IdCliente ?? 0),
                    filtro.NomeCliente,
                    (uint)(filtro.IdFornecedor ?? 0),
                    filtro.NomeFornecedor,
                    (float)filtro.ValorChequeInicio,
                    (float)filtro.ValorChequeFim,
                    filtro.UsuarioCadastro,
                    filtro.IdsRota != null && filtro.IdsRota.Any() ? string.Join(",", filtro.IdsRota) : null,
                    string.Empty,
                    filtro.ExibirApenasCaixaDiario,
                    filtro.Observacao,
                    filtro.ObterTraducaoOrdenacao(),
                    filtro.ObterPrimeiroRegistroRetornar(),
                    filtro.NumeroRegistros);

                return this.ListaPaginada(
                    cheques.Select(o => new Models.Cheques.Lista.ListaDto(o)),
                    filtro,
                    () => ChequesDAO.Instance.GetCountFilter(
                        (uint)(filtro.IdLoja ?? 0),
                        (uint)(filtro.IdPedido ?? 0),
                        (uint)(filtro.IdLiberacao ?? 0),
                        (uint)(filtro.IdAcerto ?? 0),
                        (uint)(filtro.NumeroNfe ?? 0),
                        tipo,
                        filtro.NumeroCheque ?? 0,
                        filtro.Situacao != null && filtro.Situacao.Any() ? string.Join(",", filtro.Situacao) : null,
                        filtro.Reapresentado,
                        filtro.Advogado ?? 0,
                        filtro.Titular,
                        filtro.Agencia,
                        filtro.Conta,
                        filtro.PeriodoVencimentoInicio != null ? filtro.PeriodoCadastroInicio.Value.ToShortDateString() : null,
                        filtro.PeriodoVencimentoFim != null ? filtro.PeriodoCadastroFim.Value.ToShortDateString() : null,
                        filtro.PeriodoCadastroInicio != null ? filtro.PeriodoCadastroInicio.Value.ToShortDateString() : null,
                        filtro.PeriodoCadastroFim != null ? filtro.PeriodoCadastroFim.Value.ToShortDateString() : null,
                        filtro.CpfCnpj,
                        (uint)(filtro.IdCliente ?? 0),
                        filtro.NomeCliente,
                        (uint)(filtro.IdFornecedor ?? 0),
                        filtro.NomeFornecedor,
                        (float)filtro.ValorChequeInicio,
                        (float)filtro.ValorChequeFim,
                        filtro.UsuarioCadastro,
                        filtro.IdsRota != null && filtro.IdsRota.Any() ? string.Join(",", filtro.IdsRota) : null,
                        string.Empty,
                        filtro.ExibirApenasCaixaDiario,
                        filtro.Observacao));
            }
        }

        /// <summary>
        /// Recupera a lista de situações de cheque.
        /// </summary>
        /// <returns>Uma lista JSON com os dados básicos de situações de cheque.</returns>
        [HttpGet]
        [Route("situacoes")]
        [SwaggerResponse(200, "Situações de cheque encontradas.", Type = typeof(IEnumerable<IdNomeDto>))]
        [SwaggerResponse(204, "Situações de cheque não encontradas.")]
        public IHttpActionResult ObterSituacoes()
        {
            using (var sessao = new GDATransaction())
            {
                var situacoes = DataSources.Instance.GetSituacaoCheque()
                    .Select(s => new IdNomeDto
                    {
                        Id = (int)(s.Id ?? 0),
                        Nome = s.Descr,
                    });

                return this.Lista(situacoes);
            }
        }
    }
}
