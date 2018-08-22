// <copyright file="GetProdutosController.cs" company="Sync Softwares">
// Copyright (c) Sync Softwares. Todos os direitos reservados.
// </copyright>

using GDA;
using Glass.API.Backend.Helper;
using Glass.API.Backend.Helper.Produtos;
using Glass.API.Backend.Helper.Respostas;
using Glass.API.Backend.Models.Produtos.Filtro;
using Glass.Data.DAL;
using Swashbuckle.Swagger.Annotations;
using System.Web.Http;

namespace Glass.API.Backend.Controllers.Produtos.V1
{
    /// <summary>
    /// Controller de produtos.
    /// </summary>
    public partial class ProdutosController : BaseController
    {
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
