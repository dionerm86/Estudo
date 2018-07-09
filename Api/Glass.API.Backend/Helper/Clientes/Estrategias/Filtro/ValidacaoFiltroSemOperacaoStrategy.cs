// <copyright file="ValidacaoFiltroSemOperacaoStrategy.cs" company="Sync Softwares">
// Copyright (c) Sync Softwares. Todos os direitos reservados.
// </copyright>

using GDA;
using Glass.Data.Model;
using System.Collections.Generic;
using System.Web.Http;

namespace Glass.API.Backend.Helper.Clientes.Estrategias.Filtro
{
    /// <summary>
    /// Classe para ignorar a validação do filtro de clientes.
    /// </summary>
    internal class ValidacaoFiltroSemOperacaoStrategy : IValidacaoFiltro
    {
        /// <inheritdoc/>
        public IHttpActionResult ValidarAntesBusca(GDASession sessao, int? id, string nome)
        {
            return null;
        }

        /// <inheritdoc/>
        public IHttpActionResult ValidarDepoisBusca(GDASession sessao, int? id, string nome, ref IEnumerable<Cliente> clientes)
        {
            return null;
        }
    }
}
