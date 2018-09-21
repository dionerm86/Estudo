// <copyright file="GetNotasFiscaisController.cs" company="Sync Softwares">
// Copyright (c) Sync Softwares. Todos os direitos reservados.
// </copyright>

using GDA;
using Glass.API.Backend.Helper;
using Glass.API.Backend.Helper.Respostas;
using Glass.API.Backend.Models.Genericas;
using Glass.API.Backend.Models.NotasFiscais.TiposParticipantes;
using Glass.Data.DAL;
using Swashbuckle.Swagger.Annotations;
using System.Collections.Generic;
using System.Linq;
using System.Web.Http;

namespace Glass.API.Backend.Controllers.NotasFiscais.V1
{
    /// <summary>
    /// Controller de liberações.
    /// </summary>
    public partial class NotasFiscaisController : BaseController
    {
        /// <summary>
        /// Recupera as configurações usadas pela tela de listagem de notas fiscais.
        /// </summary>
        /// <returns>Um objeto JSON com as configurações da tela.</returns>
        [HttpGet]
        [Route("configuracoes")]
        [SwaggerResponse(200, "Configurações recuperadas.", Type = typeof(Models.NotasFiscais.Configuracoes.ListaDto))]
        public IHttpActionResult ObterConfiguracoesListaNotasFiscais()
        {
            using (var sessao = new GDATransaction())
            {
                var configuracoes = new Models.NotasFiscais.Configuracoes.ListaDto();
                return this.Item(configuracoes);
            }
        }

        /// <summary>
        /// Recupera a lista de notas fiscais.
        /// </summary>
        /// <param name="filtro">Os filtros para a busca das notas fiscais.</param>
        /// <returns>Uma lista JSON com os dados das notas fiscais.</returns>
        [HttpGet]
        [Route("")]
        [SwaggerResponse(200, "Notas fiscais sem paginação (apenas uma página de retorno) ou última página retornada.", Type = typeof(IEnumerable<Models.NotasFiscais.Lista.ListaDto>))]
        [SwaggerResponse(204, "Notas fiscais não encontradas para o filtro informado.")]
        [SwaggerResponse(206, "Notas fiscais paginados (qualquer página, exceto a última).", Type = typeof(IEnumerable<Models.NotasFiscais.Lista.ListaDto>))]
        [SwaggerResponse(400, "Filtro inválido informado (campo com valor ou formato inválido).", Type = typeof(MensagemDto))]
        public IHttpActionResult ObterListaNotasFiscais([FromUri] Models.NotasFiscais.Lista.FiltroDto filtro)
        {
            using (var sessao = new GDATransaction())
            {
                filtro = filtro ?? new Models.NotasFiscais.Lista.FiltroDto();

                var notasFiscais = NotaFiscalDAO.Instance.GetListaPadrao(
                    (uint)(filtro.NumeroNfe ?? 0),
                    (uint)(filtro.IdPedido ?? 0),
                    filtro.Modelo,
                    (uint)(filtro.IdLoja ?? 0),
                    (uint)(filtro.IdCliente ?? 0),
                    filtro.NomeCliente,
                    (int)(filtro.TipoFiscal ?? 0),
                    (uint)(filtro.IdFornecedor ?? 0),
                    filtro.NomeFornecedor,
                    filtro.CodigoRota,
                    0,
                    filtro.Situacao != null ? ((int)filtro.Situacao.Value).ToString() : null,
                    filtro.PeriodoEmissaoInicio?.ToShortDateString(),
                    filtro.PeriodoEmissaoFim?.ToShortDateString(),
                    filtro.IdsCfop != null && filtro.IdsCfop.Any() ? string.Join(",", filtro.IdsCfop) : null,
                    filtro.TiposCfop != null && filtro.TiposCfop.Any() ? string.Join(",", filtro.TiposCfop) : null,
                    filtro.PeriodoEntradaSaidaInicio?.ToShortDateString(),
                    filtro.PeriodoEntradaSaidaFim?.ToShortDateString(),
                    (uint)(filtro.TipoVenda ?? 0),
                    filtro.IdsFormaPagamento != null && filtro.IdsFormaPagamento.Any() ? string.Join(",", filtro.IdsFormaPagamento) : null,
                    (int)(filtro.TipoDocumento ?? 0),
                    (int)(filtro.Finalidade ?? 0),
                    (int)(filtro.TipoEmissao ?? 0),
                    filtro.InformacaoComplementar,
                    filtro.CodigoInternoProduto,
                    filtro.DescricaoProduto,
                    filtro.ValorNotaFiscalInicio != null ? filtro.ValorNotaFiscalInicio.ToString().Replace(".", ",") : null,
                    filtro.ValorNotaFiscalFim != null ? filtro.ValorNotaFiscalFim.ToString().Replace(".", ",") : null,
                    null,
                    filtro.Lote,
                    0,
                    filtro.ObterTraducaoOrdenacao(),
                    filtro.ObterPrimeiroRegistroRetornar(),
                    filtro.NumeroRegistros);

                return this.ListaPaginada(
                    notasFiscais.Select(n => new Models.NotasFiscais.Lista.ListaDto(n)),
                    filtro,
                    () => NotaFiscalDAO.Instance.GetCountListaPadrao(
                        (uint)(filtro.NumeroNfe ?? 0),
                        (uint)(filtro.IdPedido ?? 0),
                        filtro.Modelo,
                        (uint)(filtro.IdLoja ?? 0),
                        (uint)(filtro.IdCliente ?? 0),
                        filtro.NomeCliente,
                        (int)(filtro.TipoFiscal ?? 0),
                        (uint)(filtro.IdFornecedor ?? 0),
                        filtro.NomeFornecedor,
                        filtro.CodigoRota,
                        0,
                        filtro.Situacao != null ? ((int)filtro.Situacao.Value).ToString() : null,
                        filtro.PeriodoEmissaoInicio?.ToShortDateString(),
                        filtro.PeriodoEmissaoFim?.ToShortDateString(),
                        filtro.IdsCfop != null && filtro.IdsCfop.Any() ? string.Join(",", filtro.IdsCfop) : null,
                        filtro.TiposCfop != null && filtro.TiposCfop.Any() ? string.Join(",", filtro.TiposCfop) : null,
                        filtro.PeriodoEntradaSaidaInicio?.ToShortDateString(),
                        filtro.PeriodoEntradaSaidaFim?.ToShortDateString(),
                        (uint)(filtro.TipoVenda ?? 0),
                        filtro.IdsFormaPagamento != null && filtro.IdsFormaPagamento.Any() ? string.Join(",", filtro.IdsFormaPagamento) : null,
                        (int)(filtro.TipoDocumento ?? 0),
                        (int)(filtro.Finalidade ?? 0),
                        (int)(filtro.TipoEmissao ?? 0),
                        filtro.InformacaoComplementar,
                        filtro.CodigoInternoProduto,
                        filtro.DescricaoProduto,
                        filtro.ValorNotaFiscalInicio.ToString().Replace(".", ","),
                        filtro.ValorNotaFiscalFim.ToString().Replace(".", ","),
                        null,
                        filtro.Lote,
                        filtro.OrdenacaoFiltro));
            }
        }

