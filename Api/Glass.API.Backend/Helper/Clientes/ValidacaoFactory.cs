// <copyright file="ValidacaoFactory.cs" company="Sync Softwares">
// Copyright (c) Sync Softwares. Todos os direitos reservados.
// </copyright>

using Glass.API.Backend.Helper.Clientes.Estrategias.Filtro;
using System.Web.Http;

namespace Glass.API.Backend.Helper.Clientes
{
    /// <summary>
    /// Classe que recupera a estratégia de validação.
    /// </summary>
    internal static class ValidacaoFactory
    {
        /// <summary>
        /// Recupera a estratégia de validação para a busca do controle de clientes.
        /// </summary>
        /// <param name="apiController">O controller que está sendo executado.</param>
        /// <param name="tipoValidacao">O tipo de validação que será feita.</param>
        /// <returns>O objeto que contém as validações a serem feitas, de acordo com o tipo.</returns>
        public static IValidacaoFiltro ObterEstrategiaFiltro(ApiController apiController, string tipoValidacao)
        {
            switch ((tipoValidacao ?? string.Empty).ToLowerInvariant())
            {
                case "pedido":
                    return new ValidacaoFiltroPedidoStrategy(apiController);
            }

            return new ValidacaoFiltroSemOperacaoStrategy();
        }
    }
}
