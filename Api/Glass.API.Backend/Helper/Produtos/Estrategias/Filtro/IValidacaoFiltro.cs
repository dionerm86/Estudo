// <copyright file="IValidacaoFiltro.cs" company="Sync Softwares">
// Copyright (c) Sync Softwares. Todos os direitos reservados.
// </copyright>

using GDA;
using Glass.API.Backend.Models.Produtos.V1.Filtro;
using Glass.Data.Model;
using System.Web.Http;

namespace Glass.API.Backend.Helper.Produtos.Estrategias.Filtro
{
    /// <summary>
    /// Interface com as validações feitas para o filtro de produtos.
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
        /// <param name="codigoInterno">O código do produto que será buscado.</param>
        /// <param name="dadosAdicionaisValidacao">Os dados adicionais para a validação do produto, se necessário.</param>
        /// <param name="produto">O produto encontrado.</param>
        /// <returns>A resposta HTTP, caso ocorra algum erro de validação.</returns>
        IHttpActionResult ValidarDepoisBusca(GDASession sessao, string codigoInterno, string dadosAdicionaisValidacao, Produto produto);

        /// <summary>
        /// Cria o produto, com base no tipo de validação.
        /// </summary>
        /// <param name="sessao">A transação do banco de dados que será utilizada.</param>
        /// <param name="dadosAdicionaisValidacao">Os dados adicionais para a validação do produto, se necessário.</param>
        /// <param name="produto">O produto encontrado.</param>
        /// <returns>O DTO com o produto para o tipo de validação.</returns>
        ProdutoDto ObterProduto(GDASession sessao, string dadosAdicionaisValidacao, Produto produto);
    }
}
