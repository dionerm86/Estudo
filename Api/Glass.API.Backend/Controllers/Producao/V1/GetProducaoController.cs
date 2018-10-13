// <copyright file="GetProducaoController.cs" company="Sync Softwares">
// Copyright (c) Sync Softwares. Todos os direitos reservados.
// </copyright>

using GDA;
using Glass.API.Backend.Helper;
using Glass.API.Backend.Helper.Respostas;
using Glass.API.Backend.Models.Genericas;
using Glass.API.Backend.Models.Producao.V1.Lista;
using Glass.Data.DAL;
using Swashbuckle.Swagger.Annotations;
using System.Collections.Generic;
using System.Linq;
using System.Web.Http;

namespace Glass.API.Backend.Controllers.Producao.V1
{
    /// <summary>
    /// Controller de produção.
    /// </summary>
    public partial class ProducaoController : BaseController
    {
        /// <summary>
        /// Recupera as configurações para a listagem de produção.
        /// </summary>
        /// <returns>Um objeto JSON </returns>
        [HttpGet]
        [Route("configuracoes/consulta")]
        [SwaggerResponse(200, "Configurações encontradas.", Type = typeof(Models.Producao.V1.Configuracoes.ListaDto))]
        public IHttpActionResult ObterConfiguracoesConsulta()
        {
            using (var sessao = new GDATransaction())
            {
                var configuracoes = new Models.Producao.V1.Configuracoes.ListaDto();
                return this.Item(configuracoes);
            }
        }

        /// <summary>
        /// Recupera a lista de peças para a tela de consulta de produção.
        /// </summary>
        /// <param name="filtro">Os dados informados para filtro na tela.</param>
        /// <returns>Uma lista JSON com as peças em produção.</returns>
        [HttpGet]
        [Route("")]
        [SwaggerResponse(200, "Peças em produção sem paginação (apenas uma página de retorno) ou última página retornada.", Type = typeof(IEnumerable<IdNomeDto>))]
        [SwaggerResponse(204, "Peças em produção não encontradas para o filtro informado.")]
        [SwaggerResponse(206, "Peças em produção paginadas (qualquer página, exceto a última).", Type = typeof(IEnumerable<IdNomeDto>))]
        [SwaggerResponse(400, "Filtro inválido informado (campo com valor ou formato inválido).", Type = typeof(MensagemDto))]
        public IHttpActionResult ObterPecasProducao([FromUri] FiltroDto filtro)
        {
            filtro = filtro ?? new FiltroDto();

            using (var sessao = new GDATransaction())
            {
                var pecas = ProdutoPedidoProducaoDAO.Instance.GetListConsulta(
                    filtro.IdCarregamento.GetValueOrDefault(),
                    filtro.IdLiberacaoPedido.ToString(),
                    (uint)filtro.IdPedido.GetValueOrDefault(),
                    filtro.IdPedidoImportado.ToString(),
                    (uint)filtro.IdImpressao.GetValueOrDefault(),
                    filtro.CodigoPedidoCliente,
                    filtro.IdsRotas.ObterComoString(),
                    (uint)filtro.IdCliente.GetValueOrDefault(),
                    filtro.NomeCliente,
                    filtro.NumeroEtiquetaPeca,
                    filtro.PeriodoSetorInicio.FormatarData(),
                    filtro.PeriodoSetorFim.FormatarData(),
                    filtro.PeriodoEntregaInicio.FormatarData(),
                    filtro.PeriodoEntregaFim.FormatarData(),
                    filtro.PeriodoFabricaInicio.FormatarData(),
                    filtro.PeriodoFabricaFim.FormatarData(),
                    filtro.PeriodoConferenciaPedidoInicio.FormatarData(),
                    filtro.PeriodoConferenciaPedidoFim.FormatarData(),
                    filtro.IdSetor.GetValueOrDefault(),
                    filtro.SituacoesProducao.ObterComoString(),
                    (int)filtro.SituacaoPedido.GetValueOrDefault(),
                    (int)filtro.TipoSituacaoProducao.GetValueOrDefault(),
                    filtro.IdsSubgrupos.ObterComoString(),
                    (uint)filtro.TipoEntregaPedido.GetValueOrDefault(),
                    filtro.TiposPecasExibir.ObterComoString(),
                    (uint)filtro.IdVendedorPedido.GetValueOrDefault(),
                    filtro.TiposPedidos.ObterComoString(),
                    (uint)filtro.IdCorVidro.GetValueOrDefault(),
                    (int)filtro.AlturaPeca.GetValueOrDefault(),
                    filtro.LarguraPeca.GetValueOrDefault(),
                    (float)filtro.EspessuraPeca.GetValueOrDefault(),
                    filtro.IdsProcessos.ObterComoString(),
                    filtro.IdsAplicacoes.ObterComoString(),
                    filtro.ApenasPecasAguardandoExpedicao,
                    filtro.ApenasPecasAguardandoEntradaEstoque,
                    filtro.IdsBeneficiamentos.ObterComoString(),
                    filtro.PlanoCorte,
                    filtro.NumeroEtiquetaChapa,
                    (uint)filtro.TipoFastDelivery.GetValueOrDefault(),
                    filtro.ApenasPecasParadasNaProducao,
                    filtro.ApenasPecasRepostas,
                    (uint)filtro.IdLoja.GetValueOrDefault(),
                    (int?)filtro.TipoProdutosComposicao,
                    0,
                    filtro.ObterPrimeiroRegistroRetornar(),
                    filtro.ObterTraducaoOrdenacao(),
                    filtro.ObterPrimeiroRegistroRetornar(),
                    filtro.NumeroRegistros);

                return this.ListaPaginada(
                    pecas.Select(p => new ListaDto(p)),
                    filtro,
                    () => ProdutoPedidoProducaoDAO.Instance.GetCountConsulta(
                        filtro.IdCarregamento.GetValueOrDefault(),
                        filtro.IdLiberacaoPedido.ToString(),
                        (uint)filtro.IdPedido.GetValueOrDefault(),
                        filtro.IdPedidoImportado.ToString(),
                        (uint)filtro.IdImpressao.GetValueOrDefault(),
                        filtro.CodigoPedidoCliente,
                        filtro.IdsRotas.ObterComoString(),
                        (uint)filtro.IdCliente.GetValueOrDefault(),
                        filtro.NomeCliente,
                        filtro.NumeroEtiquetaPeca,
                        filtro.PeriodoSetorInicio.FormatarData(),
                        filtro.PeriodoSetorFim.FormatarData(),
                        filtro.PeriodoEntregaInicio.FormatarData(),
                        filtro.PeriodoEntregaFim.FormatarData(),
                        filtro.PeriodoFabricaInicio.FormatarData(),
                        filtro.PeriodoFabricaFim.FormatarData(),
                        filtro.PeriodoConferenciaPedidoInicio.FormatarData(),
                        filtro.PeriodoConferenciaPedidoFim.FormatarData(),
                        filtro.IdSetor.GetValueOrDefault(),
                        filtro.SituacoesProducao.ObterComoString(),
                        (int)filtro.SituacaoPedido.GetValueOrDefault(),
                        (int)filtro.TipoSituacaoProducao.GetValueOrDefault(),
                        filtro.IdsSubgrupos.ObterComoString(),
                        (uint)filtro.TipoEntregaPedido.GetValueOrDefault(),
                        filtro.TiposPecasExibir.ObterComoString(),
                        (uint)filtro.IdVendedorPedido.GetValueOrDefault(),
                        filtro.TiposPedidos.ObterComoString(),
                        (uint)filtro.IdCorVidro.GetValueOrDefault(),
                        (int)filtro.AlturaPeca.GetValueOrDefault(),
                        filtro.LarguraPeca.GetValueOrDefault(),
                        (float)filtro.EspessuraPeca.GetValueOrDefault(),
                        filtro.IdsProcessos.ObterComoString(),
                        filtro.IdsAplicacoes.ObterComoString(),
                        filtro.ApenasPecasAguardandoExpedicao,
                        filtro.ApenasPecasAguardandoEntradaEstoque,
                        filtro.IdsBeneficiamentos.ObterComoString(),
                        filtro.PlanoCorte,
                        filtro.NumeroEtiquetaChapa,
                        (uint)filtro.TipoFastDelivery.GetValueOrDefault(),
                        filtro.ApenasPecasParadasNaProducao,
                        filtro.ApenasPecasRepostas,
                        (uint)filtro.IdLoja.GetValueOrDefault(),
                        (int?)filtro.TipoProdutosComposicao,
                        0,
                        filtro.ObterPrimeiroRegistroRetornar()));
            }
        }

