// <copyright file="ValidacaoCalculoTotalPedidoStrategy.cs" company="Sync Softwares">
// Copyright (c) Sync Softwares. Todos os direitos reservados.
// </copyright>

using GDA;
using Glass.API.Backend.Helper.Produtos.Estrategias.Filtro;
using Glass.API.Backend.Models.Produtos.CalculoTotal;
using Newtonsoft.Json;
using System;
using System.Web.Http;

namespace Glass.API.Backend.Helper.Produtos.Estrategias.CalculoTotal
{
    /// <summary>
    /// Classe para ignorar a validação do cálculo de área em m².
    /// </summary>
    internal class ValidacaoCalculoTotalPedidoStrategy : IValidacaoCalculoTotal
    {
        private readonly ApiController apiController;

        /// <summary>
        /// Inicia uma nova instância da classe <see cref="ValidacaoCalculoTotalPedidoStrategy"/>.
        /// </summary>
        /// <param name="apiController">O controller que está sendo executado.</param>
        public ValidacaoCalculoTotalPedidoStrategy(ApiController apiController)
        {
            this.apiController = apiController;
        }

        /// <inheritdoc/>
        public IHttpActionResult ValidarAntesCalculo(GDASession sessao, string dadosAdicionaisValidacao)
        {
            try
            {
                var adicionais = JsonConvert.DeserializeObject<DadosAdicionaisFiltroPedidoDto>(dadosAdicionaisValidacao);

                if (adicionais == null)
                {
                    throw new ArgumentException("dadosAdicionaisValidacao");
                }
            }
            catch
            {
                string formato = JsonConvert.SerializeObject(new DadosAdicionaisFiltroPedidoDto());
                return this.apiController.ErroValidacao("Os dados adicionais não foram informados corretamente. "
                    + $"É esperado um objeto no seguinte formato: {formato}");
            }

            return null;
        }

        /// <inheritdoc/>
        public IHttpActionResult ValidarDepoisCalculo(GDASession sessao, TotalCalculadoDto totalCalculado, string dadosAdicionaisValidacao)
        {
            return null;
        }

        /// <inheritdoc/>
        public double ObterPercentualDescontoQuantidade(string dadosAdicionaisValidacao)
        {
            var dadosAdicionais = JsonConvert.DeserializeObject<DadosAdicionaisFiltroPedidoDto>(dadosAdicionaisValidacao);
            return dadosAdicionais.PercentualDescontoPorQuantidade;
        }
    }
}
