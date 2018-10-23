// <copyright file="IValidacaoFiltro.cs" company="Sync Softwares">
// Copyright (c) Sync Softwares. Todos os direitos reservados.
// </copyright>

using GDA;
using Glass.API.Backend.Models.Cfops.V1.Filtro;
using Glass.Data.Model;
using System.Web.Http;

namespace Glass.API.Backend.Helper.Cfops.Estrategias.Filtro
{
    /// <summary>
    /// Interface com as validações feitas para o filtro de CFOP's.
    /// </summary>
    internal interface IValidacaoFiltro
    {
        /// <summary>
        /// Validação executada antes da busca.
        /// </summary>
        /// <param name="sessao">A transação do banco de dados que será utilizada.</param>
        /// <param name="codigoInterno">O código do produto que será buscado.</param>
        /// <param name="dadosAdicionaisValidacao">Os dados adicionais para a validação do produto, se necessário.</param>
        /// <returns>A resposta HTTP, caso ocorra algum erro de validação.</returns>
        IHttpActionResult ValidarAntesBusca(GDASession sessao, string codigoInterno, string dadosAdicionaisValidacao);

        /// <summary>
        /// Validação executada depois da busca.
        /// </summary>
        /// <param name="sessao">A transação do banco de dados que será utilizada.</param>
        /// <param name="codigoInterno">O código do CFOP que será buscado.</param>
        /// <param name="dadosAdicionaisValidacao">Os dados adicionais para a validação do CFOP, se necessário.</param>
        /// <param name="cfop">O CFOP encontrado.</param>
        /// <returns>A resposta HTTP, caso ocorra algum erro de validação.</returns>
        IHttpActionResult ValidarDepoisBusca(GDASession sessao, string codigoInterno, string dadosAdicionaisValidacao, Cfop cfop);

        /// <summary>
        /// Cria o CFOP, com base no tipo de validação.
        /// </summary>
        /// <param name="sessao">A transação do banco de dados que será utilizada.</param>
        /// <param name="dadosAdicionaisValidacao">Os dados adicionais para a validação do CFOP, se necessário.</param>
        /// <param name="cfop">O CFOP encontrado.</param>
        /// <returns>O DTO com o CFOP para o tipo de validação.</returns>
        CfopDto ObterCfop(GDASession sessao, string dadosAdicionaisValidacao, Cfop cfop);
    }
}