        /// <summary>
        /// Recupera a lista de peças de composição para a tela de consulta de produção.
        /// </summary>
        /// <param name="id">O identificador do produto 'pai'.</param>
        /// <param name="filtro">Os dados informados para filtro na tela.</param>
        /// <returns>Uma lista JSON com as peças em produção.</returns>
        [HttpGet]
        [Route("{id:int}/composicao")]
        [SwaggerResponse(200, "Produtos de composição em produção sem paginação (apenas uma página de retorno) ou última página retornada.", Type = typeof(IEnumerable<IdNomeDto>))]
        [SwaggerResponse(204, "Produtos de composição em produção não encontradas para o filtro informado.")]
        [SwaggerResponse(206, "Produtos de composição em produção paginadas (qualquer página, exceto a última).", Type = typeof(IEnumerable<IdNomeDto>))]
        [SwaggerResponse(400, "Filtro ou id inválido informado (campo com valor ou formato inválido).", Type = typeof(MensagemDto))]
        [SwaggerResponse(404, "Peça de produção não encontrada para o id informado.", Type = typeof(MensagemDto))]
        public IHttpActionResult ObterPecasComposicao(int id, [FromUri] Models.Producao.V1.Composicao.FiltroDto filtro)
        {
            using (var sessao = new GDATransaction())
            {
                var validacao = this.ValidarExistenciaIdProdutoProducao(sessao, id);

                if (validacao != null)
                {
                    return validacao;
                }

                var produtosComposicao = ProdutoPedidoProducaoDAO.Instance.PesquisarProdutosProducaoFilho(
                    sessao,
                    id,
                    filtro.ObterTraducaoOrdenacao(),
                    filtro.ObterPrimeiroRegistroRetornar(),
                    filtro.NumeroRegistros);

                return this.ListaPaginada(
                    produtosComposicao.Select(p => new ListaDto(p)),
                    filtro,
                    () => ProdutoPedidoProducaoDAO.Instance.PesquisarProdutosProducaoFilhoCount(
                        sessao,
                        id));
            }
        }

