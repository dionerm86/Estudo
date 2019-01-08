// <copyright file="GetPrecosTabelaClienteController.cs" company="Sync Softwares">
// Copyright (c) Sync Softwares. Todos os direitos reservados.
// </copyright>

using GDA;
using Glass.API.Backend.Helper;
using Glass.API.Backend.Helper.Produtos.PrecosTabelaCliente;
using Glass.API.Backend.Helper.Respostas;
using Glass.API.Backend.Models.Produtos.V1.PrecosTabelaCliente;
using Glass.API.Backend.Models.Produtos.V1.PrecosTabelaCliente.Lista;
using Glass.Data.DAL;
using Swashbuckle.Swagger.Annotations;
using System.Collections.Generic;
using System.Linq;
using System.Web.Http;

namespace Glass.API.Backend.Controllers.Produtos.V1.PrecosTabelaCliente
{
    /// <summary>
    /// Controller de preços de tabela por cliente.
    /// </summary>
    public partial class PrecosTabelaClienteController : BaseController
    {
        /// <summary>
        /// Recupera as configurações usadas pela tela de listagem de preços de tabela por cliente.
        /// </summary>
        /// <returns>Um objeto JSON com as configurações da tela.</returns>
        [HttpGet]
        [Route("configuracoes/precosTabelaCliente")]
        [SwaggerResponse(200, "Configurações recuperadas.", Type = typeof(Models.Produtos.V1.PrecosTabelaCliente.Configuracoes.ListaDto))]
        public IHttpActionResult ObterConfiguracoesListaPrecoTabelaCliente()
        {
            using (var sessao = new GDATransaction())
            {
                var configuracoes = new Models.Produtos.V1.PrecosTabelaCliente.Configuracoes.ListaDto();
                return this.Item(configuracoes);
            }
        }

        /// <summary>
        /// Recupera a lista de preços de tabela por cliente.
        /// </summary>
        /// <param name="filtro">Os filtros para a busca dos itens.</param>
        /// <returns>Uma lista JSON com os dados dos preços de tabela por cliente.</returns>
        [HttpGet]
        [Route("precosTabelaCliente")]
        [SwaggerResponse(200, "Preços de tabela de cliente encontrados sem paginação (apenas uma página de retorno) ou última página retornada.", Type = typeof(IEnumerable<ListaDto>))]
        [SwaggerResponse(204, "Preços de tabela de cliente não encontrados para o filtro informado.")]
        [SwaggerResponse(206, "Preços de tabela de cliente paginados (qualquer página, exceto a última).", Type = typeof(IEnumerable<ListaDto>))]
        [SwaggerResponse(400, "Filtro inválido informado (campo com valor ou formato inválido).", Type = typeof(MensagemDto))]
        public IHttpActionResult ObterListaPrecoTabelaCliente([FromUri] FiltroDto filtro)
        {
            using (var sessao = new GDATransaction())
            {
                filtro = filtro ?? new FiltroDto();

                var precosTabelaCliente = ProdutoDAO.Instance.GetListPrecoTab(
                        (uint)(filtro.IdCliente ?? 0),
                        filtro.NomeCliente,
                        (uint)(filtro.IdGrupoProduto ?? 0),
                        filtro.IdsSubgrupoProduto != null && filtro.IdsSubgrupoProduto.Any() ? string.Join(",", filtro.IdsSubgrupoProduto) : null,
                        filtro.CodigoProduto,
                        filtro.DescricaoProduto,
                        filtro.TipoValorTabela ?? 0,
                        filtro.ValorAlturaInicio ?? 0,
                        filtro.ValorAlturaFim ?? 0,
                        filtro.ValorLarguraInicio ?? 0,
                        filtro.ValorLarguraFim ?? 0,
                        filtro.OrdenacaoManual ?? 0,
                        filtro.ApenasComDesconto.GetValueOrDefault(false),
                        false,
                        filtro.ObterTraducaoOrdenacao(),
                        filtro.ObterPrimeiroRegistroRetornar(),
                        filtro.NumeroRegistros);

                return this.ListaPaginada(
                    precosTabelaCliente.Select(ptc => new ListaDto(ptc)),
                    filtro,
                    () => ProdutoDAO.Instance.GetCountPrecoTab(
                        (uint)(filtro.IdCliente ?? 0),
                        filtro.NomeCliente,
                        (uint)(filtro.IdGrupoProduto ?? 0),
                        filtro.IdsSubgrupoProduto != null && filtro.IdsSubgrupoProduto.Any() ? string.Join(",", filtro.IdsSubgrupoProduto) : null,
                        filtro.CodigoProduto,
                        filtro.DescricaoProduto,
                        filtro.TipoValorTabela ?? 0,
                        filtro.ValorAlturaInicio ?? 0,
                        filtro.ValorAlturaFim ?? 0,
                        filtro.ValorLarguraInicio ?? 0,
                        filtro.ValorLarguraFim ?? 0,
                        filtro.OrdenacaoManual ?? 0,
                        filtro.ApenasComDesconto.GetValueOrDefault(false),
                        false));
            }
        }

        /// <summary>
        /// Recupera os tipos de valor de tabela para uso no controle.
        /// </summary>
        /// <returns>Um objeto JSON com os tipos de valor de tabela.</returns>
        [HttpGet]
        [Route("tiposValorTabela")]
        [SwaggerResponse(200, "Tipos de valor de tabela encontrados.", Type = typeof(TipoValorTabela))]
        public IHttpActionResult ObterListaTiposValorTabela()
        {
            using (var sessao = new GDATransaction())
            {
                var tiposValorTabela = new ConversorEnum<TipoValorTabela>()
                    .ObterTraducao();

                return this.Lista(tiposValorTabela);
            }
        }
    }
}