        /// <summary>
        /// Recupera as situações de nota fiscal para o controle de pesquisa.
        /// </summary>
        /// <returns>Uma lista JSON com os dados das situações encontradas.</returns>
        [HttpGet]
        [Route("situacoes")]
        [SwaggerResponse(200, "Situações encontradas.", Type = typeof(IEnumerable<IdNomeDto>))]
        [SwaggerResponse(204, "Situações não encontradas.")]
        public IHttpActionResult ObterSituacoes()
        {
            using (var sessao = new GDATransaction())
            {
                var situacoes = Data.Helper.DataSources.Instance.GetSituacaoNotaFiscal()
                    .Select(c => new IdNomeDto()
                    {
                        Id = (int?)c.Id,
                        Nome = c.Descr,
                    });

                return this.Lista(situacoes);
            }
        }

        /// <summary>
        /// Recupera os tipos de participantes fiscais para o controle de pesquisa.
        /// </summary>
        /// <param name="incluirAdministradoraCartao">Indica se as administradoras de cartão serão consideradas participantes no retorno.</param>
        /// <returns>Uma lista JSON com os dados dos tipos de participantes encontrados.</returns>
        [HttpGet]
        [Route("tiposParticipantes")]
        [SwaggerResponse(200, "Tipos de participantes encontrados.", Type = typeof(IEnumerable<TipoParticipanteDto>))]
        [SwaggerResponse(204, "Tipos de participantes não encontrados.")]
        public IHttpActionResult ObterTiposParticipantesFiscais(bool incluirAdministradoraCartao = false)
        {
            using (var sessao = new GDATransaction())
            {
                var tiposParticipantes = new ConversorEnum<Data.EFD.DataSourcesEFD.TipoPartEnum>()
                    .ObterTraducao()
                    .Where(tipoParticipante =>
                    {
                        return incluirAdministradoraCartao
                            || tipoParticipante.Id != (int)Data.EFD.DataSourcesEFD.TipoPartEnum.AdministradoraCartao;
                    })
                    .Select(tipoParticipante => new TipoParticipanteDto(tipoParticipante));

                return this.Lista(tiposParticipantes);
            }
        }
    }
}