        /// <summary>
        /// Recupera a lista de situações de produção possíveis.
        /// </summary>
        /// <returns>Uma lista JSON com os dados básicos das situações de produção.</returns>
        [HttpGet]
        [Route("situacoes")]
        [SwaggerResponse(200, "Situações de produção encontradas.", Type = typeof(IEnumerable<IdNomeDto>))]
        [SwaggerResponse(204, "Situações de produção não encontradas.")]
        public IHttpActionResult ObterSituacoes()
        {
            using (var sessao = new GDATransaction())
            {
                var situacoes = new ConversorEnum<SituacaoProducao>()
                    .ObterTraducao();

                return this.Lista(situacoes);
            }
        }

        /// <summary>
        /// Recupera a lista de tipos de situações de produção possíveis.
        /// </summary>
        /// <returns>Uma lista JSON com os dados básicos dos tipso de situações de produção.</returns>
        [HttpGet]
        [Route("tiposSituacoes")]
        [SwaggerResponse(200, "Tipos de situações de produção encontradas.", Type = typeof(IEnumerable<IdNomeDto>))]
        [SwaggerResponse(204, "Tipos de situações de produção não encontradas.")]
        public IHttpActionResult ObterTiposSituacoes()
        {
            using (var sessao = new GDATransaction())
            {
                var tiposSituacoes = new ConversorEnum<TipoSituacaoProducao>()
                    .ObterTraducao();

                return this.Lista(tiposSituacoes);
            }
        }

        /// <summary>
        /// Recupera a lista de tipos de pedidos possíveis.
        /// </summary>
        /// <returns>Uma lista JSON com os dados básicos dos tipos de pedido.</returns>
        [HttpGet]
        [Route("tiposPedidos")]
        [SwaggerResponse(200, "Tipos de pedidos encontrados.", Type = typeof(IEnumerable<IdNomeDto>))]
        [SwaggerResponse(204, "Tipos de pedidos não encontrados.")]
        public IHttpActionResult ObterTiposPedidos()
        {
            using (var sessao = new GDATransaction())
            {
                var tiposPedidos = new ConversorEnum<TipoPedido>()
                    .ObterTraducao();

                return this.Lista(tiposPedidos);
            }
        }

        /// <summary>
        /// Recupera a lista de tipos de peças que podem ser exibidas.
        /// </summary>
        /// <returns>Uma lista JSON com os dados básicos dos tipos de peças a exibir.</returns>
        [HttpGet]
        [Route("tiposPecasExibir")]
        [SwaggerResponse(200, "Tipos de peças a exibir encontradas.", Type = typeof(IEnumerable<IdNomeDto>))]
        [SwaggerResponse(204, "Tipos de peças a exibir não encontradas.")]
        public IHttpActionResult ObterTiposPecasExibir()
        {
            using (var sessao = new GDATransaction())
            {
                var tiposPedidos = new ConversorEnum<TipoPecaExibir>()
                    .ObterTraducao();

                return this.Lista(tiposPedidos);
            }
        }

        /// <summary>
        /// Recupera a lista de tipos de produtos de composição que podem ser utilizados para filtro.
        /// </summary>
        /// <returns>Uma lista JSON com os dados básicos dos tipos de produtos de composição.</returns>
        [HttpGet]
        [Route("tiposProdutosComposicao")]
        [SwaggerResponse(200, "Tipos de produtos de composição encontrados.", Type = typeof(IEnumerable<IdNomeDto>))]
        [SwaggerResponse(204, "Tipos de produtos de composição não encontrados.")]
        public IHttpActionResult ObterTiposProdutosComposicao()
        {
            using (var sessao = new GDATransaction())
            {
                var tiposProdutos = new ConversorEnum<TipoProdutoComposicao>()
                    .ObterTraducao();

                return this.Lista(tiposProdutos);
            }
        }

        /// <summary>
        /// Recupera a lista de tipos de 'fast delivery' que podem ser utilizados para filtro.
        /// </summary>
        /// <returns>Uma lista JSON com os dados básicos dos tipos de produtos de composição.</returns>
        [HttpGet]
        [Route("tiposFastDelivery")]
        [SwaggerResponse(200, "Tipos de 'fast delivery' encontrados.", Type = typeof(IEnumerable<IdNomeDto>))]
        [SwaggerResponse(204, "Tipos de 'fast delivery' não encontrados.")]
        public IHttpActionResult ObterTiposFastDelivery()
        {
            using (var sessao = new GDATransaction())
            {
                var tiposProdutos = new ConversorEnum<TipoFastDelivery>()
                    .ObterTraducao();

                return this.Lista(tiposProdutos);
            }
        }
    }
}
