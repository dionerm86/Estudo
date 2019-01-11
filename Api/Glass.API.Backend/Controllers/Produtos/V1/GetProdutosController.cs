// <copyright file="GetProdutosController.cs" company="Sync Softwares">
// Copyright (c) Sync Softwares. Todos os direitos reservados.
// </copyright>

using GDA;
using Glass.API.Backend.Helper;
using Glass.API.Backend.Helper.Produtos;
using Glass.API.Backend.Helper.Respostas;
using Glass.API.Backend.Models.Produtos.V1.Filtro;
using Glass.Data.DAL;
using Swashbuckle.Swagger.Annotations;
using System.Collections.Generic;
using System.Linq;
using System.Web.Http;

namespace Glass.API.Backend.Controllers.Produtos.V1
{
    /// <summary>
    /// Controller de produtos.
    /// </summary>
    public partial class ProdutosController : BaseController
    {
        /// <summary>
        /// Recupera as configurações usadas pela tela de listagem de produtos.
        /// </summary>
        /// <returns>Um objeto JSON com as configurações da tela.</returns>
        [HttpGet]
        [Route("configuracoes")]
        [SwaggerResponse(200, "Configurações recuperadas.", Type = typeof(Models.Produtos.V1.Configuracoes.ListaDto))]
        public IHttpActionResult ObterConfiguracoesListaProdutos()
        {
            using (var sessao = new GDATransaction())
            {
                var configuracoes = new Models.Produtos.V1.Configuracoes.ListaDto();
                return this.Item(configuracoes);
            }
        }

        /// <summary>
        /// Recupera a lista de produtos.
        /// </summary>
        /// <param name="filtro">Os filtros para a busca dos itens.</param>
        /// <returns>Uma lista JSON com os dados dos produtos.</returns>
        [HttpGet]
        [Route("")]
        [SwaggerResponse(200, "Produtos sem paginação (apenas uma página de retorno) ou última página retornada.", Type = typeof(IEnumerable<Models.Produtos.V1.Lista.ListaDto>))]
        [SwaggerResponse(204, "Produtos não encontrados para o filtro informado.")]
        [SwaggerResponse(206, "Produtos paginados (qualquer página, exceto a última).", Type = typeof(IEnumerable<Models.Produtos.V1.Lista.ListaDto>))]
        [SwaggerResponse(400, "Filtro inválido informado (campo com valor ou formato inválido).", Type = typeof(MensagemDto))]
        public IHttpActionResult ObterListaProdutos([FromUri] Models.Produtos.V1.Lista.FiltroDto filtro)
        {
            using (var sessao = new GDATransaction())
            {
                filtro = filtro ?? new Models.Produtos.V1.Lista.FiltroDto();

                var produtos = Microsoft.Practices.ServiceLocation.ServiceLocator
                    .Current.GetInstance<Global.Negocios.IProdutoFluxo>()
                    .PesquisarProdutos(
                        filtro.Codigo,
                        filtro.Descricao,
                        filtro.Situacao ?? Situacao.Ativo,
                        null,
                        null,
                        null,
                        filtro.IdGrupo != null ? filtro.IdGrupo.ToString() : null,
                        filtro.IdSubgrupo != null ? filtro.IdSubgrupo.ToString() : null,
                        null,
                        false,
                        false,
                        filtro.ValorAlturaInicio,
                        filtro.ValorAlturaFim,
                        filtro.ValorLarguraInicio,
                        filtro.ValorLarguraFim,
                        null);

                ((Colosoft.Collections.IVirtualList)produtos).Configure(filtro.NumeroRegistros);
                ((Colosoft.Collections.ISortableCollection)produtos).ApplySort(filtro.ObterTraducaoOrdenacao());

                return this.ListaPaginada(
                    produtos
                        .Skip(filtro.ObterPrimeiroRegistroRetornar())
                        .Take(filtro.NumeroRegistros)
                        .Select(c => new Models.Produtos.V1.Lista.ListaDto(c)),
                    filtro,
                    () => produtos.Count);
            }
        }

        /// <summary>
        /// Recupera um produto para o controle de filtro.
        /// </summary>
        /// <param name="codigoInterno">O código interno do produto a ser buscado.</param>
        /// <param name="tipoValidacao">O tipo de validação que será feita.</param>
        /// <param name="dadosAdicionaisValidacao">Os dados adicionais para a validação do produto, se necessário.</param>
        /// <returns>Um objeto JSON com os dados de produtos para o controle.</returns>
        [HttpGet]
        [Route("filtro")]
        [SwaggerResponse(200, "Produto encontrado.", Type = typeof(ProdutoDto))]
        [SwaggerResponse(400, "Codigo interno não informado.", Type = typeof(MensagemDto))]
        [SwaggerResponse(404, "Produto não encontrado.", Type = typeof(MensagemDto))]
        public IHttpActionResult ObterParaFiltro(string codigoInterno, string tipoValidacao = null, string dadosAdicionaisValidacao = null)
        {
            if (string.IsNullOrWhiteSpace(codigoInterno))
            {
                return this.ErroValidacao("O código do produto é obrigatório.");
            }

            dadosAdicionaisValidacao = new ConversorDadosAdicionaisParaValidacao()
                .ConverterDadosCodificados(dadosAdicionaisValidacao);

            var estrategiaValidacao = ValidacaoFactory.ObterEstrategiaFiltro(this, tipoValidacao);

            using (var sessao = new GDATransaction())
            {
                var validacao = estrategiaValidacao.ValidarAntesBusca(sessao, codigoInterno, dadosAdicionaisValidacao);

                if (validacao != null)
                {
                    return validacao;
                }

                var produto = ProdutoDAO.Instance.GetByCodInterno(sessao, codigoInterno);

                if (produto == null)
                {
                    return this.NaoEncontrado(string.Format("Produto {0} não encontrado.", codigoInterno));
                }

                validacao = estrategiaValidacao.ValidarDepoisBusca(sessao, codigoInterno, dadosAdicionaisValidacao, produto);

                if (validacao != null)
                {
                    return validacao;
                }

                var produtoDto = estrategiaValidacao.ObterProduto(sessao, dadosAdicionaisValidacao, produto);
                return this.Item(produtoDto);
            }
        }
    }
}
