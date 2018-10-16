// <copyright file="GetDescontoQuantidadeController.cs" company="Sync Softwares">
// Copyright (c) Sync Softwares. Todos os direitos reservados.
// </copyright>

using GDA;
using Glass.API.Backend.Helper.Respostas;
using Glass.API.Backend.Models.DescontoQuantidade.V1.Dados;
using Glass.Configuracoes;
using Glass.Data.DAL;
using Swashbuckle.Swagger.Annotations;
using System.Web.Http;

namespace Glass.API.Backend.Controllers.DescontoQuantidade.V1
{
    /// <summary>
    /// Controller de desconto por quantidade.
    /// </summary>
    public partial class DescontoQuantidadeController : BaseController
    {
        /// <summary>
        /// Recupera os dados de desconto por quantidade para o controle.
        /// </summary>
        /// <param name="dadosEntrada">Os dados de entrada para o método.</param>
        /// <returns>Um JSON com os dados de desconto por quantidade encontrados.</returns>
        [HttpGet]
        [Route("dados")]
        [SwaggerResponse(200, "Dados encontrados.", Type = typeof(DadosDescontoQuantidadeDto))]
        [SwaggerResponse(204, "Dados não encontrados.")]
        [SwaggerResponse(400, "Filtros não informados.", Type = typeof(MensagemDto))]
        public IHttpActionResult ObterDados([FromUri] DadosEntradaDto dadosEntrada)
        {
            dadosEntrada = dadosEntrada ?? new DadosEntradaDto();

            if (dadosEntrada.IdProduto == 0)
            {
                return this.ErroValidacao("O identificador do produto é obrigatório.");
            }

            if (dadosEntrada.IdCliente == 0)
            {
                return this.ErroValidacao("O identificador do cliente é obrigatório.");
            }

            using (var sessao = new GDATransaction())
            {
                var percentualDesconto = this.ObterPercentualDescontoPorQuantidade(
                    sessao,
                    dadosEntrada);

                var percentualTabela = this.ObterPercentualDescontoTabela(
                    sessao,
                    dadosEntrada);

                return this.Item(new DadosDescontoQuantidadeDto
                {
                    PercentualDescontoPorQuantidade = percentualDesconto,
                    PercentualDescontoTabela = percentualTabela,
                });
            }
        }

        private double ObterPercentualDescontoPorQuantidade(GDASession sessao, DadosEntradaDto dadosEntrada)
        {
            if (!PedidoConfig.Desconto.DescontoPorProduto)
            {
                return 0;
            }

            return DescontoQtdeDAO.Instance.GetPercDescontoByProd(
                sessao,
                (uint)dadosEntrada.IdProduto,
                (int)dadosEntrada.Quantidade);
        }

        private double ObterPercentualDescontoTabela(GDASession sessao, DadosEntradaDto dadosEntrada)
        {
            var idGrupoProd = dadosEntrada.IdGrupoProduto
                ?? ProdutoDAO.Instance.ObtemIdGrupoProd(sessao, dadosEntrada.IdProduto);

            var idSubgrupoProd = dadosEntrada.IdSubgrupoProduto
                ?? ProdutoDAO.Instance.ObtemIdSubgrupoProd(sessao, dadosEntrada.IdProduto);

            var descontoCliente = DescontoAcrescimoClienteDAO.Instance.GetDescontoAcrescimo(
                sessao,
                (uint)dadosEntrada.IdCliente,
                idGrupoProd,
                idSubgrupoProd,
                dadosEntrada.IdProduto,
                null,
                null);

            return descontoCliente.Desconto;
        }
    }
}
