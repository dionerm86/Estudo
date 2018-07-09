// <copyright file="PostProdutosController.cs" company="Sync Softwares">
// Copyright (c) Sync Softwares. Todos os direitos reservados.
// </copyright>

using GDA;
using Glass.API.Backend.Helper;
using Glass.API.Backend.Helper.Produtos;
using Glass.API.Backend.Helper.Produtos.Estrategias.CalculoTotal;
using Glass.API.Backend.Helper.Respostas;
using Glass.API.Backend.Models.Produtos.CalculoAreaM2;
using Glass.API.Backend.Models.Produtos.CalculoTotal;
using Swashbuckle.Swagger.Annotations;
using System;
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
        /// <param name="dadosProduto">Os dados do produto para realização do cálculo.</param>
        /// <returns>Um objeto JSON com os dados de produtos para o controle.</returns>
        [HttpPost]
        [Route("calcularTotal")]
        [SwaggerResponse(200, "Total calculado.", Type = typeof(TotalCalculadoDto))]
        [SwaggerResponse(400, "Erro de validação.", Type = typeof(MensagemDto))]
        public IHttpActionResult CalcularTotal([FromBody] Models.Produtos.CalculoTotal.DadosProdutoDto dadosProduto)
        {
            if (dadosProduto == null)
            {
                return this.ErroValidacao("Os dados do produto são obrigatórios.");
            }

            dadosProduto.DadosAdicionaisValidacao = new ConversorDadosAdicionaisParaValidacao()
                .ConverterDadosCodificados(dadosProduto.DadosAdicionaisValidacao);

            var estrategiaValidacao = ValidacaoFactory.ObterEstrategiaCalculoTotal(this, dadosProduto.TipoValidacao);

            using (var sessao = new GDATransaction())
            {
                var validacao = estrategiaValidacao.ValidarAntesCalculo(sessao, dadosProduto.DadosAdicionaisValidacao);

                if (validacao != null)
                {
                    return validacao;
                }

                var totalCalculado = new TotalCalculadoDto
                {
                    Total = this.CalcularTotal(sessao, dadosProduto, estrategiaValidacao),
                };

                validacao = estrategiaValidacao.ValidarDepoisCalculo(sessao, totalCalculado, dadosProduto.DadosAdicionaisValidacao);

                if (validacao != null)
                {
                    return validacao;
                }

                return this.Item(totalCalculado);
            }
        }

        /// <summary>
        /// Recupera um produto para o controle de filtro.
        /// </summary>
        /// <param name="dadosProduto">Os dados do produto para realização do cálculo.</param>
        /// <returns>Um objeto JSON com os dados de produtos para o controle.</returns>
        [HttpPost]
        [Route("calcularAreaM2")]
        [SwaggerResponse(200, "Área calculada.", Type = typeof(AreaCalculadaDto))]
        [SwaggerResponse(400, "Erro de validação.", Type = typeof(MensagemDto))]
        public IHttpActionResult CalcularAreaM2([FromBody] Models.Produtos.CalculoAreaM2.DadosProdutoDto dadosProduto)
        {
            if (dadosProduto == null)
            {
                return this.ErroValidacao("Os dados do produto são obrigatórios.");
            }

            dadosProduto.DadosAdicionaisValidacao = new ConversorDadosAdicionaisParaValidacao()
                .ConverterDadosCodificados(dadosProduto.DadosAdicionaisValidacao);

            var estrategiaValidacao = ValidacaoFactory.ObterEstrategiaCalculoAreaM2(this, dadosProduto.TipoValidacao);

            using (var sessao = new GDATransaction())
            {
                var validacao = estrategiaValidacao.ValidarAntesCalculo(sessao, dadosProduto.DadosAdicionaisValidacao);

                if (validacao != null)
                {
                    return validacao;
                }

                var areaCalculada = new AreaCalculadaDto
                {
                    AreaM2 = this.CalcularAreaM2Real(sessao, dadosProduto),
                    AreaM2Calculo = this.CalcularAreaM2Calculo(sessao, dadosProduto, true),
                    AreaM2CalculoSemChapaDeVidro = this.CalcularAreaM2Calculo(sessao, dadosProduto, false),
                };

                validacao = estrategiaValidacao.ValidarDepoisCalculo(sessao, areaCalculada, dadosProduto.DadosAdicionaisValidacao);

                if (validacao != null)
                {
                    return validacao;
                }

                return this.Item(areaCalculada);
            }
        }

        private double CalcularAreaM2Real(GDASession sessao, Models.Produtos.CalculoAreaM2.DadosProdutoDto dadosProduto)
        {
            return Math.Round(
                Global.CalculosFluxo.ArredondaM2(
                    sessao,
                    dadosProduto.Largura,
                    (int)dadosProduto.Altura,
                    (float)dadosProduto.Quantidade,
                    dadosProduto.IdProduto,
                    dadosProduto.Redondo,
                    (float)dadosProduto.Espessura,
                    dadosProduto.CalcularMultiploDe5),
                2);
        }

        private double CalcularAreaM2Calculo(GDASession sessao, Models.Produtos.CalculoAreaM2.DadosProdutoDto dadosProduto, bool usarChapaDeVidro)
        {
            return Math.Round(
                Global.CalculosFluxo.CalcM2Calculo(
                    sessao,
                    (uint)dadosProduto.IdCliente,
                    (int)dadosProduto.Altura,
                    dadosProduto.Largura,
                    (float)dadosProduto.Quantidade,
                    dadosProduto.IdProduto,
                    dadosProduto.Redondo,
                    dadosProduto.NumeroBeneficiamentosParaAreaMinima,
                    0,
                    usarChapaDeVidro,
                    (float)dadosProduto.Espessura,
                    dadosProduto.CalcularMultiploDe5),
                2);
        }

        private decimal CalcularTotal(GDASession sessao, Models.Produtos.CalculoTotal.DadosProdutoDto dadosProduto, IValidacaoCalculoTotal estrategiaValidacao)
        {
            return Global.CalculosFluxo.CalcularTotal(
                sessao,
                dadosProduto.IdCliente,
                dadosProduto.IdProduto,
                dadosProduto.Altura,
                dadosProduto.Largura,
                dadosProduto.Quantidade,
                dadosProduto.QuantidadeAmbiente,
                dadosProduto.AreaM2,
                dadosProduto.AreaCalculadaM2,
                dadosProduto.ValorUnitario,
                estrategiaValidacao.ObterPercentualDescontoQuantidade(dadosProduto.DadosAdicionaisValidacao),
                dadosProduto.CalcularMultiploDe5,
                dadosProduto.NumeroBeneficiamentosParaAreaMinima);
        }
    }
}
