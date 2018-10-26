// <copyright file="IntegradoresController.cs" company="Sync Softwares">
// Copyright (c) Sync Softwares. Todos os direitos reservados.
// </copyright>

using System.Web.Http;

namespace Glass.API.Backend.Controllers.Integracao.V1.Integradores
{
    /// <summary>
    /// Controller de integradores.
    /// </summary>
    [RoutePrefix("api/v1/integracao/integradores")]
    public partial class IntegradoresController : BaseController
    {
        /// <summary>
        /// Obtém o gerenciador dos integradores.
        /// </summary>
        private Glass.Integracao.GerenciadorIntegradores GerenciadorIntegradores =>
             Microsoft.Practices.ServiceLocation.ServiceLocator
                 .Current.GetInstance<Glass.Integracao.GerenciadorIntegradores>();
    }
}
