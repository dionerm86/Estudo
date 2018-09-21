﻿// <copyright file="GetNotasFiscaisController.cs" company="Sync Softwares">
// Copyright (c) Sync Softwares. Todos os direitos reservados.
// </copyright>

using GDA;
using Glass.API.Backend.Helper;
using Glass.API.Backend.Helper.Respostas;
using Glass.API.Backend.Models.Genericas;
using Glass.API.Backend.Models.NotasFiscais.Boleto;
using Glass.API.Backend.Models.NotasFiscais.TiposParticipantes;
using Glass.Configuracoes;
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
        /// Valida se um boleto pode ser impresso.
        /// </summary>
        /// <param name="id">O identificador da nota fiscal.</param>
        /// <returns>Um status HTTP indicando se o boleto pode ser impresso.</returns>
        [HttpGet]
        [Route("{id}/validarBoleto")]
        [SwaggerResponse(200, "Boleto pode ser impresso.", Type = typeof(MensagemDto))]
        [SwaggerResponse(400, "Erro de valor ou formato do campo id ou de validação da impressão do boleto.", Type = typeof(MensagemDto))]
        [SwaggerResponse(404, "Nota fiscal não encontrada.", Type = typeof(MensagemDto))]
        public IHttpActionResult ValidarBoleto(int id)
        {
            using (var sessao = new GDATransaction())
            {
                var validacao = this.ValidarExistenciaIdNotaFiscal(sessao, id);

                if (validacao != null)
                {
                    return validacao;
                }

                if (!FinanceiroConfig.UsarNumNfBoletoSemSeparacao)
                {
                    return this.Ok();
                }

                var idsPedidosNf = PedidosNotaFiscalDAO.Instance
                    .GetByNf(sessao, (uint)id)
                    .Where(f => f.IdPedido.GetValueOrDefault(0) > 0)
                    .Select(f => f.IdPedido.Value);

                foreach (var idPedNf in idsPedidosNf)
                {
                    var idLiberarPedido = LiberarPedidoDAO.Instance.ObterIdLiberarPedidoParaImpressaoBoletoNFe(sessao, (int)idPedNf, id).GetValueOrDefault();

                    if (idLiberarPedido == 0)
                    {
                        return this.ErroValidacao($"Não é possível gerar o boleto desta NF-e, pois o pedido: {idPedNf} não possui uma liberação vinculada.");
                    }

                    if (PedidosNotaFiscalDAO.Instance.GetByLiberacaoPedido(sessao, (uint)idLiberarPedido).Length == 0)
                    {
                        return this.ErroValidacao($"Não é possível gerar o boleto desta NF-e, pois a liberação: {idLiberarPedido} não esta vinculada a mesma.");
                    }

                    foreach (var idPedLib in ProdutosLiberarPedidoDAO.Instance.GetIdsPedidoByLiberacao(sessao, (uint)idLiberarPedido))
                    {
                        if (!idsPedidosNf.Contains(idPedLib))
                        {
                            var notasPedido = PedidosNotaFiscalDAO.Instance
                                .GetByPedido(sessao, idPedLib)
                                .Where(f => f.IdPedido.GetValueOrDefault(0) > 0 && f.IdNf != id)
                                .Select(f => f.IdNf).ToList();

                            if (notasPedido?.Count() > 0)
                            {
                                var notas = NotaFiscalDAO.Instance.ObtemNumerosNFePeloIdNf(sessao, string.Join(",", notasPedido));

                                return this.ErroValidacao($"Não é possível gerar o boleto desta NF-e, pois o pedido: {idPedLib} da Liberação: {idLiberarPedido} esta vinculado a outras Notas Fiscais. NF-e's: {notas}.");
                            }
                        }
                    }
                }

                return this.Ok();
            }
        }

        /// <summary>
        /// Retorna o id da nota fiscal com base no id conta receber.
        /// </summary>
        /// <param name="idContaReceber">O identificador da conta a receber.</param>
        /// <returns>O identificador da nota fiscal.</returns>
        [HttpGet]
        [Route("obterIdNotaFiscalPeloIdContaReceber")]
        [SwaggerResponse(200, "Identificador da nota fiscal encontrado.", Type = typeof(NotaFiscalDto))]
        [SwaggerResponse(400, "Erro de valor ou formato do campo id ou de validação da obtenção do identificador da nota fiscal.", Type = typeof(MensagemDto))]
        public IHttpActionResult ObterIdNotaFiscalPeloIdContaReceber(int idContaReceber)
        {
            using (var sessao = new GDATransaction())
            {
                if (idContaReceber == 0)
                {
                    return this.ErroValidacao("Conta a receber não informada.");
                }

                var idsNotaFiscal = NotaFiscalDAO.Instance.ObtemIdNfByContaR((uint)idContaReceber, true);

                var id = idsNotaFiscal != null && idsNotaFiscal.Count > 0 ? (int)idsNotaFiscal[0] : (int?)null;

                return this.Item(
                    new NotaFiscalDto()
                    {
                        IdNotaFiscal = id,
                    });
            }
        }

        /// <summary>
        /// Retorna uma mensagem indicando se o boleto está impresso.
        /// </summary>
        /// <param name="id">O identificador da nota fiscal.</param>
        /// <param name="idContaReceber">O identificador da conta a receber.</param>
        /// <param name="idLiberacao">O identificador da liberação.</param>
        /// <param name="idCte">O identificador do CT-e.</param>
        /// <returns>Uma mensagem indicando se o boleto está impreso.</returns>
        [HttpGet]
        [Route("{id}/obterMensagemBoletoImpresso")]
        [SwaggerResponse(200, "Mensagem indicando se o boleto está impresso.", Type = typeof(BoletoImpressoDto))]
        [SwaggerResponse(400, "Erro de valor ou formato do campo id ou de validação da obtenção do identificador da nota fiscal.", Type = typeof(MensagemDto))]
        [SwaggerResponse(404, "Nota fiscal não encontrada.", Type = typeof(MensagemDto))]
        public IHttpActionResult ObterMensagemBoletoImpresso(int id, int? idContaReceber, int? idLiberacao, int? idCte)
        {
            using (var sessao = new GDATransaction())
            {
                var validacao = this.ValidarExistenciaIdNotaFiscal(sessao, id);

                if (validacao != null)
                {
                    return validacao;
                }

                var mensagem = WebGlass.Business.Boleto.Fluxo.Impresso.Instance.MensagemBoletoImpresso(
                    idContaReceber,
                    id,
                    idLiberacao,
                    idCte);

                return this.Item(
                    new BoletoImpressoDto()
                    {
                        Mensagem = mensagem,
                    });
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
