// <copyright file="IValidacaoFiltro.cs" company="Sync Softwares">
// Copyright (c) Sync Softwares. Todos os direitos reservados.
// </copyright>

using GDA;
using Glass.Data.Model;
using System.Collections.Generic;
using System.Web.Http;

namespace Glass.API.Backend.Helper.Clientes.Estrategias.Filtro
{
    /// <summary>
    /// Interface com as validações feitas para o filtro de clientes.
    /// </summary>
    internal interface IValidacaoFiltro
    {
        /// <summary>
        /// Validação executada antes da busca.
        /// </summary>
        /// <param name="sessao">A transação do banco de dados que será utilizada.</param>
        /// <param name="id">O identificador do cliente para o filtro.</param>
        /// <param name="nome">O nome do cliente para o filtro.</param>
        /// <returns>A resposta HTTP, caso ocorra algum erro de validação.</returns>
        IHttpActionResult ValidarAntesBusca(GDASession sessao, int? id, string nome);

        /// <summary>
        /// Validação executada depois da busca.
        /// </summary>
        /// <param name="sessao">A transação do banco de dados que será utilizada.</param>
        /// <param name="id">O identificador do cliente para o filtro.</param>
        /// <param name="nome">O nome do cliente para o filtro.</param>
        /// <param name="clientes">Os clientes encontrados.</param>
        /// <returns>A resposta HTTP, caso ocorra algum erro de validação.</returns>
        IHttpActionResult ValidarDepoisBusca(GDASession sessao, int? id, string nome, ref IEnumerable<Cliente> clientes);
    }
}
