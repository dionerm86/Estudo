// <copyright file="ValidacaoFactory.cs" company="Sync Softwares">
// Copyright (c) Sync Softwares. Todos os direitos reservados.
// </copyright>

using Glass.API.Backend.Helper.Produtos.Estrategias.CalculoAreaM2;
using Glass.API.Backend.Helper.Produtos.Estrategias.CalculoTotal;
using Glass.API.Backend.Helper.Produtos.Estrategias.Filtro;
using System.Web.Http;

namespace Glass.API.Backend.Helper.Produtos
{
    /// <summary>
    /// Classe que recupera a estratégia de validação.
    /// </summary>
    internal static class ValidacaoFactory
    {
        /// <summary>
        /// Recupera a estratégia de validação para a busca do controle de produtos.
        /// </summary>
        /// <param name="apiController">O controller que está sendo executado.</param>
        /// <param name="tipoValidacao">O tipo de validação que será feita.</param>
        /// <returns>O objeto que contém as validações a serem feitas, de acordo com o tipo.</returns>
        public static IValidacaoFiltro ObterEstrategiaFiltro(ApiController apiController, string tipoValidacao)
        {
            switch ((tipoValidacao ?? string.Empty).ToLowerInvariant())
            {
                case "pedido":
                    return new ValidacaoFiltroPedidoStrategy(apiController, false);
                case "produtopedido":
                    return new ValidacaoFiltroPedidoStrategy(apiController, true);
            }

            return new ValidacaoFiltroSemOperacaoStrategy();
        }

        /// <summary>
        /// Recupera a estratégia de validação para o cálculo de área em m².
        /// </summary>
        /// <param name="apiController">O controller que está sendo executado.</param>
        /// <param name="tipoValidacao">O tipo de validação que será feita.</param>
        /// <returns>O objeto que contém as validações a serem feitas, de acordo com o tipo.</returns>
        public static IValidacaoCalculoAreaM2 ObterEstrategiaCalculoAreaM2(ApiController apiController, string tipoValidacao)
        {
            return new ValidacaoCalculoAreaM2SemOperacaoStrategy();
        }

        /// <summary>
        /// Recupera a estratégia de validação para o cálculo de valor total.
        /// </summary>
        /// <param name="apiController">O controller que está sendo executado.</param>
        /// <param name="tipoValidacao">O tipo de validação que será feita.</param>
        /// <returns>O objeto que contém as validações a serem feitas, de acordo com o tipo.</returns>
        public static IValidacaoCalculoTotal ObterEstrategiaCalculoTotal(ApiController apiController, string tipoValidacao)
        {
            switch ((tipoValidacao ?? string.Empty).ToLowerInvariant())
            {
                case "pedido":
                    return new ValidacaoCalculoTotalPedidoStrategy(apiController);
            }

            return new ValidacaoCalculoTotalSemOperacaoStrategy();
        }
    }
}
