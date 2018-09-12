// <copyright file="LogController.cs" company="Sync Softwares">
// Copyright (c) Sync Softwares. Todos os direitos reservados.
// </copyright>

using Glass.Data.Model;
using System.Web.Http;

namespace Glass.API.Backend.Controllers.Log.V1
{
    /// <summary>
    /// Controller de logs do sistema.
    /// </summary>
    [RoutePrefix("api/v1/log")]
    public partial class LogController : BaseController
    {
        private IHttpActionResult ValidarTabelaAlteracao(LogAlteracao.TabelaAlteracao? tabela)
        {
            if (!tabela.HasValue)
            {
                return this.ErroValidacao("Tabela informada é inválida.");
            }

            return null;
        }

        private IHttpActionResult ValidarTabelaCancelamento(LogCancelamento.TabelaCancelamento? tabela)
        {
            if (!tabela.HasValue)
            {
                return this.ErroValidacao("Tabela informada é inválida.");
            }

            return null;
        }
    }
}
